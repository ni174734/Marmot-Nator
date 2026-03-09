using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyVisionChase : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase
    }

    public State state = State.Patrol;

    [Header("Patrol")]
    public float patrolSpeed = 2.5f;
    public float patrolRange = 3f;
    public float arriveDistance = 0.15f;

    [Header("Chase")]
    public Transform player;
    public float chaseSpeed = 4.5f;
    public float losePlayerTime = 1.0f;

    [Header("Vision")]
    public float viewDistance = 5f;
    [Range(0, 180)] public float viewAngle = 60f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Header("Tuning")]
    public float stickDeadzone = 0.05f;

    private Rigidbody2D rb;
    private float lastSeenTimer;

    private float spawnX;
    private float leftBound;
    private float rightBound;
    private int patrolDirection = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastSeenTimer = 999f;
    }

    void Start()
    {
        spawnX = transform.position.x;
        leftBound = spawnX - patrolRange;
        rightBound = spawnX + patrolRange;
    }

    void FixedUpdate()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        bool canSee = (player != null) && CanSeePlayer();

        if (canSee)
        {
            state = State.Chase;
            lastSeenTimer = 0f;
        }
        else
        {
            lastSeenTimer += Time.fixedDeltaTime;

            if (state == State.Chase && lastSeenTimer >= losePlayerTime)
                state = State.Patrol;
        }

        if (state == State.Patrol)
            DoPatrol();
        else
            DoChase();
    }

    void DoPatrol()
    {
        float x = transform.position.x;

        if (x >= rightBound - arriveDistance)
            patrolDirection = -1;
        else if (x <= leftBound + arriveDistance)
            patrolDirection = 1;

        rb.linearVelocity = new Vector2(patrolDirection * patrolSpeed, rb.linearVelocity.y);

        Face(patrolDirection);
    }

    void DoChase()
    {
        if (!player) return;

        float dx = player.position.x - transform.position.x;
        float dir = 0f;

        if (Mathf.Abs(dx) > stickDeadzone)
            dir = Mathf.Sign(dx);

        rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);

        Face(dir);
    }

    void Face(float dir)
    {
        if (dir == 0) return;

        Vector3 s = transform.localScale;
        s.x = Mathf.Sign(dir) * Mathf.Abs(s.x);
        transform.localScale = s;
    }

    bool CanSeePlayer()
    {
        Vector2 origin = rb.position;
        Vector2 toPlayer = (Vector2)player.position - origin;

        if (toPlayer.magnitude > viewDistance)
            return false;

        Vector2 forward = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
        float angle = Vector2.Angle(forward, toPlayer);

        if (angle > viewAngle * 0.5f)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(origin, toPlayer.normalized, viewDistance, obstacleMask | playerMask);

        if (!hit)
            return false;

        return ((1 << hit.collider.gameObject.layer) & playerMask) != 0;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        float facing = 1f;
        if (Application.isPlaying)
            facing = patrolDirection != 0 ? patrolDirection : Mathf.Sign(transform.localScale.x);
        else
            facing = Mathf.Sign(transform.localScale.x == 0 ? 1 : transform.localScale.x);

        Vector3 origin = transform.position;
        Vector3 forward = new Vector3(facing, 0f, 0f);

        Quaternion leftRot = Quaternion.Euler(0, 0, viewAngle * 0.5f);
        Quaternion rightRot = Quaternion.Euler(0, 0, -viewAngle * 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, origin + (leftRot * forward) * viewDistance);
        Gizmos.DrawLine(origin, origin + (rightRot * forward) * viewDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftBound, transform.position.y - 0.3f, 0f), new Vector3(leftBound, transform.position.y + 0.3f, 0f));
        Gizmos.DrawLine(new Vector3(rightBound, transform.position.y - 0.3f, 0f), new Vector3(rightBound, transform.position.y + 0.3f, 0f));
    }
#endif
}