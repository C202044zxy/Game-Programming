using UnityEngine;

/// <summary>
/// A patrolling predator fish. Moves back and forth between two waypoints at a
/// constant speed (Lerp driven by a PingPong clock) and kills the player on
/// contact. Uses a trigger collider so it glides over geometry on its set path.
/// </summary>
public class Predator : MonoBehaviour
{
    public Vector2 waypointA;
    public Vector2 waypointB;
    public float speed = 1.8f; // world units per second
    public Color bodyColor = new Color(0.86f, 0.27f, 0.32f, 1f);
    public float radius = 0.42f;

    float segmentTime; // seconds to travel A -> B
    float clock;

    public void Configure(Vector2 a, Vector2 b, float speed)
    {
        waypointA = a;
        waypointB = b;
        this.speed = speed;
    }

    void Awake()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = RuntimeSprites.Circle(64, bodyColor);
        sr.sortingOrder = 4;
        transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f; // local; transform scale makes world radius == radius
    }

    void Start()
    {
        float dist = Vector2.Distance(waypointA, waypointB);
        segmentTime = dist > 0.001f ? dist / Mathf.Max(0.01f, speed) : 1f;
        transform.position = waypointA;
    }

    void Update()
    {
        clock += Time.deltaTime;
        float t = Mathf.PingPong(clock / segmentTime, 1f);
        Vector2 pos = Vector2.Lerp(waypointA, waypointB, t);
        transform.position = new Vector3(pos.x, pos.y, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
    }
}
