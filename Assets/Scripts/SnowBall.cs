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
        // Continuous collision helps prevent fast snowballs from tunneling through targets.
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        // Launch once at spawn and clean up automatically if nothing is hit.
        speed = Mathf.Max(speed, 6.5f);
        rb.linearVelocity = moveDirection * speed;
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        // Re-apply velocity so physics collisions do not slow the projectile down mid-flight.
        rb.linearVelocity = moveDirection * speed;
    }

    public void SetMoveDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return;
        }

        // SnowGun decides the direction before or just after spawn.
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
        // Any impact destroys the snowball, but only the player gets the death effect.
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
