using UnityEngine;

[System.Serializable]
public class RoomConnection
{
    public Room targetRoom;
    public Door door;
    public Vector2Int direction;
}