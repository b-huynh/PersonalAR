using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AppStateListener))]
public class AppStateListenerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AppStateListener listener = (AppStateListener)target;
        AppState app = listener.GetAppState();

        if (GUILayout.Button("Toggle App On/Off"))
        {
            app.ToggleStartOrSuspend();
        }
    }
}
