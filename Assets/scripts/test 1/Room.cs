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

    [Header("Inspector Debug")]
    public bool printRoomInfoButton; // Click this to print

    void Awake()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();

        SetGridPosition();
        GetTiles();
        RegisterRoom();
    }

    void Update()
    {
        if (printRoomInfoButton)
        {
            printRoomInfoButton = false;
            PrintRoomInfo();
        }
    }

    void SetGridPosition()
    {
        if (tilemap == null) return;

        tiles.Clear();

        BoundsInt bounds = tilemap.cellBounds;

        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tiles.Add(pos);

                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
            }
        }

        if (tiles.Count > 0)
        {
            gridPosition = new Vector2Int(minX, minY); // bottom-left occupied tile
        }
    }

    void GetTiles()
    {
        // Already filled in SetGridPosition
        // Keep for compatibility if needed
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

    // 🔹 Print the room information to console
    public void PrintRoomInfo()
    {
        Debug.Log($"--- Room at {gridPosition} ---");
        Debug.Log("Tiles:");

        foreach (var tile in tiles)
        {
            Debug.Log($"  {tile}");
        }

        Debug.Log("Connections:");

        foreach (var conn in connections)
        {
            if (conn.targetRoom != null && conn.door != null)
            {
                Debug.Log($"  To Room: {conn.targetRoom.gridPosition}, Door Open: {conn.door.isOpen}, Direction: {conn.direction}");
            }
        }
    }
}