using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShipGrid))]
public class ShipGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShipGrid shipGrid = (ShipGrid)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Print Rooms to Console"))
        {
            shipGrid.PrintRooms();
        }
    }
}