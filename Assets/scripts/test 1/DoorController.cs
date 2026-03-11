using UnityEngine;

public class DoorController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                Door door = hit.collider.GetComponent<Door>();
                if (door != null) door.ToggleDoor();
            }
        }
    }
}
