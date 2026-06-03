using UnityEngine;

/// <summary>
/// A glowing collectible pearl. Builds its own visuals and trigger collider,
/// registers itself with the <see cref="GameManager"/>, and is collected when
/// the player swims into it.
/// </summary>
public class Pearl : MonoBehaviour
{
    public Color glowColor = new Color(0.80f, 0.95f, 1f, 1f);
    public float glowRadius = 0.55f;   // soft halo extent, world units
    public float pearlSize = 0.5f;     // pearl art diameter, world units
    public float coreRadius = 0.16f;   // procedural fallback core, world units
    public float pickupRadius = 0.40f; // trigger size, world units

    Transform glow;
    Vector3 glowBaseScale;
    float pulseClock;
    bool collected;

    void Awake()
    {
        glow = new GameObject("Glow").transform;
        glow.SetParent(transform, false);
        var glowSr = glow.gameObject.AddComponent<SpriteRenderer>();
        glowSr.sprite = RuntimeSprites.Glow(64, glowColor);
        glowSr.sortingOrder = 3;
        glowBaseScale = new Vector3(glowRadius * 2f, glowRadius * 2f, 1f);
        glow.localScale = glowBaseScale;

        // The pearl art sits on top of the halo.
        var pearl = new GameObject("Pearl").transform;
        pearl.SetParent(transform, false);
        var pearlSr = pearl.gameObject.AddComponent<SpriteRenderer>();
        pearlSr.sortingOrder = 4;
        if (GameArt.Apply(pearlSr, GameArt.Pearl, pearlSize) == null)
        {
            // Fall back to the procedural white core if the pearl art is missing.
            pearlSr.sprite = RuntimeSprites.Circle(32, Color.white);
            pearl.localScale = new Vector3(coreRadius * 2f, coreRadius * 2f, 1f);
        }

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = pickupRadius;
    }

    void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterPearl();
    }

    void Update()
    {
        // Gentle pulse so collectibles read as "alive" against the dark cave.
        pulseClock += Time.deltaTime;
        float pulse = 1f + 0.12f * Mathf.Sin(pulseClock * 3f);
        glow.localScale = glowBaseScale * pulse;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player")) return;

        collected = true;
        if (GameManager.Instance != null)
            GameManager.Instance.CollectPearl();
        // Week 3: spawn particle burst + chime here before destroying.
        Destroy(gameObject);
    }
}
