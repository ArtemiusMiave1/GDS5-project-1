using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Room Tilemap")]
    public Tilemap tilemap; // Assign this in the Inspector (one per Room)

    [Header("Room Info")]
    public Vector2Int gridPosition;      // Bottom-left tile of the room in the grid
    public List<Vector3Int> tiles = new List<Vector3Int>();

    void Awake()
    {
        // Make sure we have a Tilemap assigned
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogError("Room requires a Tilemap component!");
                return;
            }
        }

        SetGridPosition(); // Set bottom-left tile
        GetTiles();        // Cache all tiles
        RegisterRoom();    // Add to ShipGrid
    }

    // Use the bottom-left tile of this Tilemap as the room's grid position
    void SetGridPosition()
    {
        BoundsInt bounds = tilemap.cellBounds;

        // Find first tile in bounds (safe in case there are empty tiles)
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                gridPosition = new Vector2Int(pos.x, pos.y);
                break;
            }
        }
    }

    // Cache all tiles that exist in this room
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
}