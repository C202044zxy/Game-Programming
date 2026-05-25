using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Defines the cave as a 2D tile map and stamps it onto Unity tilemaps at runtime.
/// '1' = solid wall tile, '0' = open water.
/// Layout is 40 wide x 22 tall. Origin (0,0) is bottom-left.
/// The figure-eight shape: left loop (cols 1-18), central corridor (cols 17-23), right loop (cols 22-39).
/// </summary>
[RequireComponent(typeof(Grid))]
public class CaveLayout : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap backgroundTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase wallTile;
    [SerializeField] TileBase waterTile;

    // 22 rows x 40 cols. Row 0 = bottom.
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
        if (wallTilemap == null || backgroundTilemap == null) return;
        StampTiles();
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
