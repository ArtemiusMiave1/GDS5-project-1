using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("Room Grid Positions")]
    public Vector2Int currentTile;     // Bottom-right tile of current room
    public Vector2Int destinationTile; // Bottom-right tile of target room

    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    private Vector3 targetPos;

    void Start()
    {
        // Start at currentTile position
        targetPos = ShipGridManager.Instance.GridToWorld(currentTile);
        transform.position = targetPos;
    }

    void Update()
    {
        // Move toward target position
        //GridLayout.
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // When reached, step to next tile
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            StepTowardsDestination();
        }
    }

    void StepTowardsDestination()
    {
        if (currentTile == destinationTile)
            return;

        Vector2Int direction = destinationTile - currentTile;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            direction = new Vector2Int(direction.x > 0 ? 2 : -2, 0);
        else
            direction = new Vector2Int(0, direction.y > 0 ? 2 : -2);

        Vector2Int nextTile = currentTile + direction;

        // 🚨 NEW: Check if room exists
        if (!ShipGrid.Instance.HasRoom(nextTile))
            return;

        currentTile = nextTile;
        targetPos = ShipGridManager.Instance.GridToWorld(currentTile);
    }
    bool IsBlocked(Vector2Int from, Vector2Int to)
    {
        Door[] doors = FindObjectsOfType<Door>();

        foreach (Door door in doors)
        {
            if (door.gridPosition == to && !door.isOpen)
                return true;
        }

        return false;
    }
}