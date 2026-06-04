using UnityEngine;

/// <summary>
/// A short-lived particle burst played where a pearl was collected. It builds and
/// configures its own <see cref="ParticleSystem"/> in Awake, emits a single burst,
/// and removes itself when the effect finishes (via <c>ParticleSystemStopAction.Destroy</c>).
/// <see cref="Pearl"/> spawns one of these on its own free-standing GameObject just
/// before it destroys itself, so the sparkle outlives the pearl.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class CollectBurst : MonoBehaviour
{
    public Color color = new Color(0.85f, 0.97f, 1f, 1f);
    public int count = 18;          // particles in the burst
    public float lifetime = 0.6f;   // seconds each particle lives
    public float speed = 2.2f;      // outward launch speed, world units / second
    public float size = 0.22f;      // particle diameter, world units

    static Sprite particleSprite;
    static Material particleMaterial;

    void Awake()
    {
        var ps = GetComponent<ParticleSystem>();
        ps.Stop();

        var main = ps.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = lifetime;
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.4f, speed);
        main.startSize = new ParticleSystem.MinMaxCurve(size * 0.5f, size);
        main.startColor = color;
        main.gravityModifier = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy; // self-clean when finished

        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.05f;

        // Fade alpha to zero over life so the sparkle dissolves cleanly.
        var col = ps.colorOverLifetime;
        col.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
        col.color = grad;

        // Shrink toward the end of life for a softer dissolve.
        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));

        var renderer = GetComponent<ParticleSystemRenderer>();
        renderer.material = ParticleMaterial();
        renderer.sortingOrder = 6; // above the player, pearls and sharks

        ps.Play();
    }

    static Material ParticleMaterial()
    {
        if (particleMaterial != null) return particleMaterial;
        if (particleSprite == null) particleSprite = RuntimeSprites.Glow(32, Color.white);
        particleMaterial = new Material(Shader.Find("Sprites/Default"))
        {
            mainTexture = particleSprite.texture
        };
        return particleMaterial;
    }
}
