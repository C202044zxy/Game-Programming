using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float swimForce = 12f;
    [SerializeField] float maxSpeed = 6f;
    [SerializeField] SpriteRenderer spriteRenderer;

    Rigidbody2D rb;
    Vector2 input;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (spriteRenderer == null) return;
        if (input.x > 0) spriteRenderer.flipX = false;
        else if (input.x < 0) spriteRenderer.flipX = true;
    }

    void FixedUpdate()
    {
        if (input != Vector2.zero)
            rb.AddForce(input * swimForce);
        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }
}
