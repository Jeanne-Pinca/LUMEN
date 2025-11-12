using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // ... (All your variable declarations are fine) ...
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Image healthImage;
    
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public int extraDashValue = 1;
    private bool isDashing = false;
    private int extraDashes;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
  
    public int extraJumpsValue = 1;
    private int extraJumps;
    public Transform wallCheck;
    public float wallCheckDistance = 0.4f;
    public float wallSlideSpeed = 2f;
    private bool isTouchingWall;

    void Start()
    {
        // <-- FIX: Cleaned up Start() to be more efficient.
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        extraJumps = extraJumpsValue;
        extraDashes = extraDashValue;
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        float moveInput = Input.GetAxis("Horizontal");

        // <-- FIX: The unconditional movement line that was here has been DELETED.

        // State Checks
        CheckIfWallSliding();

        // Ground Resets
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
            extraDashes = extraDashValue;
        }

        // --- FIX: This IF/ELSE block is now the ONLY thing controlling horizontal movement ---
        if (isTouchingWall && !isGrounded)
        {
            // Wall Slide State
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue)); // <-- FIX: Capital 'C' in Clamp
        }
        else
        {
            // Normal Movement State
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        // Handle Inputs (Jump, Dash, Flip)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                extraJumps--;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && extraDashes > 0)
        {
            StartCoroutine(Dash());
            extraDashes--;
        }
        
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Update Animation and UI
        SetAnimation(moveInput);
        healthImage.fillAmount = health / 100f;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // <-- FIX: Using transform.localScale.x is more reliable for dash direction.
        float dashDirection = transform.localScale.x;

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // <-- FIX: This function now ONLY checks for the wall. It no longer touches the physics.
    private void CheckIfWallSliding()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, new Vector2(transform.localScale.x, 0), wallCheckDistance, groundLayer);
    }

    // ... (The rest of your functions: SetAnimation, OnCollisionEnter2D, etc. are all perfectly fine) ...
    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0)
            {
                animator.Play("Player_Idle");
            }
            else
            {
                animator.Play("Player_Run");
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                animator.Play("Player_Jump");
            }
            else
            {
                animator.Play("Player_Fall");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if (health <= 0)
            {
               Die();
            }
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
    
    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}