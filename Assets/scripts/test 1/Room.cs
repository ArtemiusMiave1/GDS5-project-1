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
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();

        SetGridPosition();
        GetTiles();
        RegisterRoom();
    }

    void Start()
    {
        UpdateVisual(); // ensure correct color at start
    }

    void SetGridPosition()
    {
        Vector3Int cellPos = tilemap.cellBounds.min;
        gridPosition = new Vector2Int(cellPos.x, cellPos.y);
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