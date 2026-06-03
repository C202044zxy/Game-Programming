using UnityEngine;

/// <summary>
/// Builds simple procedural sprites at runtime so the game needs no imported
/// art assets yet. All sprites are authored at 1 world unit (pixelsPerUnit ==
/// texture size) so callers size them purely through transform scale.
/// </summary>
public static class RuntimeSprites
{
    // Solid filled circle with a one-pixel soft edge for clean anti-aliasing.
    public static Sprite Circle(int size, Color fill)
    {
        var tex = NewTexture(size);
        float c = (size - 1) * 0.5f;
        float r = size * 0.5f;
        var clear = new Color(0, 0, 0, 0);
        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c));
                float edge = r - 1f;
                if (d <= edge)
                    pixels[y * size + x] = fill;
                else if (d <= r)
                    pixels[y * size + x] = new Color(fill.r, fill.g, fill.b,
                                                     fill.a * Mathf.Clamp01(1f - (d - edge)));
                else
                    pixels[y * size + x] = clear;
            }
        }
        return Finish(tex, pixels, size, "CircleTex");
    }

    // Bright core fading to transparent at the rim - reads as a soft glow.
    public static Sprite Glow(int size, Color core)
    {
        var tex = NewTexture(size);
        float c = (size - 1) * 0.5f;
        float r = size * 0.5f;
        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / r; // 0 centre .. 1 rim
                float a = Mathf.Clamp01(1f - d);
                a *= a; // concentrate brightness toward the core
                pixels[y * size + x] = new Color(core.r, core.g, core.b, core.a * a);
            }
        }
        return Finish(tex, pixels, size, "GlowTex");
    }

    static Texture2D NewTexture(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }

    static Sprite Finish(Texture2D tex, Color[] pixels, int size, string name)
    {
        tex.SetPixels(pixels);
        tex.Apply();
        tex.name = name;
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
