using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    [Header("State")]
    public bool isOpen = false; // start CLOSED

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

        // Ensure collider matches closed state
        if (doorCollider != null)
            doorCollider.enabled = !isOpen;
    }

    // Called when player clicks on the door
    void OnMouseDown()
    {
        ToggleDoor();
    }

    // 🔄 Toggle door open/closed
    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (doorCollider != null)
            doorCollider.enabled = !isOpen;
        // Trigger NPCs to recalc paths
        foreach (NPCMovement npc in FindObjectsOfType<NPCMovement>())
        {
            npc.OnDoorStateChanged();
        }
        UpdateVisual();
    }

    // 🎨 Update sprite color based on state
    void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = isOpen ? Color.blue : Color.red;
    }

    // 🔁 Convert world position → grid position (without snapping)
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

    // 🔍 Detect which rooms this door connects
    void RegisterWithRooms()
    {
        Room[] allRooms = FindObjectsOfType<Room>();
        List<Room> foundRooms = new List<Room>();

        // Offsets to check around the door in world space
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

                if (room.tilemap.HasTile(cell) && !foundRooms.Contains(room))
                {
                    foundRooms.Add(room);
                    Debug.Log($"Door found room at {room.gridPosition}");
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

    // 🔗 Connect rooms for pathfinding
    void ConnectRooms()
    {
        if (roomA == null || roomB == null) return;

        Vector2Int dirAB = roomB.gridPosition - roomA.gridPosition;

        // Normalize direction to -1,0,1
        dirAB = new Vector2Int(
            Mathf.Clamp(dirAB.x, -1, 1),
            Mathf.Clamp(dirAB.y, -1, 1)
        );

        // Room A → B
        roomA.connections.Add(new RoomConnection
        {
            targetRoom = roomB,
            door = this,
            direction = dirAB
        });

        // Room B → A
        roomB.connections.Add(new RoomConnection
        {
            targetRoom = roomA,
            door = this,
            direction = -dirAB
        });

        Debug.Log($"Door connected {roomA.gridPosition} <-> {roomB.gridPosition}");
    }
}