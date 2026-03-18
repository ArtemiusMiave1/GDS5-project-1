using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int gridPosition;
    public bool isOpen = true;
    public Collider2D doorCollider;

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        doorCollider.enabled = !isOpen;
    }
}
