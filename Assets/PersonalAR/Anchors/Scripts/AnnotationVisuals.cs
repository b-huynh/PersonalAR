using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

using TMPro;

[RequireComponent(typeof(Anchorable))]
public class AnnotationVisuals : MonoBehaviour
{
    // Reference to a scriptable variable, e.g. AnnotationModeBoolVariable
    [SerializeField]
    private BoolVariable AnnotationModeState;

    // Visuals
    [SerializeField]
    private GameObject label;
    public TMPro.TextMeshPro tagTextMesh;

    void OnEnable()
    {
        // Match current value
        OnValueChanged(AnnotationModeState.RuntimeValue);
        // Subscribe to value changed events.
        AnnotationModeState.OnValueChanged += OnValueChanged;
        
        GetComponent<Anchorable>().OnAnchorSet.AddListener(OnAnchorSet);
    }

    void OnDisable()
    {
        AnnotationModeState.OnValueChanged -= OnValueChanged;

        GetComponent<Anchorable>().OnAnchorSet.RemoveListener(OnAnchorSet);
    }

    private void OnValueChanged(bool newValue)
    {
        label.SetActive(newValue);
    }

    public void SetDisplayText(string text)
    {
        tagTextMesh.text = text;
    }

    public void OnAnchorSet(AnchorableObject anchor)
    {   
        if (tagTextMesh != null)
        {
            SetDisplayText(anchor.WorldAnchorName);
        }
    }

    public void Delete()
    {
        string anchorName = GetComponent<Anchorable>().Anchor.WorldAnchorName;

        IAnchorService anchorService;
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out anchorService))
        {
            anchorService.UnregisterAnchor(anchorName);
        } 
    }
}
