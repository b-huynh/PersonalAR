using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScaleTween : MonoBehaviour
{
    public LeanTweenType inTween;
    public Vector3 inTweenTo;
    public LeanTweenType outTween;
    public Vector3 outTweenTo;
    public bool tweenOutOnStart;
    public float duration;
    public float delay;
    public UnityEvent OnTweenInComplete;
    public UnityEvent OnTweenOutComplete;

    // Is true only if most recent tween call was TweenIn.
    public bool IsTweenedIn { get; private set; }

    // void OnEnable()
    // {
    //     transform.localScale = outTweenTo;
    // }

    void Reset()
    {
        tweenOutOnStart = true;
    }

    public void TweenIn()
    {
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, inTweenTo, duration)
            .setDelay(delay)
            .setEase(inTween)
            .setOnComplete(delegate () { OnTweenInComplete.Invoke(); });
        IsTweenedIn = true;
    }

    public void TweenOut()
    {
        LeanTween.scale(gameObject, outTweenTo, duration)
            .setDelay(delay)
            .setEase(outTween)
            .setOnComplete(delegate () { OnTweenOutComplete.Invoke(); });
        IsTweenedIn = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (tweenOutOnStart)
        {
            TweenOut();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
