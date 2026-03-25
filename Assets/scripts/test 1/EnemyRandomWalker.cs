using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyRandomWalker : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;
    public float decisionInterval = 5f;

    public Room currentRoom;
    public Room targetRoom;
    public Vector3 targetPos;
    public bool isMoving = false;


    IEnumerator Start()
    {
        yield return null; // wait 1 frame so rooms register

        // Find starting room
        Vector2Int tilePos = ShipGridManager.Instance.WorldToGrid(transform.position);
        currentRoom = ShipGrid.Instance.GetRoom(tilePos);

        if (currentRoom == null)
        {
            Debug.LogError("EnemyRandomWalker: Could not find starting room!");
            yield return null;
        }

        // Pick a first target immediately
        PickRandomTargetRoom();

        // Start loop to pick new target every interval
        StartCoroutine(RandomMovementLoop());
    }

    void Update()
    {
        if (isMoving && targetRoom != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                currentRoom = targetRoom;
                targetRoom = null;
                isMoving = false;
            }
        }

        // Check flood
        if (currentRoom != null && currentRoom.isFlooded)
        {
            Die();
        }
    }

    IEnumerator RandomMovementLoop()
    {
        while (true)
        {
            if (!isMoving)
            {
                PickRandomTargetRoom();
            }
            yield return new WaitForSeconds(decisionInterval);
        }
    }

    void PickRandomTargetRoom()
    {
        if (currentRoom == null) return;

        List<Room> openNeighbors = new List<Room>();
        foreach (var conn in currentRoom.connections)
        {
            if (conn.door != null && conn.door.isOpen)
                openNeighbors.Add(conn.targetRoom);
        }

        if (openNeighbors.Count > 0)
        {
            targetRoom = openNeighbors[Random.Range(0, openNeighbors.Count)];
            targetPos = ShipGridManager.Instance.GridToWorld(targetRoom.gridPosition);
            isMoving = true;
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died in a flooded room!");
        Destroy(gameObject);
    }
}