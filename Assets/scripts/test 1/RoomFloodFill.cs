using System.Collections.Generic;
using UnityEngine;

public static class FloodSystem
{
    // 🔹 Floods rooms starting from a given object
    public static void RoomFloodFill(GameObject startObject, Color floodColor)
    {
        if (startObject == null)
        {
            Debug.LogWarning("Flood: Start object is null!");
            return;
        }

        // Find starting room
        Vector2Int startTile = ShipGridManager.Instance.WorldToGrid(startObject.transform.position);
        Room startRoom = ShipGrid.Instance.GetRoom(startTile);

        if (startRoom == null)
        {
            Debug.LogWarning("Flood: Start object is not inside any room!");
            return;
        }

        Queue<Room> toFlood = new Queue<Room>();
        HashSet<Room> visited = new HashSet<Room>();

        toFlood.Enqueue(startRoom);
        visited.Add(startRoom);

        while (toFlood.Count > 0)
        {
            Room current = toFlood.Dequeue();
            current.isFlooded = true; // Make sure your Room class has this bool

            // Change tilemap color to indicate flood
            if (current.tilemap != null)
            {
                current.tilemap.color = floodColor;
            }

            // Check connected rooms through open doors
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
    }
}