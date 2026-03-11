using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = true;
    public Collider2D doorCollider;

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        doorCollider.enabled = !isOpen;
    }
}
