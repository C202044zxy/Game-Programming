using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds the underwater playfield: one large open body of water bounded by an
/// irregular rocky floor, a thin rocky ceiling and solid side walls, with a few
/// reef mounds rising from the floor to swim around. Nothing here is maze-like -
/// the whole water volume is a single connected space. The terrain is generated
/// from a fixed seed so the layout (and the spawn / patrol points derived from
/// it) is reproducible between runs.
/// </summary>
public class CaveBuilder : MonoBehaviour
{
    [Header("Visuals")]
    public Color wallColor = new Color(0.15f, 0.17f, 0.21f, 1f);
    public Color waterColor = new Color(0.04f, 0.07f, 0.13f, 1f);

    [Header("Origin")]
    public Vector2 originOffset = Vector2.zero;

    [Header("Terrain")]
    public int width = 56;
    public int height = 30;
    public int seed = 1337;

    public Vector2Int Size { get; private set; }
    public Vector2 SpawnPoint { get; private set; }

    bool[,] open;       // open[col, worldY] == true -> open water
    Sprite cellSprite;

    void Awake()
    {
        Size = new Vector2Int(width, height);
        cellSprite = BuildUnitSprite();
        GenerateTerrain();
        BuildBackground();
        BuildWalls();
        SpawnPoint = FindSpawnPoint();
    }

    // --- Terrain generation ---------------------------------------------------

    void GenerateTerrain()
    {
        open = new bool[width, height];
        var rng = new System.Random(seed);

        // Rolling rocky floor height (in cells) from layered sine waves.
        var floorTop = new int[width];
        float phase = (float)rng.NextDouble() * 10f;
        for (int col = 0; col < width; col++)
        {
            float n = Mathf.Sin(col * 0.30f + phase) * 1.6f
                    + Mathf.Sin(col * 0.13f + 1.3f) * 1.1f
                    + Mathf.Sin(col * 0.07f + 0.5f) * 1.0f;
            floorTop[col] = Mathf.Clamp(4 + Mathf.RoundToInt(n), 2, 7);
        }

        // A few reef mounds rising from the floor (parabolic humps).
        int mounds = 3;
        for (int m = 0; m < mounds; m++)
        {
            int center = Mathf.RoundToInt(width * (0.22f + 0.28f * m)) + rng.Next(-2, 3);
            int halfW = rng.Next(4, 7);
            int peak = rng.Next(6, 10);
            for (int col = center - halfW; col <= center + halfW; col++)
            {
                if (col < 1 || col >= width - 1) continue;
                float d = (col - center) / (float)halfW; // -1 .. 1
                int add = Mathf.RoundToInt(peak * (1f - d * d));
                floorTop[col] = Mathf.Max(floorTop[col], 4 + add);
            }
        }

        // Thin, slightly uneven rocky ceiling.
        var openTop = new int[width];
        for (int col = 0; col < width; col++)
        {
            int thickness = 1 + Mathf.RoundToInt(0.5f + 0.5f * Mathf.Sin(col * 0.22f));
            openTop[col] = height - thickness;
        }

        for (int col = 1; col < width - 1; col++)
            for (int y = floorTop[col]; y < openTop[col]; y++)
                open[col, y] = true;
    }

    // --- World building -------------------------------------------------------

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

    /// <summary>
    /// Builds the terrain as solid boxes. Each row's run of adjacent wall cells
    /// becomes a single stretched box (and one collider), which keeps the object
    /// count low and reads as a continuous rocky mass.
    /// </summary>
    void BuildWalls()
    {
        var walls = new GameObject("Walls");
        walls.transform.SetParent(transform, false);

        for (int worldY = 0; worldY < Size.y; worldY++)
        {
            int start = -1;
            for (int col = 0; col <= Size.x; col++)
            {
                bool wall = col < Size.x && !open[col, worldY];
                if (wall && start < 0)
                {
                    start = col;
                }
                else if (!wall && start >= 0)
                {
                    int end = col - 1;
                    CreateWallRun(walls.transform, start, end, worldY);
                    start = -1;
                }
            }
        }
    }

