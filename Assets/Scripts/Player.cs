using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Health and Movement stats
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    // Ground Check variables
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Image healthImage;
    
    // Dash variables
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public int extraDashValue = 1;
    private bool isDashing = false;
    private int extraDashes;

    // Components and State variables
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
  
    // Extra Jumps (Double Jump)
    public int extraJumpsValue = 1;
    private int extraJumps;
    
    // Wall Slide variables
    public Transform wallCheck;
    public float wallCheckRadius = 0.2f;
    public float wallSlideSpeed = 2f;
    public LayerMask wallLayer; 
    private bool isTouchingWall;
    
    // Coyote Time variables
    public float coyoteTime = 0.1f; 
    private float coyoteTimeCounter;

    //is respawning tag
     private bool isRespawning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        extraJumps = extraJumpsValue;
        extraDashes = extraDashValue;
    }

    void Update()
    {
        if (isRespawning)
        {
            isRespawning = false; // Reset the flag for the next frame
            return;               // Skip the rest of the Update loop
        }

        if (isDashing)
        {
            return;
        }
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        CheckIfWallSliding();
        
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
            extraDashes = extraDashValue;
        }

        float moveInput = Input.GetAxis("Horizontal");
        
        if (isTouchingWall && !isGrounded)
        {
            rb.linearVelocity = new Vector2(0f, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        
        animator.SetBool("if_jump", false);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (coyoteTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animator.SetBool("if_jump", true);
                coyoteTimeCounter = 0f;
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animator.SetBool("if_jump", true);
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
            transform.localScale = new Vector3(4, 4, 4);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-4, 4, 4);
        }

        SetAnimation(moveInput);
        healthImage.fillAmount = health / 100f;
    }

    private void CheckIfWallSliding()
    {
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }
    
    private IEnumerator Dash()
    {
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        
        float dashDirection = transform.localScale.x;

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private void FixedUpdate() { }

    // --- MODIFIED: This function is now more robust and explicitly separates ground/air logic ---
    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            // --- GROUNDED LOGIC ---
            // When we are on the ground, we are guaranteed to not be falling.
            animator.SetBool("if_falling", false);
            // We are running if there is horizontal input.
            animator.SetBool("is_running", moveInput != 0);
        }
        else
        {
            // --- AIRBORNE LOGIC ---
            // When we are in the air, we are not running on the ground.
            animator.SetBool("is_running", false);

            // We are "falling" if our vertical velocity is negative.
            // Using a small threshold like -0.1f is often more reliable than 0.
            if (rb.linearVelocity.y < -0.1f)
            {
                animator.SetBool("if_falling", true);
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
               GameManager.Instance.RespawnPlayer(this);
            }
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
    
    public void Respawn()
    {
        isRespawning = true;
        health = 100;
        healthImage.fillAmount = health / 100f;
    }
}