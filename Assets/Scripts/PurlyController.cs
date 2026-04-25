using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PurlyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gravityScale = 3f;
    public float groundCheckDistance = 0.1f;
    public float maxFallSpeed = 18f;
    public bool enableFallDeath;
    public float fallDeathY = -12f;

    public Transform visual;
    public InputActionAsset inputActions;
    public string moveActionName = "Player/Move";
    public string jumpActionName = "Player/Jump";
    public string animatorMoveXParameter = "MoveX";
    public string animatorGroundedParameter = "Grounded";

    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private Purly_Health health;
    private Animator animator;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        health = GetComponent<Purly_Health>();
        animator = visual != null ? visual.GetComponent<Animator>() : GetComponentInChildren<Animator>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        if (inputActions != null)
        {
            moveAction = inputActions.FindAction(moveActionName, true);
            jumpAction = inputActions.FindAction(jumpActionName, false);
        }
    }

    void OnEnable()
    {
        moveAction?.Enable();

        if (jumpAction != null)
        {
            jumpAction.Enable();
        }
    }

    void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.Disable();
        }

        moveAction?.Disable();
    }

    void Update()
    {
        moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        if (enableFallDeath && health != null && !health.isDead && transform.position.y < fallDeathY)
        {
            health.Die();
        }
    }

    void FixedUpdate()
    {
        UpdateGroundedState();

        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveInput.x * moveSpeed;
        velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);

        if (isGrounded && jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            velocity.y = jumpForce;
            isGrounded = false;
        }

        rb.linearVelocity = velocity;

        if (animator != null)
        {
            animator.SetFloat(animatorMoveXParameter, moveInput.x);
            animator.SetBool(animatorGroundedParameter, isGrounded);
        }
    }

    void LateUpdate()
    {
        if (visual == null)
        {
            return;
        }

        if (moveInput.x > 0.01f)
        {
            visual.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (moveInput.x < -0.01f)
        {
            visual.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    void UpdateGroundedState()
    {
        if (bodyCollider == null)
        {
            isGrounded = false;
            return;
        }

        Bounds bounds = bodyCollider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 size = new Vector2(bounds.size.x * 0.9f, groundCheckDistance);

        Collider2D[] hits = Physics2D.OverlapBoxAll(origin + Vector2.down * (groundCheckDistance * 0.5f), size, 0f);
        isGrounded = false;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null || hit == bodyCollider || hit.isTrigger)
            {
                continue;
            }

            isGrounded = true;
            break;
        }
    }
}
