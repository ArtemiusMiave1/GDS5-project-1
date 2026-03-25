using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    [Header("State")]
    public bool isOpen = true;

    [Header("Debug")]
    public bool toggleDoorButton; // 👈 click in Inspector

    [Header("References")]
    public Collider2D doorCollider;
    public SpriteRenderer spriteRenderer;

    [Header("Grid Info")]
    public Vector2Int gridPosition;

    [Header("Connected Rooms")]
    public Room roomA;
    public Room roomB;

    void Start()
    {
        // Auto-assign SpriteRenderer if missing
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        SetGridPosition();
        RegisterWithRooms();
        UpdateVisual();
    }

    void Update()
    {
        // Inspector button trigger
        if (toggleDoorButton)
        {
            toggleDoorButton = false;
            ToggleDoor();
        }
    }

    // 🔄 Toggle door open/closed
    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (doorCollider != null)
            doorCollider.enabled = !isOpen;

        UpdateVisual();
    }

    // 🎨 Update color
    void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = isOpen ? Color.blue : Color.red;
    }

    // 🔁 Convert world position → grid position (NO snapping)
    void SetGridPosition()
    {
        if (ShipGridManager.Instance == null || ShipGridManager.Instance.grid == null)
        {
            Debug.LogError("Door: Missing ShipGridManager or Grid!");
            return;
        }

        Vector3Int cell = ShipGridManager.Instance.grid.WorldToCell(transform.position);
        gridPosition = new Vector2Int(cell.x, cell.y);
    }

    // 🔍 Find which rooms this door connects
    void RegisterWithRooms()
    {
        Room[] allRooms = FindObjectsOfType<Room>();

        List<Room> foundRooms = new List<Room>();

        // Small offsets around the door
        Vector3[] offsets = new Vector3[]
        {
        Vector3.left * 0.6f,
        Vector3.right * 0.6f,
        Vector3.up * 0.6f,
        Vector3.down * 0.6f
        };

        foreach (Vector3 offset in offsets)
        {
            Vector3 checkPos = transform.position + offset;

            foreach (Room room in allRooms)
            {
                if (room.tilemap == null) continue;

                Vector3Int cell = room.tilemap.WorldToCell(checkPos);

                if (room.tilemap.HasTile(cell))
                {
                    if (!foundRooms.Contains(room))
                    {
                        foundRooms.Add(room);
                        Debug.Log($"Door found room at {room.gridPosition}");
                    }
                }
            }
        }

        if (foundRooms.Count >= 2)
        {
            roomA = foundRooms[0];
            roomB = foundRooms[1];

            ConnectRooms();
        }
        else
        {
            Debug.LogWarning($"Door found {foundRooms.Count} rooms (expected 2)");
        }
    }

    // 🔗 Create connections between rooms
    void ConnectRooms()
    {
        if (roomA == null || roomB == null) return;

        Vector2Int dirAB = roomB.gridPosition - roomA.gridPosition;

        // Normalize direction (so it's -1, 0, or 1)
        dirAB = new Vector2Int(
            Mathf.Clamp(dirAB.x, -1, 1),
            Mathf.Clamp(dirAB.y, -1, 1)
        );

        // Add connection A → B
        roomA.connections.Add(new RoomConnection
        {
            targetRoom = roomB,
            door = this,
            direction = dirAB
        });

        // Add connection B → A
        roomB.connections.Add(new RoomConnection
        {
            targetRoom = roomA,
            door = this,
            direction = -dirAB
        });

        Debug.Log($"Door connected {roomA.gridPosition} <-> {roomB.gridPosition}");
    }
}