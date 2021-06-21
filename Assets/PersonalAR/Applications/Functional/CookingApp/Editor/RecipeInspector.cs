using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Recipe))]
public class RecipeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Recipe recipe = (Recipe)target;

        GUILayout.BeginHorizontal("box");
        GUILayout.Label($"Step: {recipe.CurrentIndex + 1} / {recipe.Steps.Count}");
        if (GUILayout.Button("Next", GUILayout.Width(100)))
        {
            recipe.Next();
        }

        if (GUILayout.Button("Reset", GUILayout.Width(100)))
        {
            recipe.Reset();
        }
        GUILayout.EndHorizontal();
    }
}
