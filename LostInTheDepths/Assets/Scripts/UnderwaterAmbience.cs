using UnityEngine;

/// <summary>
/// Cosmetic underwater ambience: a deep-water gradient backdrop that fills the
/// cave bounds, plus a field of bubbles that drift upward, sway gently and loop
/// back to the bottom. Built at runtime by <see cref="GameBootstrap"/>.
/// </summary>
public class UnderwaterAmbience : MonoBehaviour
{
    [Header("Bubbles")]
    public int bubbleCount = 28;
    public Vector2 riseSpeedRange = new Vector2(0.4f, 1.1f); // world units / second
    public Vector2 sizeRange = new Vector2(0.12f, 0.34f);    // world diameter
    public float bubbleAlpha = 0.5f;

    Rect area;            // world-space rectangle the bubbles travel within
    Bubble[] bubbles;

    struct Bubble
    {
        public Transform t;
        public float speed; // rise speed
        public float sway;  // horizontal drift amplitude
        public float phase; // sway phase offset
    }

    /// <summary>
    /// Sizes the backdrop and seeds the bubble field to the cave's grid bounds.
    /// </summary>
    public void Build(Vector2 origin, Vector2Int gridSize)
    {
        // Cells sit at integer grid positions, so the filled area spans half a
        // cell beyond the first and last cell centres.
        area = new Rect(origin.x - 0.5f, origin.y - 0.5f, gridSize.x, gridSize.y);
        BuildBackdrop();
        BuildBubbles();
    }

    void BuildBackdrop()
    {
        var sprite = GameArt.Load(GameArt.Water);
        if (sprite == null) return;

        var bg = new GameObject("WaterBackdrop");
        bg.transform.SetParent(transform, false);
        bg.transform.position = new Vector3(area.center.x, area.center.y, 2f);

        var sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = -9; // above the cave's flat fill (-10), behind walls (0)

        Vector2 s = sprite.bounds.size;
        bg.transform.localScale = new Vector3(area.width / Mathf.Max(0.0001f, s.x),
                                              area.height / Mathf.Max(0.0001f, s.y),
                                              1f);
    }

    void BuildBubbles()
    {
        var sprite = GameArt.Load(GameArt.Bubble);
        var root = new GameObject("Bubbles").transform;
        root.SetParent(transform, false);

        bubbles = new Bubble[bubbleCount];
        for (int i = 0; i < bubbleCount; i++)
        {
            var go = new GameObject("Bubble");
            go.transform.SetParent(root, false);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 2; // in front of walls, behind pearls and the fish
            sr.color = new Color(1f, 1f, 1f, bubbleAlpha);

            float size = Random.Range(sizeRange.x, sizeRange.y);
            if (sprite != null)
            {
                sr.sprite = sprite;
                float longest = Mathf.Max(sprite.bounds.size.x, sprite.bounds.size.y);
                float scale = longest > 0.0001f ? size / longest : size;
                go.transform.localScale = new Vector3(scale, scale, 1f);
            }

            go.transform.position = RandomPos();
            bubbles[i] = new Bubble
            {
                t = go.transform,
                speed = Random.Range(riseSpeedRange.x, riseSpeedRange.y),
                sway = Random.Range(0.15f, 0.5f),
                phase = Random.Range(0f, Mathf.PI * 2f),
            };
        }
    }

    Vector3 RandomPos()
        => new Vector3(Random.Range(area.xMin, area.xMax),
                       Random.Range(area.yMin, area.yMax), 1.5f);

    void Update()
    {
        if (bubbles == null) return;

        for (int i = 0; i < bubbles.Length; i++)
        {
            var b = bubbles[i];
            if (b.t == null) continue;

            Vector3 p = b.t.position;
            p.y += b.speed * Time.deltaTime;
            p.x += Mathf.Sin(Time.time * 1.5f + b.phase) * b.sway * Time.deltaTime;

            if (p.y > area.yMax) // surfaced - loop back to the bottom
            {
                p.y = area.yMin;
                p.x = Random.Range(area.xMin, area.xMax);
            }
            b.t.position = p;
        }
    }
}
