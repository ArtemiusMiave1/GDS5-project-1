using UnityEngine;
using System.Collections.Generic;

public static class AStarPathfinding
{
    // Find a path from start room to target room
    public static List<Room> FindPath(Room start, Room target)
    {
        List<Room> openList = new List<Room>();
        HashSet<Room> closedList = new HashSet<Room>();

        Dictionary<Room, Room> cameFrom = new Dictionary<Room, Room>();
        Dictionary<Room, float> gScore = new Dictionary<Room, float>();

        openList.Add(start);
        gScore[start] = 0;

        while (openList.Count > 0)
        {
            // Pick room with lowest fScore = g + heuristic
            Room current = openList[0];
            foreach (Room r in openList)
            {
                float fCurrent = gScore[current] + Heuristic(current, target);
                float fR = gScore.ContainsKey(r) ? gScore[r] + Heuristic(r, target) : Mathf.Infinity;
                if (fR < fCurrent) current = r;
            }

            if (current == target)
                return ReconstructPath(cameFrom, current);

            openList.Remove(current);
            closedList.Add(current);

            foreach (RoomConnection conn in current.connections)
            {
                // Skip blocked doors
                if (!conn.door.isOpen)
                    continue;

                Room neighbor = conn.targetRoom;
                if (closedList.Contains(neighbor))
                    continue;

                float tentativeG = gScore[current] + 1; // distance = 1 per room

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        // No path found
        return new List<Room>();
    }

    private static List<Room> ReconstructPath(Dictionary<Room, Room> cameFrom, Room current)
    {
        List<Room> path = new List<Room> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private static float Heuristic(Room a, Room b)
    {
        // Manhattan distance
        return Mathf.Abs(a.gridPosition.x - b.gridPosition.x) +
               Mathf.Abs(a.gridPosition.y - b.gridPosition.y);
    }
}