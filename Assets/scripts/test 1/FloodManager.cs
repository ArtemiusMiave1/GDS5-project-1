using System.Collections.Generic;
using UnityEngine;

public class FloodManager : MonoBehaviour
{
    [Header("Flood Settings")]
    public Color floodColor = Color.cyan;
    public Color normalColor = Color.white;

    // Keep track of currently flooded rooms
    private HashSet<Room> floodedRooms = new HashSet<Room>();

    private Room originRoom;

    void Start()
    {
        // Determine which room the FloodManager object is in
        Vector2Int originTile = ShipGridManager.Instance.WorldToGrid(transform.position);
        originRoom = ShipGrid.Instance.GetRoom(originTile);

        if (originRoom == null)
        {
            Debug.LogWarning("FloodManager: The object is not inside a room!");
        }

        // Initial flood
        UpdateFlood();
    }

    // Call this whenever a door opens or closes
    public void UpdateFlood()
    {
        if (originRoom == null)
            return;

        Queue<Room> toFlood = new Queue<Room>();
        HashSet<Room> visited = new HashSet<Room>();

        toFlood.Enqueue(originRoom);
        visited.Add(originRoom);

        while (toFlood.Count > 0)
        {
            Room current = toFlood.Dequeue();

            // Flood it
            current.isFlooded = true;
            if (current.tilemap != null)
                current.tilemap.color = floodColor;

            // Spread to open-door neighbors
            foreach (RoomConnection conn in current.connections)
            {
                Room neighbor = conn.targetRoom;
                if (neighbor != null && !visited.Contains(neighbor) && conn.door != null && conn.door.isOpen)
                {
                    toFlood.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        // Unflood rooms that are no longer reachable
        foreach (Room room in floodedRooms)
        {
            if (!visited.Contains(room))
            {
                room.isFlooded = false;
                if (room.tilemap != null)
                    room.tilemap.color = normalColor;
            }
        }

        // Update floodedRooms set
        floodedRooms = visited;
    }
}