using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Map))]
public class MapGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Map myScript = (Map)target;
        if (GUILayout.Button("Generate Map"))
        {
            myScript.GenerateMap();
        }
        if (GUILayout.Button("Destroy Map"))
        {
            myScript.DestroyMap();
        }
    }
}
