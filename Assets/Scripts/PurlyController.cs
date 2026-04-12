using UnityEngine;
using UnityEngine.InputSystem;

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
    public InputActionAsset inputActions;
    public string moveActionName = "Player/Move";
    public string rotateActionName = "Player/Rotate";

    private Rigidbody2D rb;
    private InputAction moveAction;
    private InputAction rotateAction;
    private Vector2 moveInput;
    private float rotateInput;
    private float visualYRotation = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (inputActions != null)
        {
            moveAction = inputActions.FindAction(moveActionName, true);
            rotateAction = inputActions.FindAction(rotateActionName, true);
        }
    }

    void OnEnable()
    {
        moveAction?.Enable();
        rotateAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        rotateAction?.Disable();
    }

    void Update()
    {
        moveInput = moveAction != null ? moveAction.ReadValue<Vector2>().normalized : Vector2.zero;
        rotateInput = rotateAction != null ? rotateAction.ReadValue<float>() : 0f;
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

        visual.localRotation = Quaternion.Euler(0f, visualYRotation, 0f);
    }
}
