using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

[CustomEditor(typeof(AnchorServiceDebugger))]
public class AnchorServiceDebuggerInspector : Editor
{
    private AnchorService anchorService;
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
        if (anchorService == null) { return; }

        GUILayout.BeginVertical("box");
        GUILayout.Label("HANDLERS", EditorStyles.boldLabel);
        foreach(var kv in anchorService.handlers)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label(kv.Key.WorldAnchorName, GUILayout.Width(75));
            foreach(AppState app in kv.Value)
            {
                string appName = app.appName == string.Empty ? "Empty" : app.appName;
                GUILayout.Label(appName, GUILayout.Width(100));
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}
