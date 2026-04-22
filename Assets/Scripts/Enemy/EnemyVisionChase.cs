using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyVisionChase : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase,
        Retreat,
        Stunned
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

    [Header("Retreat / Stun")]
    public float retreatSpeed = 5.5f;
    public float retreatDuration = 1.25f;
    public float stunDuration = 1.25f;

    [Header("Tuning")]
    public float stickDeadzone = 0.05f;

    private Rigidbody2D rb;
    private float lastSeenTimer;

    private float spawnX;
    private float leftBound;
    private float rightBound;
    private int patrolDirection = 1;

    private float retreatTimer;
    private float stunTimer;
    private Vector2 retreatFromPoint;

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
        if (state == State.Stunned)
        {
            DoStunned();
            return;
        }

        if (state == State.Retreat)
        {
            DoRetreat();
            return;
        }

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

    void DoRetreat()
    {
        retreatTimer -= Time.fixedDeltaTime;

        float dx = transform.position.x - retreatFromPoint.x;
        float dir = 0f;

        if (Mathf.Abs(dx) > stickDeadzone)
            dir = Mathf.Sign(dx);

        rb.linearVelocity = new Vector2(dir * retreatSpeed, rb.linearVelocity.y);
        Face(dir);

        if (retreatTimer <= 0f)
            state = State.Patrol;
    }

    void DoStunned()
    {
        stunTimer -= Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (stunTimer <= 0f)
            state = State.Patrol;
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

    public void StartRetreat(Vector2 screamOrigin)
    {
		Debug.Log(gameObject.name + " entering retreat state");
		
        retreatFromPoint = screamOrigin;
        retreatTimer = retreatDuration;
        state = State.Retreat;
		lastSeenTimer = 999f; // forget the player for now
    }

    public void StartStun()
    {
        stunTimer = stunDuration;
        state = State.Stunned;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
	
	public bool IsDisabledFromAttack()
	{
		return state == State.Retreat || state == State.Stunned;
	}
}