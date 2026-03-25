using UnityEngine;
using System.Collections.Generic;

public class RoomFloodFill : MonoBehaviour
{
    [Header("Settings")]
    public Transform originObject;

    [Header("Debug")]
    public bool runFlood;

    private HashSet<Room> floodedRooms = new HashSet<Room>();

    void Update()
    {
        if (runFlood)
        {
            runFlood = false;
            RunFlood();
        }
    }

    public void RunFlood()
    {
        // 🔄 Reset all rooms
        foreach (Room room in FindObjectsOfType<Room>())
        {
            room.SetFlooded(false);
        }

        floodedRooms.Clear();

        Room startRoom = FindRoomAtPosition(originObject.position);

        if (startRoom == null)
        {
            Debug.LogError("Flood: No starting room found!");
            return;
        }

        Queue<Room> queue = new Queue<Room>();

        queue.Enqueue(startRoom);
        floodedRooms.Add(startRoom);
        startRoom.SetFlooded(true);

        while (queue.Count > 0)
        {
            Room current = queue.Dequeue();

            foreach (RoomConnection conn in current.connections)
            {
                // 🚪 Only spread through OPEN doors
                if (!conn.door.isOpen)
                    continue;

                Room neighbor = conn.targetRoom;

                if (!floodedRooms.Contains(neighbor))
                {
                    floodedRooms.Add(neighbor);
                    neighbor.SetFlooded(true);
                    queue.Enqueue(neighbor);
                }
            }
        }

        Debug.Log("Flooded rooms count: " + floodedRooms.Count);
    }

    // 🔍 Find room at world position
    Room FindRoomAtPosition(Vector3 worldPos)
    {
        foreach (Room room in FindObjectsOfType<Room>())
        {
            if (room.tilemap == null) continue;

            Vector3Int cell = room.tilemap.WorldToCell(worldPos);

            if (room.tilemap.HasTile(cell))
                return room;
        }

        return null;
    }
}