using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodManager : MonoBehaviour
{
    public static FloodManager Instance;

    [Header("Flood Settings")]
    public Color floodColor = new Color(0.1f, 0.3f, 1f); // dark blue
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f); // light grey

    private HashSet<Room> floodedRooms = new HashSet<Room>();
    private Room originRoom;

    void Awake()
    {
        // Setup singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    IEnumerator Start()
    {
        yield return null; // wait a frame so rooms register
        InitializeOrigin();
        UpdateFlood();
    }

    void InitializeOrigin()
    {
        Vector2Int originTile = ShipGridManager.Instance.WorldToGrid(transform.position);
        originRoom = ShipGrid.Instance.GetRoom(originTile);

        if (originRoom == null)
            Debug.LogError("FloodManager: Could not find origin room!");
    }

    // Call whenever a door opens
    public bool floodStopped = false; // can be set by trigger

    public void UpdateFlood()
    {
        // Don't flood if the stop trigger is active
        if (originRoom == null || floodStopped)
            return;

        Queue<Room> toFlood = new Queue<Room>();
        HashSet<Room> visited = new HashSet<Room>();

        toFlood.Enqueue(originRoom);
        visited.Add(originRoom);

        while (toFlood.Count > 0)
        {
            Room current = toFlood.Dequeue();

            // Flood the room permanently
            if (!current.isFlooded)
            {
                current.isFlooded = true;
                current.UpdateRoomVisual();
                floodedRooms.Add(current);
            }

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
    }

    // Optional: stop all flooding (for reset, debug, etc.)
    public void StopAllFlooding()
    {
        foreach (Room room in floodedRooms)
        {
            room.isFlooded = false;
            room.UpdateRoomVisual();
        }

        floodedRooms.Clear();
        Debug.Log("FloodManager: All flooding stopped.");
    }
}