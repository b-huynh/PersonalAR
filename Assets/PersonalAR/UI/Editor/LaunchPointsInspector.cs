using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LaunchPoints))]
public class LaunchPointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LaunchPoints launchPoints = (LaunchPoints)target;

        if (GUILayout.Button("Create HUD Points"))
        {
            launchPoints.CreateHUDPoints();
        }

        if (GUILayout.Button("GetPoint"))
        {
            Transform t = launchPoints.GetLaunchPoint();
        }
    }
}
