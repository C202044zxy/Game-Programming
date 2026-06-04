using UnityEngine;

/// <summary>
/// The level exit. It spawns dormant — dim and harmless — and watches the
/// <see cref="GameManager"/> each frame. The moment every pearl has been collected
/// it activates: it brightens, begins to swirl and pulse, and plays a one-off
/// portal cue (<see cref="SoundFX.PlayPortalReady"/>). Once active, the player
/// swimming into it ends the run via <see cref="GameManager.WinAndExit"/>, which
/// records the score and time and loads the win scene. <see cref="GameBootstrap"/>
/// places one at the player's spawn so the level forms a there-and-back loop.
/// </summary>
public class Portal : MonoBehaviour
{
    public Color dormantColor = new Color(0.30f, 0.45f, 0.75f, 1f);
    public Color activeColor = new Color(0.55f, 0.95f, 1f, 1f);
    public float radius = 0.9f;    // visual extent, world units
    public float spinSpeed = 40f;  // degrees / second once active

    SpriteRenderer halo;
    SpriteRenderer core;
    bool active;
    bool used;
    float pulseClock;

    void Awake()
    {
        halo = MakeLayer("Halo", 3, radius * 2.2f);
        core = MakeLayer("Core", 4, radius * 1.2f);

        var trigger = gameObject.AddComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = radius * 0.7f;

        ApplyColor(dormantColor, 0.25f);
    }

    SpriteRenderer MakeLayer(string name, int order, float worldDiameter)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.transform.localScale = new Vector3(worldDiameter, worldDiameter, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = RuntimeSprites.Glow(64, Color.white);
        sr.sortingOrder = order;
        return sr;
    }

    void Update()
    {
        if (!active)
        {
            if (GameManager.Instance != null && GameManager.Instance.AllCollected)
                Activate();
            return;
        }

        // Swirl and pulse so the open portal reads as live and inviting.
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
        pulseClock += Time.deltaTime;
        float pulse = 1f + 0.10f * Mathf.Sin(pulseClock * 4f);
        core.transform.localScale = Vector3.one * (radius * 1.2f * pulse);
    }

    /// <summary>The final pearl was collected: light up and announce the exit.</summary>
    void Activate()
    {
        active = true;
        ApplyColor(activeColor, 1f);
        SoundFX.PlayPortalReady();
    }

    void ApplyColor(Color c, float haloAlpha)
    {
        halo.color = new Color(c.r, c.g, c.b, haloAlpha);
        core.color = new Color(c.r, c.g, c.b, Mathf.Min(1f, haloAlpha + 0.35f));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!active || used || !other.CompareTag("Player")) return;
        used = true; // guard against a double trigger before the scene swaps
        if (GameManager.Instance != null)
            GameManager.Instance.WinAndExit();
    }
}
