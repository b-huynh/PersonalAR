using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class ExtrudeHandle : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusHandler
{
    // Target bounding box. Must also have a BoundingBox component.
    public BoxCollider targetObject;

    // Materials for grabbed and ungrabbed states
    public Material handleMaterial;
    public Material handleGrabbedMaterial;

    // Direction of extrusion
    private Vector3 extrudeDir;

    // Used to determine amount to extrude based on how far handle is pulled
    private Vector3 initialGrabPoint;
    private Vector3 initialAxisGrabPoint;
    private Vector3 currentAxisGrabPoint;

    // Initial state of target, apply calculated scale factor on these values
    private Vector3 targetInitialPosition;
    private Vector3 targetInitialScale;

    // Minimum and Maximum scaling allowed. Based on targetObject's MinMaxScaleConstraint
    private float scaleMinimum;
    private float scaleMaximum;

    private Vector3 initialHandleAlignedPosition;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material = handleMaterial;
        Vector3 targetAlignedPosition =
            targetObject.transform.InverseTransformPoint(transform.position);
        extrudeDir = targetAlignedPosition.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PrintPositionInfo();
        }
    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        GetComponent<Renderer>().material = handleGrabbedMaterial;

        eventData.Use();
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        GetComponent<Renderer>().material = handleMaterial;
    
        eventData.Use();
    }



    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        GetComponent<Renderer>().material = handleGrabbedMaterial;

        var scaleConstraint = targetObject.GetComponent<MinMaxScaleConstraint>();
        scaleMinimum = scaleConstraint.ScaleMinimum;
        scaleMaximum = scaleConstraint.ScaleMaximum;

        targetInitialPosition = targetObject.transform.localPosition;
        targetInitialScale = targetObject.transform.localScale;

        initialGrabPoint = eventData.Pointer.Result.Details.Point;

        // GameObject grabbedHandle = eventData.Pointer.Result.CurrentPointerTarget;
        // Vector3 initialHandleAlignedPosition =
        //     targetObject.transform.InverseTransformPoint(grabbedHandle.transform.position);
        // extrudeDir = initialHandleAlignedPosition.normalized;

        initialAxisGrabPoint = Vector3.Project(eventData.Pointer.Position - initialGrabPoint, extrudeDir);

        eventData.Use();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        Vector3 pointerPosition =  eventData.Pointer.Position;
        currentAxisGrabPoint = Vector3.Project(pointerPosition - initialGrabPoint, extrudeDir);

        Vector3 scaleFactor = currentAxisGrabPoint - initialAxisGrabPoint;
        if (extrudeDir == Vector3.left || extrudeDir == Vector3.back || extrudeDir == Vector3.down)
            scaleFactor = -scaleFactor;

        Vector3 newTargetScale = targetInitialScale + scaleFactor;
        if (IsWithinConstraints(newTargetScale, scaleMinimum, scaleMaximum))
        {
            targetObject.transform.localScale = newTargetScale;

            if (extrudeDir == Vector3.left || extrudeDir == Vector3.back || extrudeDir == Vector3.down)
                targetObject.transform.localPosition = targetInitialPosition - (scaleFactor / 2.0f);
            else
                targetObject.transform.localPosition = targetInitialPosition + (scaleFactor / 2.0f);
        }

        eventData.Use();
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        GetComponent<Renderer>().material = handleMaterial;

        eventData.Use();
    }

    private bool IsWithinConstraints(Vector3 targetScale, float min, float max)
    {
        if (targetScale.x >= min && targetScale.y >= min && targetScale.z >= min &&
            targetScale.x <= max && targetScale.y <= max && targetScale.z <= max)
            return true;
        else
            return false;
    }

    private void PrintPositionInfo()
    {
        string log =
              "Initial Grab Point: " + initialGrabPoint.ToString("F4") + "\n"
            + "Extrude Dir: " + extrudeDir.ToString("F4") + "\n"
            + "Bounding Box World Position: " + targetObject.transform.position.ToString("F4") + "\n"
            + "Bounding Box Extents: " + targetObject.bounds.extents.ToString("F4") + "\n"
            + "Bounding Box lossyScale: " + targetObject.transform.lossyScale.ToString("F4") + "\n";
        Debug.Log(log);
    }

    // Event is not used
    public void OnPointerClicked(MixedRealityPointerEventData eventData) {}
}
