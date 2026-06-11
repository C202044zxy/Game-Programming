using UnityEngine;

/// <summary>
/// Drives the player fish. Reads the WASD / arrow input each frame and, in
/// <see cref="FixedUpdate"/>, applies a thrust force to the <see cref="Rigidbody2D"/>
/// while capping it at <see cref="topSpeed"/>. Movement feels swim-like because
/// gravity is off and the body uses water drag (set in <see cref="Awake"/>), so
/// the fish coasts to a stop when input is released. The sprite is flipped to
/// face its direction of travel.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Swim Tuning")]
    public float thrust = 18f;
    public float topSpeed = 5.5f;
    public float waterDrag = 2.4f;

    /// <summary>
    /// Scales both thrust and the speed cap. Left at 1 normally; the speed
    /// <see cref="BlessingEffects">blessing</see> raises it for a short burst.
    /// </summary>
    public float SpeedMultiplier { get; set; } = 1f;

    Rigidbody2D body;
    SpriteRenderer sprite;
    Vector2 inputAxis;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        body.gravityScale = 0f;
        body.drag = waterDrag;
        body.angularDrag = 4f;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 raw = new Vector2(h, v);
        inputAxis = raw.sqrMagnitude > 1f ? raw.normalized : raw;

        // The fish art faces right by default; flip it when swimming left.
        if (sprite != null && Mathf.Abs(body.velocity.x) > 0.05f)
            sprite.flipX = body.velocity.x < 0f;
    }

    void FixedUpdate()
    {
        if (inputAxis.sqrMagnitude > 0.0001f)
            body.AddForce(inputAxis * thrust * SpeedMultiplier, ForceMode2D.Force);

        float cap = topSpeed * SpeedMultiplier;
        if (body.velocity.magnitude > cap)
            body.velocity = body.velocity.normalized * cap;
    }
}
