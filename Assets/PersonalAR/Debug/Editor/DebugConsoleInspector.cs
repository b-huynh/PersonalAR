using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugConsole))]
public class DebugConsoleInspector : Editor
{
    string testMessage = string.Empty;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebugConsole dc = (DebugConsole)target;

        testMessage = EditorGUILayout.TextField("Test Message: ", testMessage);

        GUILayout.BeginVertical("box");

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Send Message");
        if (GUILayout.Button("Log")) { ARDebug.Log(testMessage, dc); }
        if (GUILayout.Button("Warn")) { ARDebug.LogWarning(testMessage, dc); }
        if (GUILayout.Button("Error")) { ARDebug.LogError(testMessage, dc); }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        GUILayout.Label($"Log to Unity: {ARDebug.logToUnityConsole}");
        if (GUILayout.Button("True")) { dc.SetLogToUnityConsole(true); }
        if (GUILayout.Button("False")) { dc.SetLogToUnityConsole(false); }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}