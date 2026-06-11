using UnityEngine;

/// <summary>The two temporary powers a <see cref="Blessing"/> can grant.</summary>
public enum BlessingType
{
    /// <summary>Raises the fish's top speed and thrust for a short time.</summary>
    Speed,
    /// <summary>Hides the fish from predators so it can't be traced.</summary>
    Disguise,
}

/// <summary>
/// Lives on the player fish and tracks the "Blessing of the Fish" power-ups it has
/// picked up. Each blessing runs on its own countdown so the two can overlap. While
/// active it drives the player's speed multiplier (<see cref="PlayerController"/>)
/// and tints the sprite for feedback — a bright cyan glow for speed and a
/// translucent shimmer for disguise. <see cref="IsDisguised"/> is polled by
/// <see cref="Predator"/> so a disguised fish is neither chased nor killed.
/// </summary>
public class BlessingEffects : MonoBehaviour
{
    [Header("Speed Blessing")]
    public float speedMultiplier = 1.8f;
    public Color speedTint = new Color(0.45f, 1f, 1f, 1f);

    [Header("Disguise Blessing")]
    [Range(0f, 1f)] public float disguiseAlpha = 0.35f;

    float speedTimer;
    float disguiseTimer;

    PlayerController controller;
    SpriteRenderer sprite;
    Color baseColor = Color.white;

    /// <summary>True while the disguise blessing is active — predators ignore the fish.</summary>
    public bool IsDisguised => disguiseTimer > 0f;

    /// <summary>True while the speed blessing is active.</summary>
    public bool SpeedActive => speedTimer > 0f;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) baseColor = sprite.color;
    }

    /// <summary>
    /// Grants a blessing for <paramref name="duration"/> seconds. Re-picking the
    /// same blessing refreshes (never shortens) its remaining time.
    /// </summary>
    public void Grant(BlessingType type, float duration)
    {
        switch (type)
        {
            case BlessingType.Speed: speedTimer = Mathf.Max(speedTimer, duration); break;
            case BlessingType.Disguise: disguiseTimer = Mathf.Max(disguiseTimer, duration); break;
        }
        Apply();
    }

    void Update()
    {
        if (speedTimer <= 0f && disguiseTimer <= 0f) return;

        if (speedTimer > 0f) speedTimer -= Time.deltaTime;
        if (disguiseTimer > 0f) disguiseTimer -= Time.deltaTime;
        Apply();
    }

    /// <summary>Pushes the current blessing state onto the controller and sprite.</summary>
    void Apply()
    {
        if (controller != null)
            controller.SpeedMultiplier = SpeedActive ? speedMultiplier : 1f;

        if (sprite != null)
        {
            Color c = SpeedActive ? speedTint : baseColor;
            if (IsDisguised) c.a = disguiseAlpha;
            sprite.color = c;
        }
    }
}
