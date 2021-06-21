using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScaleTween))]
public class ScaleTweenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ScaleTween scaleTween = (ScaleTween)target;

        GUILayout.BeginHorizontal("box");
        GUILayout.Label($"IsTweenedIn: {scaleTween.IsTweenedIn}");
        if (GUILayout.Button("TweenIn"))
        {
            scaleTween.TweenIn();
        }

        if (GUILayout.Button("TweenOut"))
        {
            scaleTween.TweenOut();
        }
        GUILayout.EndHorizontal();
    }
}
