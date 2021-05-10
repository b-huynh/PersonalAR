using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnnotationSetVisuals))]
public class AnnotationSetVisualsInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AnnotationSetVisuals component = (AnnotationSetVisuals)target;
        if (GUILayout.Button("Generate Buttons"))
        {
            component.RepopulateButtons();
        }
    }
}
