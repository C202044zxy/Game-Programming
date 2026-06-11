using UnityEngine;

/// <summary>
/// A "Blessing of the Fish" collectible. Like a <see cref="Pearl"/> it builds its
/// own glowing visuals and trigger collider, but instead of counting toward the
/// score it grants the player a temporary power via <see cref="BlessingEffects"/>
/// and then destroys itself. Its colour is chosen from <see cref="type"/>: cyan
/// for speed, violet for disguise.
/// </summary>
public class Blessing : MonoBehaviour
{
    public BlessingType type = BlessingType.Speed;
    public float duration = 6f;     // seconds the granted power lasts
    public float glowRadius = 0.6f; // soft halo extent, world units
    public float coreRadius = 0.18f;// inner orb, world units
    public float pickupRadius = 0.42f; // trigger size, world units

    Transform glow;
    Vector3 glowBaseScale;
    float pulseClock;
    bool collected;

    void Awake()
    {
        Color glowColor = type == BlessingType.Speed
            ? new Color(0.45f, 1f, 1f, 1f)    // cyan = speed
            : new Color(0.72f, 0.45f, 1f, 1f); // violet = disguise

        glow = new GameObject("Glow").transform;
        glow.SetParent(transform, false);
        var glowSr = glow.gameObject.AddComponent<SpriteRenderer>();
        glowSr.sprite = RuntimeSprites.Glow(64, glowColor);
        glowSr.sortingOrder = 3;
        glowBaseScale = new Vector3(glowRadius * 2f, glowRadius * 2f, 1f);
        glow.localScale = glowBaseScale;

        // A bright core sits on top of the halo so the blessing reads as a solid orb.
        var core = new GameObject("Core").transform;
        core.SetParent(transform, false);
        var coreSr = core.gameObject.AddComponent<SpriteRenderer>();
        coreSr.sprite = RuntimeSprites.Circle(32, Color.white);
        coreSr.color = Color.Lerp(glowColor, Color.white, 0.5f);
        coreSr.sortingOrder = 4;
        core.localScale = new Vector3(coreRadius * 2f, coreRadius * 2f, 1f);

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = pickupRadius;
    }

    void Update()
    {
        // Pulse a touch faster than a pearl so blessings stand out as special.
        pulseClock += Time.deltaTime;
        float pulse = 1f + 0.16f * Mathf.Sin(pulseClock * 4f);
        glow.localScale = glowBaseScale * pulse;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;

        var effects = other.GetComponent<BlessingEffects>();
        if (effects == null) effects = other.gameObject.AddComponent<BlessingEffects>();
        effects.Grant(type, duration);

        Destroy(gameObject);
    }
}
