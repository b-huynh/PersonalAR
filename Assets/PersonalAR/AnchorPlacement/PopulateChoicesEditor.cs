using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PopulateChoices))]
public class PopulateChoicesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PopulateChoices pc = (PopulateChoices)target;

        if (GUILayout.Button("Repopulate Buttons"))
        {
            pc.RepopulateButtons();
        }
    }
}
