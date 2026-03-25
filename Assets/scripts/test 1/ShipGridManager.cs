using UnityEngine;

public class ShipGridManager : MonoBehaviour
{
    public static ShipGridManager Instance;

    public Grid grid; 

    void Awake()
    {
        Instance = this;

        if (grid == null)
        {
            Debug.LogError("Grid is not assigned!");
        }
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        Vector3Int cell = new Vector3Int(gridPos.x, gridPos.y, 0);
        return grid.CellToWorld(cell) + grid.cellSize / 2f;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3Int cell = grid.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }
}