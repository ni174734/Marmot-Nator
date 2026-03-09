using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2.5f;
    public float patrolRange = 3f;

    private Rigidbody2D rb;

    private float leftBound;
    private float rightBound;
    private int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float spawnX = transform.position.x;

        leftBound = spawnX - patrolRange;
        rightBound = spawnX + patrolRange;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        if (transform.position.x >= rightBound)
            direction = -1;

        if (transform.position.x <= leftBound)
            direction = 1;

        // Flip sprite
        if (direction != 0)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Sign(direction) * Mathf.Abs(s.x);
            transform.localScale = s;
        }
    }
}