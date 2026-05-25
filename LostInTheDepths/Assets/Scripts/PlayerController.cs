using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Swim Tuning")]
    public float thrust = 18f;
    public float topSpeed = 5.5f;
    public float waterDrag = 2.4f;

    Rigidbody2D body;
    Vector2 inputAxis;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
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
    }

    void FixedUpdate()
    {
        if (inputAxis.sqrMagnitude > 0.0001f)
            body.AddForce(inputAxis * thrust, ForceMode2D.Force);

        if (body.velocity.magnitude > topSpeed)
            body.velocity = body.velocity.normalized * topSpeed;
    }
}
