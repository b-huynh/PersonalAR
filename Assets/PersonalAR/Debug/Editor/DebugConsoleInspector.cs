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

        if (GUILayout.Button("Test Log Message"))
        {
            Debug.Log(testMessage);
        }

        if (GUILayout.Button("Test Error Message"))
        {
            Debug.LogError(testMessage);
        }
    }
}