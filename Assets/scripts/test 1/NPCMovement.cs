using UnityEngine;
using System.Collections.Generic;

public class NPCMovement : MonoBehaviour
{
    [Header("Target")]
    public Transform targetObject; // 👈 assign in Inspector

    [Header("Movement")]
    public float moveSpeed = 3f;

    private Vector2Int currentTile;
    private Vector3 targetPos;

    private List<Room> currentPath = new List<Room>();
    private int pathIndex = 0;


    void Update()
    {
        if (currentPath == null || currentPath.Count == 0 || targetPos == null)
        {
            UpdateCurrentTile();

            targetPos = ShipGridManager.Instance.GridToWorld(currentTile);
            transform.position = targetPos;

            CalculatePath();
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            MoveAlongPath();
        }
    }

    // 🔁 Convert NPC position → grid tile
    void UpdateCurrentTile()
    {
        currentTile = ShipGridManager.Instance.WorldToGrid(transform.position);
    }

    // 🔁 Convert target object → grid tile
    Vector2Int GetTargetTile()
    {
        return ShipGridManager.Instance.WorldToGrid(targetObject.position);
    }

    // 🚀 A* PATHFINDING
    void CalculatePath()
    {
        Room startRoom = ShipGrid.Instance.GetRoom(currentTile);
        Room targetRoom = ShipGrid.Instance.GetRoom(GetTargetTile());

        if (startRoom == null)
        {
            Debug.LogError("A*: Start");
            return;
        }
        if (targetRoom == null)
        {
            Debug.LogError("A*: Target room is null!");
            return;
        }

        currentPath = FindPath(startRoom, targetRoom);
        pathIndex = 0;

        if (currentPath.Count > 0)
        {
            MoveToRoom(currentPath[0]);
        }
    }

    void MoveAlongPath()
    {
        pathIndex++;

        if (pathIndex >= currentPath.Count)
            return;

        MoveToRoom(currentPath[pathIndex]);
    }

    void MoveToRoom(Room room)
    {
        currentTile = room.gridPosition;
        targetPos = ShipGridManager.Instance.GridToWorld(currentTile);
    }

    // 🧠 A* IMPLEMENTATION
    List<Room> FindPath(Room start, Room goal)
    {
        List<Room> openSet = new List<Room>();
        HashSet<Room> closedSet = new HashSet<Room>();

        Dictionary<Room, Room> cameFrom = new Dictionary<Room, Room>();

        Dictionary<Room, float> gScore = new Dictionary<Room, float>();
        Dictionary<Room, float> fScore = new Dictionary<Room, float>();

        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            Room current = GetLowestFScore(openSet, fScore);

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (RoomConnection conn in current.connections)
            {
                // 🚪 Skip closed doors
                if (!conn.door.isOpen)
                    continue;

                Room neighbor = conn.targetRoom;

                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeG = gScore[current] + 1;

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeG >= gScore.GetValueOrDefault(neighbor, float.MaxValue))
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;
                fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);
            }
        }

        Debug.LogWarning("A*: No path found!");
        return new List<Room>();
    }

    float Heuristic(Room a, Room b)
    {
        return Mathf.Abs(a.gridPosition.x - b.gridPosition.x) +
               Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
    }

    Room GetLowestFScore(List<Room> list, Dictionary<Room, float> fScore)
    {
        Room best = list[0];
        float bestScore = fScore.GetValueOrDefault(best, float.MaxValue);

        foreach (Room room in list)
        {
            float score = fScore.GetValueOrDefault(room, float.MaxValue);
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