using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FocusMode : InputSystemGlobalHandlerListener, IMixedRealityGestureHandler
{
    public GameObject HoldIndicator = null;
    public GameObject SelectIndicator = null;

    public Material DefaultMaterial = null;
    public Material HoldMaterial = null;
    public Material SelectMaterial = null;
    
    #region InputSystemGlobalHandlerListener Implementation

    protected override void RegisterHandlers()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityGestureHandler>(this);
        ARDebug.Log("Registered gestuer handler");
    }

    protected override void UnregisterHandlers()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityGestureHandler>(this);
        ARDebug.Log("Unregistered Gesture Handler");
    }

    #endregion InputSystemGlobalHandlerListener Implementation

    public void OnGestureStarted(InputEventData eventData)
    {
        ARDebug.Log($"OnGestureStarted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

        
        // var action = eventData.MixedRealityInputAction.Description;
        // if (action == "Hold Action")
        // {
        //     SetIndicator(HoldIndicator, "Hold: started", HoldMaterial);
        // }
        // // else if (action == "Manipulate Action")
        // // {
        // //     SetIndicator(ManipulationIndicator, $"Manipulation: started {Vector3.zero}", ManipulationMaterial, Vector3.zero);
        // // }
        // // else if (action == "Navigation Action")
        // // {
        // //     SetIndicator(NavigationIndicator, $"Navigation: started {Vector3.zero}", NavigationMaterial, Vector3.zero);
        // //     ShowRails(Vector3.zero);
        // // }

        // SetIndicator(SelectIndicator, "Select:", DefaultMaterial);
    }

    public void OnGestureUpdated(InputEventData eventData)
    {
        ARDebug.Log($"OnGestureUpdated [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        
        // var action = eventData.MixedRealityInputAction.Description;
        // if (action == "Hold Action")
        // {
        //     SetIndicator(HoldIndicator, "Hold: updated", DefaultMaterial);
        // }
    }

    public void OnGestureUpdated(InputEventData<Vector3> eventData)
    {
        ARDebug.Log($"OnGestureUpdated [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        // var action = eventData.MixedRealityInputAction.Description;
        // if (action == "Manipulate Action")
        // {
        //     SetIndicator(ManipulationIndicator, $"Manipulation: updated {eventData.InputData}", ManipulationMaterial, eventData.InputData);
        // }
        // else if (action == "Navigation Action")
        // {
        //     SetIndicator(NavigationIndicator, $"Navigation: updated {eventData.InputData}", NavigationMaterial, eventData.InputData);
        //     ShowRails(eventData.InputData);
        // }
    }

    public void OnGestureCompleted(InputEventData eventData)
    {
        ARDebug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        // var action = eventData.MixedRealityInputAction.Description;
        // if (action == "Hold Action")
        // {
        //     SetIndicator(HoldIndicator, "Hold: completed", DefaultMaterial);
        // }
        // else if (action == "Select")
        // {
        //     SetIndicator(SelectIndicator, "Select: completed", SelectMaterial);
        // }
    }

    public void OnGestureCompleted(InputEventData<Vector3> eventData)
    {
        ARDebug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        // var action = eventData.MixedRealityInputAction.Description;
        // if (action == "Manipulate Action")
        // {
        //     SetIndicator(ManipulationIndicator, $"Manipulation: completed {eventData.InputData}", DefaultMaterial, eventData.InputData);
        // }
        // else if (action == "Navigation Action")
        // {
        //     SetIndicator(NavigationIndicator, $"Navigation: completed {eventData.InputData}", DefaultMaterial, eventData.InputData);
        //     HideRails();
        // }
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        ARDebug.Log($"OnGestureCanceled [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
        // var action = eventData.MixedRealityInputAction.Description;
        // if (action == "Hold Action")
        // {
        //     SetIndicator(HoldIndicator, "Hold: canceled", DefaultMaterial);
        // }
        // // else if (action == "Manipulate Action")
        // // {
        // //     SetIndicator(ManipulationIndicator, "Manipulation: canceled", DefaultMaterial);
        // // }
        // // else if (action == "Navigation Action")
        // // {
        // //     SetIndicator(NavigationIndicator, "Navigation: canceled", DefaultMaterial);
        // //     HideRails();
        // // }
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
}