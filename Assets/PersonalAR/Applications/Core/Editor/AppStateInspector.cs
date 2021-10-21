using UnityEngine;
using UnityEditor;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

[CustomEditor(typeof(AppState))]
public class AppStateInspector : Editor
{
    AnchorService anchorService;

    void OnEnable()
    {
        IAnchorService serviceInterface;
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out serviceInterface))
        {
            anchorService = (AnchorService)serviceInterface;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AppState appState = (AppState)target;

        GUILayout.Label("Execution State");
        GUILayout.BeginVertical("box");

            GUILayout.Label($"[{appState.ExecutionState}]");
            GUILayout.BeginHorizontal("box");
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
            
            GUILayout.BeginHorizontal("box");
            if (anchorService != null && anchorService.AnchoredObjects != null)
            {
                foreach(var kv in anchorService.AnchoredObjects)
                {
                    if (GUILayout.Button(kv.Key, GUILayout.Width(50)))
                    {
                        var ec = new ExecutionContext(new GameObject());
                        ec.Anchor = kv.Value;
                        appState.StartActivity(ActivityType.ObjectMenu, ec);
                    }
                }
            }
            GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.Label("Running Activities");
        GUILayout.BeginVertical("box");
        foreach(var kv in appState.RunningActivities)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(kv.Value.ToString());
            GUILayout.Label(kv.Key.ToString());
            if (GUILayout.Button("Stop"))
            {
                var ec = new ExecutionContext(new GameObject());
                appState.StopActivity(kv.Key, ec);
                break;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.Label("App Variables");
        GUILayout.BeginVertical("box");
        if (appState.Variables != null)
        {
            var variablesEditor = Editor.CreateEditor(appState.Variables);
            variablesEditor.OnInspectorGUI();
        }
        GUILayout.EndVertical();
    }
}
