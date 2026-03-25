using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCMovement : MonoBehaviour
{
    [Header("Targets")]
    public List<Transform> targetObjects = new List<Transform>(); // list of targets
    private int targetIndex = 0;

    [Header("Movement")]
    public float moveSpeed = 3f;

    public Vector2Int currentTile;
    private Vector3 targetPos;

    private List<Room> currentPath = new List<Room>();
    private int pathIndex = 0;

    void Start()
    {
        StartCoroutine(InitializeMovement());
    }

    IEnumerator InitializeMovement()
    {
        yield return null; // wait a frame for rooms/doors to register

        UpdateCurrentTile();
        targetPos = ShipGridManager.Instance.GridToWorld(currentTile);
        transform.position = targetPos;

        if (targetObjects.Count > 0)
            SetTarget(targetObjects[targetIndex]);
    }

    void Update()
    {
        // Update current tile
        UpdateCurrentTile();

        // Get current room
        Room currentRoom = ShipGrid.Instance.GetRoom(currentTile);

        // Check if current room is flooded
        if (currentRoom != null && currentRoom.isFlooded)
        {
            Die();
            return;
        }

        // ------------------- Check if NPC shares room with any enemy -------------------
        EnemyRandomWalker[] enemies = FindObjectsOfType<EnemyRandomWalker>();
        foreach (EnemyRandomWalker enemy in enemies)
        {
            Room enemyRoom = enemy.currentRoom;
            if (currentRoom != null && currentRoom == enemyRoom)
            {
                Debug.Log($"{gameObject.name} killed by enemy in room {currentRoom.gridPosition}!");
                Die();
                return;
            }
        }
        // -------------------------------------------------------------------------------

        if (currentPath == null || currentPath.Count == 0)
            return;

        // Move toward the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            MoveAlongPath();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died in a flooded room!");
        Destroy(gameObject);
    }

    void UpdateCurrentTile()
    {
        currentTile = ShipGridManager.Instance.WorldToGrid(transform.position);
    }

    void SetTarget(Transform newTarget)
    {
        if (newTarget == null) return;

        Vector2Int targetTile = ShipGridManager.Instance.WorldToGrid(newTarget.position);
        Room startRoom = ShipGrid.Instance.GetRoom(currentTile);
        Room targetRoom = ShipGrid.Instance.GetRoom(targetTile);

        if (startRoom == null || targetRoom == null)
        {
            Debug.LogWarning("NPC A*: Start or target room null!");
            currentPath.Clear();
            return;
        }

        currentPath = FindPath(startRoom, targetRoom);
        pathIndex = 0;

        if (currentPath.Count > 0)
            MoveToRoom(currentPath[0]);
    }

    void MoveAlongPath()
    {
        pathIndex++;

        if (pathIndex >= currentPath.Count)
        {
            // Reached target → pick next target
            targetIndex = (targetIndex + 1) % targetObjects.Count;
            SetTarget(targetObjects[targetIndex]);
            return;
        }

        MoveToRoom(currentPath[pathIndex]);
    }

    void MoveToRoom(Room room)
    {
        currentTile = room.gridPosition;
        targetPos = ShipGridManager.Instance.GridToWorld(currentTile);
    }

    // ---------------- A* Pathfinding ----------------
    public void OnDoorStateChanged()
    {
        // Recalculate path when doors open/close
        CalculatePath();
    }

    public void CalculatePath()
    {
        if (targetObjects.Count == 0) return;

        Room startRoom = ShipGrid.Instance.GetRoom(currentTile);
        Room targetRoom = ShipGrid.Instance.GetRoom(ShipGridManager.Instance.WorldToGrid(targetObjects[targetIndex].position));

        if (startRoom == null || targetRoom == null)
        {
            Debug.LogWarning("NPC A*: Start or Target room is null!");
            currentPath.Clear();
            return;
        }

        currentPath = FindPath(startRoom, targetRoom);
        pathIndex = 0;

        if (currentPath.Count > 0)
            MoveToRoom(currentPath[0]);
    }

    // ---------------- A* Functions ----------------
    List<Room> FindPath(Room start, Room goal)
    {
        List<Room> openList = new List<Room> { start };
        HashSet<Room> closedList = new HashSet<Room>();
        Dictionary<Room, Room> cameFrom = new Dictionary<Room, Room>();
        Dictionary<Room, float> gScore = new Dictionary<Room, float> { [start] = 0f };
        Dictionary<Room, float> fScore = new Dictionary<Room, float> { [start] = Heuristic(start, goal) };

        while (openList.Count > 0)
        {
            Room current = GetLowestFScore(openList, fScore);

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openList.Remove(current);
            closedList.Add(current);

            foreach (RoomConnection conn in current.connections)
            {
                if (!conn.door.isOpen)
                    continue;

                Room neighbor = conn.targetRoom;
                if (closedList.Contains(neighbor))
                    continue;

                float tentativeG = gScore[current] + 1f;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return new List<Room>();
    }

    float Heuristic(Room a, Room b)
    {
        return Mathf.Abs(a.gridPosition.x - b.gridPosition.x) + Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
    }

    Room GetLowestFScore(List<Room> list, Dictionary<Room, float> fScore)
    {
        Room best = list[0];
        float bestScore = fScore.GetValueOrDefault(best, Mathf.Infinity);

        foreach (Room room in list)
        {
            float score = fScore.GetValueOrDefault(room, Mathf.Infinity);
            if (score < bestScore)
            {
                best = room;
                bestScore = score;
            }
        }

        return best;
    }

    List<Room> ReconstructPath(Dictionary<Room, Room> cameFrom, Room current)
    {
        List<Room> path = new List<Room> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
}