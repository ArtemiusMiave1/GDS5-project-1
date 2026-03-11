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
            return; // Already at destination

        Vector2Int direction = destinationTile - currentTile;

        // Move 1 room at a time (2 units per step)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            direction = new Vector2Int(direction.x > 0 ? 2 : -2, 0);
        else
            direction = new Vector2Int(0, direction.y > 0 ? 2 : -2);

        currentTile += direction;
        targetPos = ShipGridManager.Instance.GridToWorld(currentTile);
    }
}