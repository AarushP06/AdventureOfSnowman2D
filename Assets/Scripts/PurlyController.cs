using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PurlyController : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float rotateSpeed = 220f;

    public float minX = -5.9f;
    public float maxX = 6.6f;
    public float minY = -3.6f;
    public float maxY = 1.35f;

    public Transform visual;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float rotateInput;
    private float visualYRotation = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h = 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v = -1f;

        moveInput = new Vector2(h, v).normalized;

        rotateInput = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateInput = 1f;
        if (Input.GetKey(KeyCode.E)) rotateInput = -1f;
    }

    void FixedUpdate()
    {
        Vector2 nextPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);

        rb.MovePosition(nextPos);
    }

    void LateUpdate()
    {
        if (visual == null) return;

        if (rotateInput != 0f)
        {
            visualYRotation += rotateInput * rotateSpeed * Time.deltaTime;
        }

        if (moveInput.sqrMagnitude > 0.001f)
        {
            visualYRotation += rotateSpeed * Time.deltaTime;
        }

        visual.localRotation = Quaternion.Euler(0f, visualYRotation, 0f);
    }
}