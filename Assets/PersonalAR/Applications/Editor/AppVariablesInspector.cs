using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AppVariables), true)]
public class AppVariablesInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AppVariables variables = (AppVariables)target;
     
        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("Save"))
        {
            if (variables.Save() == true)
            {
                Debug.Log("Saved filed successfully");
            }
            else
            {
                Debug.LogError("Failed to save file");
            }
        }

        if (GUILayout.Button("Load"))
        {
            if (variables.Load() == true)
            {
                Debug.Log("Variables loaded successfully");
            }
            else
            {
                Debug.LogError("Failed to load variables");
            }
        }
        GUILayout.EndHorizontal();
    }
}
