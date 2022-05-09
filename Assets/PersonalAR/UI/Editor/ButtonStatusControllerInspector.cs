using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonStatusController))]
public class ButtonStatusControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ButtonStatusController controller = (ButtonStatusController)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Enable"))
        {
            controller.SetStatus(true);
        }

        if (GUILayout.Button("Disable"))
        {
            controller.SetStatus(false);
        }
        GUILayout.EndHorizontal();
    }
}
