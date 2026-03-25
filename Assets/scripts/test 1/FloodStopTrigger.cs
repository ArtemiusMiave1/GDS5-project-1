using UnityEngine;

public class FloodStopTrigger : MonoBehaviour
{
    [Header("Flood Stop Trigger")]
    public bool floodStopped = false; // Has the flood been stopped?

    void Update()
    {
        if (floodStopped) return;

        // Get all NPCs in the scene
        NPCMovement[] npcs = FindObjectsOfType<NPCMovement>();

        // Get the room this trigger object is in
        Room myRoom = ShipGrid.Instance.GetRoom(ShipGridManager.Instance.WorldToGrid(transform.position));
        if (myRoom == null) return;

        foreach (NPCMovement npc in npcs)
        {
            Room npcRoom = ShipGrid.Instance.GetRoom(npc.currentTile);
            if (npcRoom != null && npcRoom == myRoom)
            {
                StopFlood();
                break;
            }
        }
    }

    void StopFlood()
    {
        FloodManager.Instance.floodStopped = true; // stops flooding
        floodStopped = true;
        Debug.Log("Flood stopped by NPC reaching trigger!");

        // Stop flooding via FloodManager singleton
        if (FloodManager.Instance != null)
        {
            FloodManager.Instance.StopAllFlooding();
        }

        // Optional: Clear flooded state on all rooms
        Room[] allRooms = FindObjectsOfType<Room>();
        foreach (Room room in allRooms)
        {
            room.isFlooded = false;
            room.UpdateRoomVisual(); // Make sure Room has this function
        }
    }
}