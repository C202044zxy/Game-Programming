using UnityEngine;
using UnityEngine.Tilemaps;

// '1' = solid wall, '0' = open water. 22 rows x 40 cols, row 0 = top string → y=21 in scene.
[RequireComponent(typeof(Grid))]
public class CaveLayout : MonoBehaviour
{
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap backgroundTilemap;
    [SerializeField] TileBase wallTile;
    [SerializeField] TileBase waterTile;

    static readonly string[] layout = {
        "1111111111111111111111111111111111111111",
        "1000000000000001100000000000000110000001",
        "1011111111111101100111111111110110111101",
        "1010000000001101100100000000110110100101",
        "1010111110001101100101111100110110101101",
        "1010100010001101100101000100110110101101",
        "1010100010000001100101000100001100101101",
        "1010100010000000000101000100000000101101",
        "1010100010000000000101000100000000101101",
        "1010100010000001100101000100001100101101",
        "1010100010001101100101000100110110101101",
        "1010111110001101100101111100110110101101",
        "1010000000001101100100000000110110100101",
        "1011111111111101100111111111110110111101",
        "1000000000000001100000000000000110000001",
        "1111000111111111111111111111111110001111",
        "1110000100000000000000000000000010000111",
        "1100000100011111111111111111000010000011",
        "1100001100010000000000000010001100000011",
        "1111111111110000000000000011111111111111",
        "1111111111111111111111111111111111111111",
        "1111111111111111111111111111111111111111",
    };

    void Start()
    {
        foreach (var tm in GetComponentsInChildren<Tilemap>())
        {
            if (tm.gameObject.name == "WallTilemap") wallTilemap = tm;
            else if (tm.gameObject.name == "BackgroundTilemap") backgroundTilemap = tm;
        }

        if (wallTilemap == null || backgroundTilemap == null) return;

        if (wallTile == null) wallTile = CreateFlatTile(new Color(0.35f, 0.30f, 0.25f));
        if (waterTile == null) waterTile = CreateFlatTile(new Color(0.15f, 0.45f, 0.80f));

        StampTiles();
    }

    static TileBase CreateFlatTile(Color color)
    {
        var tex = new Texture2D(1, 1) { filterMode = FilterMode.Point };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        var sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.color = color;
        return tile;
    }

    void StampTiles()
    {
        int rows = layout.Length;
        for (int row = 0; row < rows; row++)
        {
            string line = layout[row];
            for (int col = 0; col < line.Length; col++)
            {
                var pos = new Vector3Int(col, rows - 1 - row, 0);
                if (line[col] == '1')
                    wallTilemap.SetTile(pos, wallTile);
                else
                    backgroundTilemap.SetTile(pos, waterTile);
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Preview Layout in Scene")]
    void PreviewInEditor()
    {
        wallTilemap.ClearAllTiles();
        backgroundTilemap.ClearAllTiles();
        StampTiles();
    }
#endif
}
