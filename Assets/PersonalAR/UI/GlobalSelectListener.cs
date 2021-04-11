using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;

using Recaug;

public class GlobalSelectListener : MonoBehaviour, IMixedRealityInputActionHandler
{
    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnActionStarted(BaseInputEventData eventData)
    {
        var action = eventData.MixedRealityInputAction.Description;
        if (action == "Select")
        {
            if (eventData.InputSource.SourceType == InputSourceType.Hand)
            {
                if (eventData.InputSource.Pointers.Length == 0) return;

                Vector3 hitposition = eventData.InputSource.Pointers[0].BaseCursor.Position;
                GameObject target = eventData.InputSource.Pointers[0].Result.CurrentPointerTarget;

                hitposition += (Camera.main.transform.position - hitposition).normalized * 0.1f;

                if (target && target.layer == LayerMask.NameToLayer("Spatial Awareness"))
                {
                    DoSelect(hitposition);
                }
            }
        }
    }

    public void OnActionEnded(BaseInputEventData eventData)
    {
        // Debug.Log(eventData.MixedRealityInputAction.Description);
    }

    public void DoSelect(Vector3 hitPos)
    {
        // Hit a position on spatial mesh with any select input event.
    }
}
