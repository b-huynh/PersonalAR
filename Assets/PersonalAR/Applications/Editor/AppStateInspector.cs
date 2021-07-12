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
