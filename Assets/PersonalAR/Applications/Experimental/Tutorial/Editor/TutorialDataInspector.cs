using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TutorialData))]
public class TutorialDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Create TTS", GUILayout.Width(100)))
        {
            Debug.Log($"Called on {target.GetInstanceID()}");
        }
    }
}
