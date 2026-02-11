using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PurlyController : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float rotateSpeed = 220f;

    public float minX = -6.3f;
    public float maxX = 6.3f;
    public float minY = -3.5f;
    public float maxY = 3.5f;

    Rigidbody rb;
    float fixedZ;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        fixedZ = rb.position.z;
    }

    void Update()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h = 1f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v = -1f;

        Vector3 dir = new Vector3(h, v, 0f).normalized;

        Vector3 nextPos = rb.position + dir * moveSpeed * Time.fixedDeltaTime;
        nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
        nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);
        nextPos.z = fixedZ;

        rb.MovePosition(nextPos);

        // Rotate in place (Q/E)
        float rot = 0f;
        if (Input.GetKey(KeyCode.Q)) rot = 2f;
        if (Input.GetKey(KeyCode.E)) rot = -2f;

        if (rot != 0f)
        {
            float yaw = rotateSpeed * rot * Time.deltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yaw, 0f));
        }

        if (dir.sqrMagnitude > 0.001f)
        {
            float yaw = rotateSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yaw, 0f));
        }
    }
}
