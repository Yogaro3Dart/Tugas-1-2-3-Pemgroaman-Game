using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float sprintCooldown = 3f;
    [SerializeField] private float sprintDuration = 1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // Component references
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Movement states
    private bool isGrounded;
    private bool isSprinting;
    private bool canSprint = true;
    private float sprintTimer;
    private float sprintCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Perbaikan Rigidbody settings
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 3f;
        
        // Pastikan tidak ada yang null
        if (groundCheck == null)
        {
            Debug.LogError("Ground Check belum di-assign di Inspector!");
            GameObject checkObj = new GameObject("GroundCheck");
            checkObj.transform.parent = this.transform;
            checkObj.transform.localPosition = new Vector3(0, -0.95f, 0);
            groundCheck = checkObj.transform;
        }
    }

    private void FixedUpdate()
    {
        // Pindahkan physics movement ke FixedUpdate
        float moveInput = Input.GetAxisRaw("Horizontal");
        
        // Movement
        float currentSpeed = moveSpeed;
        if (isSprinting)
            currentSpeed *= sprintMultiplier;

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    private void Update()
    {
        // Ground check dengan visual debugging
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckRadius, isGrounded ? Color.green : Color.red);
        
        // Handle input
        float moveInput = Input.GetAxisRaw("Horizontal");
        
        // Jump
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (animator) animator.SetTrigger("Jump");
        }

        // Sprint
        HandleSprint();

        // Update sprite direction
        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;

        // Update animations
        if (animator)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsSprinting", isSprinting);
        }
    }

    private void HandleSprint()
    {
        // Sprinting mechanics
        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;
            if (sprintTimer <= 0)
            {
                isSprinting = false;
                canSprint = false;
                sprintCooldownTimer = sprintCooldown;
            }
        }

        if (!canSprint)
        {
            sprintCooldownTimer -= Time.deltaTime;
            if (sprintCooldownTimer <= 0)
                canSprint = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canSprint && isGrounded)
        {
            isSprinting = true;
            sprintTimer = sprintDuration;
            if (animator) animator.SetTrigger("Sprint");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
    
    // Helper untuk debugging
    private void OnGUI()
    {
        GUILayout.Label("Grounded: " + isGrounded);
        GUILayout.Label("Velocity: " + rb.linearVelocity);
    }
}