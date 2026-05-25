using UnityEngine;

public class CaveBuilder : MonoBehaviour
{
    [Header("Visuals")]
    public Color wallColor = new Color(0.18f, 0.24f, 0.32f, 1f);
    public Color waterColor = new Color(0.04f, 0.07f, 0.13f, 1f);

    [Header("Origin")]
    public Vector2 originOffset = Vector2.zero;

    public Vector2Int Size { get; private set; }
    public Vector2 SpawnPoint { get; private set; }

    static readonly string[] Layout =
    {
        "########################################",
        "#......................................#",
        "#.####.####.####.####.####.####.####...#",
        "#.#..#.#..#.#..#.#..#.#..#.#..#.#......#",
        "#.#..#.#..#.#..#.#..#.#..#.#..#.#......#",
        "#.####.####.####.####.####.####.####...#",
        "#......................................#",
        "#...####################################",
        "#...#..................................#",
        "#...#..####################...####.....#",
        "#...#.....................#...#..#.....#",
        "#...####################..#...#..#.....#",
        "#......................#..#...#..#.....#",
        "#.######...#############..#...####.....#",
        "#......#...............#..#............#",
        "#......#...####.####...#..#............#",
        "#......#...#..#.#..#...#..#............#",
        "#......#...#..#.#..#...#..#............#",
        "#......#...####.####...#..#............#",
        "#......#...............#..#............#",
        "#......#################..#............#",
        "#.........................#............#",
        "########################################",
    };

    Sprite cellSprite;

    void Awake()
    {
        Size = new Vector2Int(Layout[0].Length, Layout.Length);
        cellSprite = BuildUnitSprite();
        BuildBackground();
        BuildWalls();
        SpawnPoint = FindFirstOpenCell();
    }

    void BuildBackground()
    {
        var bg = new GameObject("CaveBackground");
        bg.transform.SetParent(transform, false);
        float w = Size.x;
        float h = Size.y;
        bg.transform.localPosition = new Vector3(originOffset.x + w * 0.5f - 0.5f,
                                                  originOffset.y + h * 0.5f - 0.5f,
                                                  1f);
        bg.transform.localScale = new Vector3(w, h, 1f);
        var sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = cellSprite;
        sr.color = waterColor;
        sr.sortingOrder = -10;
    }

    void BuildWalls()
    {
        var walls = new GameObject("Walls");
        walls.transform.SetParent(transform, false);

        int rows = Size.y;
        int cols = Size.x;

        for (int row = 0; row < rows; row++)
        {
            string line = Layout[row];
            for (int col = 0; col < cols; col++)
            {
                if (col >= line.Length) break;
                if (line[col] != '#') continue;

                // YAML rows top-to-bottom -> world Y bottom-to-top
                int worldY = rows - 1 - row;
                var cell = new GameObject($"Wall_{col}_{worldY}");
                cell.transform.SetParent(walls.transform, false);
                cell.transform.localPosition = new Vector3(originOffset.x + col,
                                                            originOffset.y + worldY,
                                                            0f);

                var sr = cell.AddComponent<SpriteRenderer>();
                sr.sprite = cellSprite;
                sr.color = wallColor;
                sr.sortingOrder = 0;

                var col2d = cell.AddComponent<BoxCollider2D>();
                col2d.size = Vector2.one;
            }
        }
    }

    Vector2 FindFirstOpenCell()
    {
        int rows = Size.y;
        int cols = Size.x;
        for (int row = 0; row < rows; row++)
        {
            string line = Layout[row];
            for (int col = 0; col < cols; col++)
            {
                if (col >= line.Length) continue;
                if (line[col] != '.') continue;
                int worldY = rows - 1 - row;
                return new Vector2(originOffset.x + col, originOffset.y + worldY);
            }
        }
        return new Vector2(originOffset.x + cols * 0.5f, originOffset.y + rows * 0.5f);
    }

    static Sprite BuildUnitSprite()
    {
        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        var pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        tex.name = "CaveCellTex";
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }
}
