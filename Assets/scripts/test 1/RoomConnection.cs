using UnityEngine;

[System.Serializable]
public class RoomConnection
{
    public Room targetRoom;
    public Door door;
    public Vector2Int direction; // e.g. (1,0), (0,1)
}