    void CreateWallRun(Transform parent, int startCol, int endCol, int worldY)
    {
        int len = endCol - startCol + 1;
        float cx = originOffset.x + (startCol + endCol) * 0.5f;
        float cy = originOffset.y + worldY;

        var cell = new GameObject($"Wall_{startCol}_{worldY}");
        cell.transform.SetParent(parent, false);
        cell.transform.localPosition = new Vector3(cx, cy, 0f);
        cell.transform.localScale = new Vector3(len, 1f, 1f);

        var sr = cell.AddComponent<SpriteRenderer>();
        sr.sprite = cellSprite;
        sr.color = wallColor;
        sr.sortingOrder = 0;

        cell.AddComponent<BoxCollider2D>().size = Vector2.one;
    }

    // --- Queries --------------------------------------------------------------

    /// <summary>World position of the centre of a cell in grid coordinates.</summary>
    public Vector2 CellToWorld(int col, int worldY)
        => new Vector2(originOffset.x + col, originOffset.y + worldY);

    /// <summary>True if the given grid cell is open water (not rock / out of bounds).</summary>
    public bool IsOpen(int col, int worldY)
    {
        if (col < 0 || worldY < 0 || col >= Size.x || worldY >= Size.y) return false;
        return open[col, worldY];
    }

    /// <summary>World positions of every open cell, for scattering collectibles.</summary>
    public List<Vector2> OpenCells()
    {
        var cells = new List<Vector2>();
        for (int worldY = 0; worldY < Size.y; worldY++)
            for (int col = 0; col < Size.x; col++)
                if (IsOpen(col, worldY))
                    cells.Add(CellToWorld(col, worldY));
        return cells;
    }

    /// <summary>
    /// Open cells whose neighbour directly below is rock - i.e. the surface of
    /// the floor and the tops of reef mounds. These are where seabed decorations
    /// (rocks, seagrass) are planted.
    /// </summary>
    public List<Vector2> FloorSurfaceCells()
    {
        var cells = new List<Vector2>();
        for (int col = 1; col < Size.x - 1; col++)
            for (int worldY = 1; worldY < Size.y; worldY++)
                if (IsOpen(col, worldY) && !IsOpen(col, worldY - 1))
                    cells.Add(CellToWorld(col, worldY));
        return cells;
    }

    /// <summary>
    /// Runs of horizontally-adjacent open cells at least <paramref name="minLen"/>
    /// long, returned as (left end, right end) world positions. Used as shark
    /// patrol lanes.
    /// </summary>
    public List<(Vector2 a, Vector2 b)> HorizontalCorridors(int minLen)
    {
        var runs = new List<(Vector2 a, Vector2 b)>();
        for (int worldY = 0; worldY < Size.y; worldY++)
        {
            int start = -1;
            for (int col = 0; col <= Size.x; col++)
            {
                bool isOpen = IsOpen(col, worldY);
                if (isOpen && start < 0)
                {
                    start = col;
                }
                else if (!isOpen && start >= 0)
                {
                    int end = col - 1;
                    if (end - start + 1 >= minLen)
                        runs.Add((CellToWorld(start, worldY), CellToWorld(end, worldY)));
                    start = -1;
                }
            }
        }
        return runs;
    }

    Vector2 FindSpawnPoint()
    {
        // Mid-height open cell near the left edge of the open water.
        for (int col = 1; col < Size.x; col++)
        {
            int lo = -1, hi = -1;
            for (int worldY = 0; worldY < Size.y; worldY++)
            {
                if (IsOpen(col, worldY))
                {
                    if (lo < 0) lo = worldY;
                    hi = worldY;
                }
            }
            if (lo >= 0)
                return CellToWorld(col, (lo + hi) / 2);
        }
        return CellToWorld(Size.x / 2, Size.y / 2);
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
