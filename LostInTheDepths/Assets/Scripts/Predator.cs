using UnityEngine;

/// <summary>
/// A shark predator. It cruises back and forth along a patrol lane, but when the
/// player swims within its detection range it breaks off to lunge straight at
/// them at a higher speed. Once the player escapes (a larger "lose" range, for
/// hysteresis) it swims back to its lane and resumes patrolling. Kills the
/// player on contact via a trigger collider, so it glides over geometry.
/// </summary>
public class Predator : MonoBehaviour
{
    public Vector2 waypointA;
    public Vector2 waypointB;
    public float speed = 1.8f;        // patrol speed, world units / second
    public float chaseMultiplier = 1.9f; // how much faster the lunge is
    public float detectRange = 4.5f;  // start chasing inside this radius
    public float loseRange = 7.5f;    // give up once the player is past this
    public Color bodyColor = new Color(0.42f, 0.48f, 0.53f, 1f);
    public float length = 1.9f;       // visual longest side, world units
    public float radius = 0.5f;       // body collision radius, world units

    enum State { Patrol, Chase, Return }
    State state = State.Patrol;

    float segmentTime; // seconds to travel A -> B at patrol speed
    float clock;
    SpriteRenderer sr;
    Transform player;

    public void Configure(Vector2 a, Vector2 b, float speed)
    {
        waypointA = a;
        waypointB = b;
        this.speed = speed;
    }

    void Awake()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 4;
        if (GameArt.Apply(sr, GameArt.Shark, length) == null)
        {
            // Fall back to the procedural circle if the shark art is missing.
            sr.sprite = RuntimeSprites.Circle(64, bodyColor);
            transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
        }

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        // Keep the world collision radius at `radius` whatever scale the art chose.
        col.radius = radius / Mathf.Max(0.0001f, transform.localScale.x);
    }

    void Start()
    {
        float dist = Vector2.Distance(waypointA, waypointB);
        segmentTime = dist > 0.001f ? dist / Mathf.Max(0.01f, speed) : 1f;
        transform.position = waypointA;

        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null) player = playerGo.transform;
    }

    void Update()
    {
        Vector2 prev = transform.position;
        Vector2 next = prev;

        float toPlayer = player != null
            ? Vector2.Distance(prev, player.position)
            : float.MaxValue;

        switch (state)
        {
            case State.Patrol:
                next = PatrolStep();
                if (toPlayer <= detectRange) state = State.Chase;
                break;

            case State.Chase:
                next = Vector2.MoveTowards(prev, player.position,
                                           speed * chaseMultiplier * Time.deltaTime);
                if (toPlayer > loseRange || player == null) state = State.Return;
                break;

            case State.Return:
                Vector2 home = NearestPointOnLane(prev);
                next = Vector2.MoveTowards(prev, home, speed * Time.deltaTime);
                if (toPlayer <= detectRange)
                {
                    state = State.Chase;
                }
                else if (Vector2.Distance(next, home) < 0.05f)
                {
                    ResyncClock(home);
                    state = State.Patrol;
                }
                break;
        }

        transform.position = new Vector3(next.x, next.y, 0f);

        // The shark art faces right by default; flip it to lead with its head.
        float dx = next.x - prev.x;
        if (sr != null && Mathf.Abs(dx) > 0.0001f)
            sr.flipX = dx < 0f;
    }

    Vector2 PatrolStep()
    {
        clock += Time.deltaTime;
        float t = Mathf.PingPong(clock / segmentTime, 1f);
        return Vector2.Lerp(waypointA, waypointB, t);
    }

    /// <summary>Closest point on the patrol segment to <paramref name="p"/>.</summary>
    Vector2 NearestPointOnLane(Vector2 p)
    {
        Vector2 ab = waypointB - waypointA;
        float u = Mathf.Clamp01(Vector2.Dot(p - waypointA, ab) /
                                Mathf.Max(0.0001f, ab.sqrMagnitude));
        return waypointA + ab * u;
    }

    /// <summary>
    /// Rewinds the patrol clock so PingPong resumes from the lane point the shark
    /// just returned to, giving a seamless hand-off back into the patrol.
    /// </summary>
    void ResyncClock(Vector2 lanePoint)
    {
        Vector2 ab = waypointB - waypointA;
        float u = Mathf.Clamp01(Vector2.Dot(lanePoint - waypointA, ab) /
                                Mathf.Max(0.0001f, ab.sqrMagnitude));
        clock = u * segmentTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
    }
}
