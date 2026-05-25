using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Swimming")]
    [SerializeField] float swimForce = 12f;
    [SerializeField] float maxSpeed = 6f;

    [Header("Sprite")]
    [SerializeField] SpriteRenderer spriteRenderer;

    Rigidbody2D rb;
    Vector2 inputDir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Gravity is already 0 in Physics2D settings, but enforce it here too
        rb.gravityScale = 0f;
        // Linear drag creates the floaty drift-to-stop feel
        rb.linearDamping = 2.5f;
    }

    void Update()
    {
        inputDir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        FlipSprite();
    }

    void FixedUpdate()
    {
        if (inputDir != Vector2.zero)
            rb.AddForce(inputDir * swimForce, ForceMode2D.Force);

        // Clamp speed so the player cannot accelerate forever
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void FlipSprite()
    {
        if (spriteRenderer == null) return;
        if (inputDir.x > 0) spriteRenderer.flipX = false;
        else if (inputDir.x < 0) spriteRenderer.flipX = true;
    }
}
