using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomPinCodes))]
public class RandomPinCodesInspector : Editor
{
    public override void OnInspectorGUI()
    {
        RandomPinCodes pinCodes = (RandomPinCodes)target;

        DrawDefaultInspector();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Generate", GUILayout.Width(100)))
        {
            pinCodes.Generate();
        }
        foreach(DictionaryEntry kv in pinCodes.labeledCodePieces)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(kv.Key.ToString(), GUILayout.Width(25));
            GUILayout.Label(kv.Value.ToString());
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}
