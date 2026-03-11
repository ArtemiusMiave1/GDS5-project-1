using UnityEngine;

public class ShipGridManager : MonoBehaviour
{
    public static ShipGridManager Instance;

    public int roomSize = 2; // Each room is 2x2 tiles

    void Awake()
    {
        Instance = this;
    }

    // Converts grid/world tile to world position (here we just return the same since we use world coordinates)
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x, gridPos.y, 0);
    }
}