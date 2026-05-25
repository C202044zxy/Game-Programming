using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameBootstrap : MonoBehaviour
{
    [Header("Player Look")]
    public Color playerColor = new Color(1f, 0.78f, 0.32f, 1f);
    public float playerRadius = 0.35f;

    void Start()
    {
        var cave = new GameObject("Cave").AddComponent<CaveBuilder>();
        cave.transform.SetParent(transform, false);

        var player = SpawnPlayer(cave.SpawnPoint);

        var rigGo = new GameObject("CameraRig");
        rigGo.transform.SetParent(transform, false);
        var rig = rigGo.AddComponent<CameraRig>();
        rigGo.transform.position = new Vector3(player.transform.position.x,
                                                player.transform.position.y,
                                                -10f);
        rig.Attach(Camera.main, player.transform);
    }

    GameObject SpawnPlayer(Vector2 spawn)
    {
        var go = new GameObject("Player");
        go.tag = "Player";
        go.transform.position = new Vector3(spawn.x, spawn.y, 0f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = BuildCircleSprite(64, playerColor);
        sr.sortingOrder = 5;
        go.transform.localScale = new Vector3(playerRadius * 2f, playerRadius * 2f, 1f);

        var body = go.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;

        var col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;

        go.AddComponent<PlayerController>();
        return go;
    }

    static Sprite BuildCircleSprite(int size, Color fill)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float cx = (size - 1) * 0.5f;
        float cy = (size - 1) * 0.5f;
        float r = size * 0.5f;
        var clear = new Color(0, 0, 0, 0);
        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float edge = r - 1f;
                if (d <= edge)
                {
                    pixels[y * size + x] = fill;
                }
                else if (d <= r)
                {
                    float t = 1f - (d - edge);
                    pixels[y * size + x] = new Color(fill.r, fill.g, fill.b, Mathf.Clamp01(t));
                }
                else
                {
                    pixels[y * size + x] = clear;
                }
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        tex.name = "PlayerCircleTex";
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
