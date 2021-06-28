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

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Launch: ");
        if (GUILayout.Button("Main"))
        {
            var ec = new ExecutionContext(new GameObject());
            appState.StartActivity(ActivityType.MainMenu, ec);
        }
        if (GUILayout.Button("Default"))
        {
            var ec = new ExecutionContext(new GameObject());
            appState.StartActivity(ActivityType.Default, ec);
        }
        GUILayout.EndHorizontal();

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
