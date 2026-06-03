using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads and caches the imported underwater sprites under Resources/Art and
/// sizes a <see cref="SpriteRenderer"/> to a target world size. The art is CC0
/// (Kenney Fish Pack) plus two authored textures - see CREDITS.md. The
/// procedural <see cref="RuntimeSprites"/> helper is still used for soft glows
/// and as a fallback when a sprite fails to load.
/// </summary>
public static class GameArt
{
    public const string Fish = "fish";
    public const string Shark = "shark";
    public const string Pearl = "pearl";
    public const string Bubble = "bubble";
    public const string Water = "bg_water";
    public const string Rock = "rock";
    public const string Seagrass = "seagrass";

    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public static Sprite Load(string name)
    {
        if (!cache.TryGetValue(name, out var sprite))
        {
            sprite = Resources.Load<Sprite>("Art/" + name);
            cache[name] = sprite;
        }
        return sprite;
    }

    /// <summary>
    /// Assigns the named sprite to <paramref name="sr"/> and scales its transform
    /// uniformly so the sprite's longest side spans <paramref name="worldSize"/>
    /// world units. Returns the sprite, or null if it could not be loaded so the
    /// caller can fall back to a procedural sprite.
    /// </summary>
    public static Sprite Apply(SpriteRenderer sr, string name, float worldSize)
    {
        var sprite = Load(name);
        if (sprite == null) return null;
        sr.sprite = sprite;
        Vector2 size = sprite.bounds.size;
        float longest = Mathf.Max(size.x, size.y);
        float scale = longest > 0.0001f ? worldSize / longest : 1f;
        sr.transform.localScale = new Vector3(scale, scale, 1f);
        return sprite;
    }
}
