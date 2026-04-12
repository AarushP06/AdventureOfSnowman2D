using UnityEngine;

public class SnowBall : MonoBehaviour
{
    public float speed = 6.5f;
    public float lifeTime = 6f;

    private Rigidbody2D rb;
    private Collider2D snowballCollider;
    private Vector2 moveDirection = Vector2.right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        snowballCollider = GetComponent<Collider2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        speed = Mathf.Max(speed, 6.5f);
        rb.linearVelocity = moveDirection * speed;
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    public void SetMoveDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return;
        }

        moveDirection = direction.normalized;

        if (rb != null)
        {
            rb.linearVelocity = moveDirection * Mathf.Max(speed, 6.5f);
        }
    }

    public void IgnoreCollisionWith(Collider2D otherCollider)
    {
        if (snowballCollider == null || otherCollider == null)
        {
            return;
        }

        Physics2D.IgnoreCollision(snowballCollider, otherCollider, true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleImpact(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleImpact(other.gameObject);
    }

    void HandleImpact(GameObject hitObject)
    {
        Debug.Log("Snowball hit: " + hitObject.name);

        if (hitObject.CompareTag("Player"))
        {
            Purly_Health purly = hitObject.GetComponent<Purly_Health>();

            if (purly != null)
            {
                purly.Die();
            }
        }

        Destroy(gameObject);
    }
}
