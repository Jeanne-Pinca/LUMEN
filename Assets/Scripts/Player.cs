using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // ... (All other variables are the same)
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
    public float wallCheckRadius = 0.2f; // <-- NEW
    public float wallSlideSpeed = 2f;
    public LayerMask wallLayer; 
    private bool isTouchingWall;
    
    public float coyoteTime = 0.1f; 
    private float coyoteTimeCounter;

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
        if (isDashing)
        {
            return;
        }
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        CheckIfWallSliding(); // Check our state
        
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
            // Wall Slide State
            // <-- MODIFIED: Set X velocity to 0 for a better "stick"
            rb.linearVelocity = new Vector2(0f, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            // Normal Movement State
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (coyoteTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                coyoteTimeCounter = 0f;
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

        SetAnimation(moveInput);
        healthImage.fillAmount = health / 100f;
    }
    
    // <-- MODIFIED: Switched to a more reliable OverlapCircle check
    private void CheckIfWallSliding()
    {
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer);
    }
    
    // ... (The rest of your code from Dash() onwards is perfect and needs no changes) ...
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