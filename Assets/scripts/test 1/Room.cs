using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemap;

    [Header("Tiles")]
    public List<Vector3Int> tiles = new List<Vector3Int>();
    public Vector2Int gridPosition;

    [Header("Connections")]
    public List<RoomConnection> connections = new List<RoomConnection>();

    [Header("Flood State")]
    public bool isFlooded = false;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color floodedColor = Color.cyan;

    void Awake()
    {
        //if (tilemap == null)
        //    tilemap = GetComponent<Tilemap>();

        //SetGridPosition();
        //GetTiles();
        //RegisterRoom();
    }

    void Start()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();

        SetGridPosition();
        GetTiles();
        RegisterRoom();
        UpdateVisual(); // ensure correct color at start
    }

    void SetGridPosition()
    {
        if (tilemap == null) return;

        tiles.Clear();

        // Go through all tiles that actually exist
        BoundsInt bounds = tilemap.cellBounds;

        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tiles.Add(pos);

                // Track bottom-left occupied tile
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
            }
        }

        if (tiles.Count > 0)
        {
            gridPosition = new Vector2Int(minX, minY); // bottom-left occupied tile
            Debug.Log($"Room gridPosition set to bottom-left occupied tile: {gridPosition}");
        }
        else
        {
            Debug.LogWarning("Room has no tiles!");
        }
    }

    void GetTiles()
    {
        tiles.Clear();

        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tiles.Add(pos);
            }
        }
    }

    void RegisterRoom()
    {
        if (ShipGrid.Instance != null)
        {
            ShipGrid.Instance.AddRoom(this);
        }
        else
        {
            Debug.LogError("No ShipGrid found in scene!");
        }
    }

    // 🌊 Set flooded state
    public void SetFlooded(bool flooded)
    {
        isFlooded = flooded;
        UpdateVisual();
    }

    // 🎨 Update tile colors
    public void UpdateVisual()
    {
        if (tilemap == null) return;

        Color color = isFlooded ? floodedColor : normalColor;

        foreach (var tile in tiles)
        {
            tilemap.SetColor(tile, color);
        }
    }
}