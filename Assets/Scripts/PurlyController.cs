using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PurlyController : MonoBehaviour
{
    // These values control Purly's platforming feel.
    public float moveSpeed = 5f;
    public float jumpForce = 10.5f;
    public float gravityScale = 1.75f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.1f;
    public float fallGravityMultiplier = 2.35f;
    public float lowJumpGravityMultiplier = 2.8f;
    public float groundCheckDistance = 0.14f;
    public float groundedFallSnapSpeed = 2f;
    public float groundCheckWidthMultiplier = 0.82f;
    public float maxFallSpeed = 14f;
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
    private bool wasGroundedLastFrame;
    private bool jumpHeld;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

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
        // Read player input every frame, then cache jump timing so physics can use it in FixedUpdate.
        moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        jumpHeld = jumpAction != null && jumpAction.IsPressed();

        if (jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            // Store the jump press briefly so the player can still jump on the next physics step.
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter = Mathf.Max(0f, jumpBufferCounter - Time.deltaTime);
        }

        if (enableFallDeath && health != null && !health.isDead && transform.position.y < fallDeathY)
        {
            health.Die();
        }
    }

    void FixedUpdate()
    {
        UpdateGroundedState();

        // Coyote time allows a jump slightly after leaving a platform.
        if (isGrounded)
        {
            // Reset coyote time while grounded so Purly can still jump a moment after leaving an edge.
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter = Mathf.Max(0f, coyoteTimeCounter - Time.fixedDeltaTime);
        }

        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveInput.x * moveSpeed;

        // Jump only if a recent button press overlaps with a recent grounded state.
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            // This is the actual jump: set upward velocity and consume the buffered jump.
            velocity.y = jumpForce;
            isGrounded = false;
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }
        else if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -groundedFallSnapSpeed;
        }

        velocity = ApplyBetterJumpGravity(velocity);
        velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        rb.linearVelocity = velocity;
        wasGroundedLastFrame = isGrounded;

        if (animator != null)
        {
            // Feed the animator enough information to choose idle, walk, and jump states.
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

        // Flip only the visual object so physics and colliders stay stable.
        if (moveInput.x > 0.01f)
        {
            visual.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (moveInput.x < -0.01f)
        {
            visual.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    Vector2 ApplyBetterJumpGravity(Vector2 velocity)
    {
        float gravityMultiplier = 1f;

        // Falling is heavier than rising, and releasing jump early creates a shorter jump.
        if (velocity.y < 0f)
        {
            // Make falling faster so the jump does not feel floaty.
            gravityMultiplier = fallGravityMultiplier;
        }
        else if (velocity.y > 0f && !jumpHeld)
        {
            // If the jump button is released early, apply extra gravity for a shorter jump.
            gravityMultiplier = lowJumpGravityMultiplier;
        }

        if (gravityMultiplier > 1f)
        {
            float extraGravity = Physics2D.gravity.y * rb.gravityScale * (gravityMultiplier - 1f) * Time.fixedDeltaTime;
            velocity.y += extraGravity;
        }

        return velocity;
    }

    void UpdateGroundedState()
    {
        if (bodyCollider == null)
        {
            isGrounded = false;
            return;
        }

        // Probe a short box just below Purly so only solid floor contacts count as grounded.
        Bounds bounds = bodyCollider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 size = new Vector2(bounds.size.x * groundCheckWidthMultiplier, groundCheckDistance);

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

        // Clear buffered jump after landing so one press does not trigger twice.
        if (!wasGroundedLastFrame && isGrounded)
        {
            jumpBufferCounter = 0f;
        }
    }
}
