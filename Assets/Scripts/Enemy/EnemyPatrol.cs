using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2.5f;
    public float patrolRange = 3f;

    [Header("Retreat")]
    public float retreatSpeed = 5f;
    public float retreatDuration = 1.25f;

    private Rigidbody2D rb;

    private float leftBound;
    private float rightBound;
    private int direction = 1;

    private bool retreating = false;
    private float retreatTimer;
    private Vector2 retreatFromPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float spawnX = transform.position.x;
        leftBound = spawnX - patrolRange;
        rightBound = spawnX + patrolRange;
    }

    void FixedUpdate()
    {
        if (retreating)
        {
            DoRetreat();
            return;
        }

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        if (transform.position.x >= rightBound)
            direction = -1;

        if (transform.position.x <= leftBound)
            direction = 1;

        Face(direction);
    }

    void DoRetreat()
    {
        retreatTimer -= Time.fixedDeltaTime;

        float dx = transform.position.x - retreatFromPoint.x;
        float dir = Mathf.Sign(dx);

        rb.linearVelocity = new Vector2(dir * retreatSpeed, rb.linearVelocity.y);
        Face(dir);

        if (retreatTimer <= 0f)
            retreating = false;
    }

    void Face(float dir)
    {
        if (dir == 0) return;

        Vector3 s = transform.localScale;
        s.x = Mathf.Sign(dir) * Mathf.Abs(s.x);
        transform.localScale = s;
    }

    public void StartRetreat(Vector2 screamOrigin)
    {
        retreatFromPoint = screamOrigin;
        retreatTimer = retreatDuration;
        retreating = true;
    }
	
	public bool IsRetreating()
	{
		return retreating;
	}
}