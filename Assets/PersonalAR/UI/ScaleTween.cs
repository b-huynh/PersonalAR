using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[CustomEditor(typeof(ScaleTween))]
public class ScaleTweenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ScaleTween scaleTween = (ScaleTween)target;

        if (GUILayout.Button("TweenIn"))
        {
            scaleTween.TweenIn();
        }

        if (GUILayout.Button("TweenOut"))
        {
            scaleTween.TweenOut();
        }
    }
}

public class ScaleTween : MonoBehaviour
{
    public LeanTweenType inTween;
    public Vector3 inTweenTo;
    public LeanTweenType outTween;

    public float duration;
    public float delay;
    public UnityEvent OnTweenInComplete;
    public UnityEvent OnTweenOutComplete;

    void OnEnable()
    {
        transform.localScale = Vector3.zero;
    }

    public void TweenIn()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, inTweenTo, duration)
            .setDelay(delay)
            .setEase(inTween)
            .setOnComplete(delegate () { OnTweenInComplete.Invoke(); });
    }

    public void TweenOut()
    {
        LeanTween.scale(gameObject, Vector3.zero, duration)
            .setDelay(delay)
            .setEase(outTween)
            .setOnComplete(delegate () { OnTweenOutComplete.Invoke(); });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
