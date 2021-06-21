using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AppState))]
public class AppStateInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AppState appState = (AppState)target;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Running Activities");

        foreach(var kv in appState.RunningActivities)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(kv.Value.ToString());
            GUILayout.Label(kv.Key.ToString());
            if (GUILayout.Button("Stop"))
            {
                // Do nothing for now...
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }
}
