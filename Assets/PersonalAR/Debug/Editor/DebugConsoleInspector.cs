using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugConsole))]
public class DebugConsoleInspector : Editor
{
    string testMessage = "";
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebugConsole dc = (DebugConsole)target;


        testMessage = EditorGUILayout.TextField("Test Message: ", testMessage);

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Test: ");
        if (GUILayout.Button("Log")) { Debug.Log(testMessage); }
        if (GUILayout.Button("Warn")) { Debug.LogWarning(testMessage); }
        if (GUILayout.Button("Error")) { Debug.LogError(testMessage); }
        GUILayout.EndHorizontal();
    }
}