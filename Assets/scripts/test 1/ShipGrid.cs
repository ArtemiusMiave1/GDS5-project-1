using UnityEngine;
using System.Collections.Generic;

public class ShipGrid : MonoBehaviour
{
    public static ShipGrid Instance;

    private Dictionary<Vector2Int, Room> roomLookup = new Dictionary<Vector2Int, Room>();

    void Start()
    {
        PrintRooms();
    }

    void Awake()
    {
        Instance = this;
    }

    public void AddRoom(Room room)
    {
        if (!roomLookup.ContainsKey(room.gridPosition))
        {
            roomLookup.Add(room.gridPosition, room);
        }
    }

    public Room GetRoom(Vector2Int pos)
    {
        roomLookup.TryGetValue(pos, out Room room);
        return room;
    }

    public bool HasRoom(Vector2Int pos)
    {
        return roomLookup.ContainsKey(pos);
    }

    void PrintRooms()
    {
        Debug.Log("---- Current Rooms ----");

        foreach (var entry in roomLookup)
        {
            Room room = entry.Value;

            Debug.Log(
                "Room at: " + entry.Key +
                " | Tile Count: " + room.tiles.Count
            );
        }
    }
    //void OnDrawGizmos()
    //{
    //    if (roomLookup == null) return;

    //    Gizmos.color = Color.green;

    //    foreach (var entry in roomLookup)
    //    {
    //        Vector3 pos = new Vector3(entry.Key.x, entry.Key.y, 0);
    //        Gizmos.DrawWireCube(pos + Vector3.one, Vector3.one);
    //    }
    //}
}