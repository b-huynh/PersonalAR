using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Extensions;
using Microsoft.MixedReality.Toolkit.Utilities;
public class GestureListener : MonoBehaviour, IMixedRealityGestureHandler
{
    public static List<GestureEvent> gestures = new List<GestureEvent>();
    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityGestureHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityGestureHandler>(this);
    }

    public void OnGestureStarted(InputEventData eventData)
    {
        //ARDebug.Log($"OnGestureStarted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

        //Debug.Log($"OnGestureStarted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

        GestureEvent gesture = new GestureEvent();
        gesture.unixTime = Utils.UnixTimestampMilliseconds();
        gesture.systemTime = eventData.EventTime.ToString("HH-mm-ss-ff");
        gesture.action = eventData.MixedRealityInputAction.Description;
        gesture.eventType = "Start";

        Vector3 rightEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out rightEndPoint))
        {
            gesture.hand = "right";
            gesture.position = rightEndPoint;
            Debug.Log("right hand started at " + rightEndPoint);
        }

        Vector3 leftEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Left, out leftEndPoint))
        {
            gesture.hand = "left";
            gesture.position = leftEndPoint;
            Debug.Log("left hand started at " + leftEndPoint);
        }

        gestures.Add(gesture);

    }

    public void OnGestureUpdated(InputEventData eventData)
    {
        //ARDebug.Log($"OnGestureUpdated [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        
        //Debug.Log($"OnGestureUpdated [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        
        GestureEvent gesture = new GestureEvent();
        gesture.unixTime = Utils.UnixTimestampMilliseconds();
        gesture.systemTime = eventData.EventTime.ToString("HH-mm-ss-ff");
        gesture.action = eventData.MixedRealityInputAction.Description;
        gesture.eventType = "Update";
        Vector3 rightEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out rightEndPoint))
        {
            gesture.hand = "right";
            gesture.position = rightEndPoint;
            Debug.Log("right hand updated at " + rightEndPoint);
        }

        Vector3 leftEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Left, out leftEndPoint))
        {
            gesture.hand = "left";
            gesture.position = leftEndPoint;
            Debug.Log("left hand updated at " + leftEndPoint);
        }

        gestures.Add(gesture);

    }

    public void OnGestureCompleted(InputEventData eventData)
    {
        //ARDebug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        
        //Debug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        
        GestureEvent gesture = new GestureEvent();
        gesture.unixTime = Utils.UnixTimestampMilliseconds();
        gesture.systemTime = eventData.EventTime.ToString("HH-mm-ss-ff");
        gesture.action = eventData.MixedRealityInputAction.Description;
        gesture.eventType = "Complete";

        Vector3 rightEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out rightEndPoint))
        {
            gesture.hand = "right";
            gesture.position = rightEndPoint;
            Debug.Log("right hand completed at " + rightEndPoint);
        }

        Vector3 leftEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Left, out leftEndPoint))
        {
            gesture.hand = "left";
            gesture.position = leftEndPoint;
            Debug.Log("left hand completed at " + leftEndPoint);
        }

        gestures.Add(gesture);
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        //ARDebug.Log($"OnGestureCanceled [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        //Debug.Log($"OnGestureCanceled [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        
        GestureEvent gesture = new GestureEvent();
        gesture.unixTime = Utils.UnixTimestampMilliseconds();
        gesture.systemTime = eventData.EventTime.ToString("HH-mm-ss-ff");
        gesture.action = eventData.MixedRealityInputAction.Description;
        gesture.eventType = "Cancel";
        
        Vector3 rightEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out rightEndPoint))
        {
            gesture.hand = "right";
            gesture.position = rightEndPoint;
            Debug.Log("right hand cancelled at " + rightEndPoint);
        }

        Vector3 leftEndPoint;
        if (PointerUtils.TryGetHandRayEndPoint(Handedness.Left, out leftEndPoint))
        {
            gesture.hand = "left";
            gesture.position = leftEndPoint;
            Debug.Log("left hand cancelled at " + leftEndPoint);
        }

        gestures.Add(gesture);
    }

    private void SetIndicator(GameObject indicator, string label, Material material)
    {
        if (indicator)
        {
            var renderer = indicator.GetComponentInChildren<Renderer>();
            if (material && renderer)
            {
                renderer.material = material;
            }
            var text = indicator.GetComponentInChildren<TextMeshPro>();
            if (text)
            {
                text.text = label;
            }
        }
    }

    private void SetIndicator(GameObject indicator, string label, Material material, Vector3 position)
    {
        SetIndicator(indicator, label, material);
        if (indicator)
        {
            indicator.transform.localPosition = position;
        }
    }

    public static List<GestureEvent> getGestureEvents(){
        List<GestureEvent> perviousEvents = gestures;

        gestures = new List<GestureEvent>();
        return perviousEvents;
    }
}

[System.Serializable]
public class GestureEvent{
    public long unixTime;
    public string systemTime;
    public long eventTime;
    public string action;

    public string eventType;
    public Vector3 position;

    public string hand;

}