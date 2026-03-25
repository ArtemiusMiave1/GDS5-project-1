using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    [Header("State")]
    public bool isOpen = false;

    [Header("References")]
    public Collider2D doorCollider;
    public SpriteRenderer spriteRenderer;
    public GameObject outlineObject; // Assign child outline object

    [Header("Grid Info")]
    public Vector2Int gridPosition;

    [Header("Connected Rooms")]
    public Room roomA;
    public Room roomB;

    [Header("Colors")]
    public Color closedColor = new Color(0.8f, 0.8f, 0.8f); // light grey
    public Color openColor = new Color(0.102f, 0.161f, 0.216f); // #1A2937 dark blue

    [Header("Collider Settings")]
    public Vector2 colliderSizeMultiplier = new Vector2(1.2f, 1.2f);

    private bool isHovered = false;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (doorCollider == null)
            doorCollider = GetComponent<Collider2D>();

        SetGridPosition();
        RegisterWithRooms();

        ResizeCollider();

        UpdateVisual();

        if (doorCollider != null)
            doorCollider.enabled = !isOpen;

        if (outlineObject != null)
        {
            outlineObject.SetActive(false);

            // Ensure outline is above door
            SpriteRenderer outlineSR = outlineObject.GetComponent<SpriteRenderer>();
            if (outlineSR != null)
            {
                outlineSR.sortingOrder = 3;
            }
        }
    }

    void Update()
    {
        UpdateOutlineTransform();
    }

    void OnMouseDown()
    {
        ToggleDoor();
    }

    void OnMouseEnter()
    {
        isHovered = true;
        UpdateVisual();
    }

    void OnMouseExit()
    {
        isHovered = false;
        UpdateVisual();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;

        if (doorCollider != null)
            doorCollider.enabled = !isOpen;

        foreach (NPCMovement npc in FindObjectsOfType<NPCMovement>())
        {
            
            npc.OnDoorStateChanged();
        }

        FindObjectOfType<FloodManager>()?.UpdateFlood();

        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = isOpen ? openColor : closedColor;

        if (outlineObject != null)
        {
            outlineObject.SetActive(isHovered);

            // Match door exactly for perfect hover
            SpriteRenderer outlineSR = outlineObject.GetComponent<SpriteRenderer>();
            if (outlineSR != null)
            {
                outlineSR.sortingOrder = 3; // always above everything
            }
        }
    }

    void UpdateOutlineTransform()
    {
        if (outlineObject == null || spriteRenderer == null) return;

        // Match position, rotation, and scale
        outlineObject.transform.position = transform.position;
        outlineObject.transform.rotation = transform.rotation;
        outlineObject.transform.localScale = transform.localScale * 1.1f; // slightly bigger for outline
    }

    void ResizeCollider()
    {
        BoxCollider2D box = doorCollider as BoxCollider2D;

        if (box != null && spriteRenderer != null)
        {
            Vector2 spriteSize = spriteRenderer.bounds.size;

            box.size = new Vector2(
                spriteSize.x * colliderSizeMultiplier.x,
                spriteSize.y * colliderSizeMultiplier.y
            );
        }
    }

    void SetGridPosition()
    {
        if (ShipGridManager.Instance == null || ShipGridManager.Instance.grid == null)
        {
            Debug.LogError("Door: Missing ShipGridManager or Grid!");
            return;
        }

        Vector3Int cell = ShipGridManager.Instance.grid.WorldToCell(transform.position);
        gridPosition = new Vector2Int(cell.x, cell.y);
    }

    void RegisterWithRooms()
    {
        Room[] allRooms = FindObjectsOfType<Room>();
        List<Room> foundRooms = new List<Room>();

        Vector3[] offsets = new Vector3[]
        {
            Vector3.left * 0.6f,
            Vector3.right * 0.6f,
            Vector3.up * 0.6f,
            Vector3.down * 0.6f
        };

        foreach (Vector3 offset in offsets)
        {
            Vector3 checkPos = transform.position + offset;

            foreach (Room room in allRooms)
            {
                if (room.tilemap == null) continue;

                Vector3Int cell = room.tilemap.WorldToCell(checkPos);

                if (room.tilemap.HasTile(cell) && !foundRooms.Contains(room))
                {
                    foundRooms.Add(room);
                }
            }
        }

        if (foundRooms.Count >= 2)
        {
            roomA = foundRooms[0];
            roomB = foundRooms[1];
            ConnectRooms();
        }
        else
        {
            Debug.LogWarning($"Door found {foundRooms.Count} rooms (expected 2)");
        }
    }

    void ConnectRooms()
    {
        if (roomA == null || roomB == null) return;

        Vector2Int dirAB = roomB.gridPosition - roomA.gridPosition;

        dirAB = new Vector2Int(
            Mathf.Clamp(dirAB.x, -1, 1),
            Mathf.Clamp(dirAB.y, -1, 1)
        );

        roomA.connections.Add(new RoomConnection
        {
            targetRoom = roomB,
            door = this,
            direction = dirAB
        });

        roomB.connections.Add(new RoomConnection
        {
            targetRoom = roomA,
            door = this,
            direction = -dirAB
        });
    }
}