using UnityEngine;

public class Room
{
    public Vector2Int origin; // bottom-right tile
    public Door northDoor;
    public Door southDoor;
    public Door eastDoor;
    public Door westDoor;

    public Room(Vector2Int origin)
    {
        this.origin = origin;
    }
}
