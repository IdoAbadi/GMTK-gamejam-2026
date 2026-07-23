using UnityEngine;
using UnityEngine.InputSystem;

public class Player_movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public bool allowMouseFollow = false;

    public Rigidbody2D rb; // assign in editor
    float horizontal;
    bool facingRight;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;

    Vector2 mouseTarget;
    bool hasMouseTarget = false;

    void Update()
    {
        // Input System only
        horizontal = 0f;

        // Keyboard
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontal -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal += 1f;
            if (kb.spaceKey.wasPressedThisFrame && IsGrounded())
            {
                if (rb != null)
                {
                    Vector2 lv = rb.linearVelocity;
                    lv.y = jumpForce;
                    rb.linearVelocity = lv;
                }
            }
        }

        // Gamepad left stick
        var gp = Gamepad.current;
        if (gp != null)
        {
            float s = gp.leftStick.x.ReadValue();
            if (Mathf.Abs(s) > 0.2f) horizontal = s;
        }

        // Mouse follow
        var mouse = Mouse.current;
        if (mouse.leftButton.isPressed)
        {
            allowMouseFollow = true;
        }
        else
        {
            allowMouseFollow = false;
            hasMouseTarget = false;
        }
        if (allowMouseFollow && mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            if (Camera.main != null)
            {
                Vector2 sp = mouse.position.ReadValue();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(sp.x, sp.y, 0f));
                mouseTarget = new Vector2(worldPos.x, transform.position.y);
                hasMouseTarget = true;
            }
        }

        // If there's a mouse target and no input, move toward clicked X
        if (hasMouseTarget && Mathf.Abs(horizontal) < 0.2f)
        {
            float dx = mouseTarget.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.1f)
                horizontal = Mathf.Sign(dx);
            else
            {
                hasMouseTarget = false;
                horizontal = 0f;
            }
        }

        // Optional: flip sprite depending on movement direction
        if (horizontal > 0 && !facingRight) Flip();
        else if (horizontal < 0 && facingRight) Flip();
    }

    void FixedUpdate()
    {
        // Apply horizontal movement while preserving vertical (linear) velocity
        if (rb != null)
        {
            Vector2 linearVelocity = rb.linearVelocity; // linear velocity
            linearVelocity.x = horizontal * moveSpeed;
            rb.linearVelocity = linearVelocity;
        }
    }

    bool IsGrounded()
    {
        if (groundCheck != null)
        {
            Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            return hit != null;
        }

        // Fallback raycast down if no groundCheck provided
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
        return hit2.collider != null;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
