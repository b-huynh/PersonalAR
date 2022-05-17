using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomPinCodes))]
public class RandomPinCodesInspector : Editor
{
    static int testCode;
    public override void OnInspectorGUI()
    {
        RandomPinCodes pinCodes = (RandomPinCodes)target;

        DrawDefaultInspector();

        GUILayout.BeginVertical("box");
        if (GUILayout.Button("Reset", GUILayout.Width(100)))
        {
            pinCodes.Reset();
        }

        if (GUILayout.Button("Generate", GUILayout.Width(100)))
        {
            pinCodes.Generate();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test Code: ", GUILayout.Width(100)))
        {
            Debug.Log(pinCodes.Contains(testCode));
            var matches = pinCodes.Codes.Where(code => code.CompareEntry(testCode));
            Debug.Log($"Matches: {matches.Count()}");
            foreach(var match in matches)
            {
                Debug.Log(match);
            }
        }
        testCode = EditorGUILayout.IntField(testCode,GUILayout.Width(75));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("CODE", GUILayout.Width(100));
        GUILayout.Label("| P0 (Asgn.)", GUILayout.Width(75));
        GUILayout.Label("| P1 (Asgn.)", GUILayout.Width(75));
        GUILayout.Label("| P2 (Asgn.)", GUILayout.Width(75));
        GUILayout.EndHorizontal();

        foreach(Code code in pinCodes.Codes)
        {
            GUILayout.BeginHorizontal();
            string usedMark = code.Used ? "( Y )" : "( N )";
            GUILayout.Label($"{code.ToString()} {usedMark}", GUILayout.Width(100));
            foreach(CodePiece piece in code.Pieces)
            {
                string assignedMark = piece.Assigned ? "( Y )" : "( N )";
                GUILayout.Label($"| {piece.Value} {assignedMark}", GUILayout.Width(75));
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}
