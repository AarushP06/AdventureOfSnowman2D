using UnityEngine;

public class SnowBall : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;
    public float lifeTime = 6f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Snowball hit: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            Purly_Health purly = collision.gameObject.GetComponent<Purly_Health>();

            if (purly != null)
            {
                purly.Die();
            }

            Destroy(gameObject);
        }
    }
}