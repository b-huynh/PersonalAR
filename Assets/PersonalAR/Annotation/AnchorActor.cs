using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

using TMPro;

public class AnchorActor : MonoBehaviour, IAnchorable
{
    // Reference to a scriptable variable, e.g. AnnotationModeBoolVariable
    [SerializeField]
    private BoolVariable IsEnabled;

    [SerializeField]
    private GameObject mesh;
    [SerializeField]
    private GameObject label;

    private AnchorableObject _anchor;
    public string AnchorName
    {
        get => _anchor.WorldAnchorName;
    }

    public TMPro.TextMeshPro tagTextMesh;

    void OnEnable()
    {
        // Match current value
        OnValueChanged(IsEnabled.RuntimeValue);
        // Subscribe to value changed events.
        IsEnabled.OnValueChanged += OnValueChanged;
    }

    void OnDisable()
    {
        IsEnabled.OnValueChanged -= OnValueChanged;
    }

    private void OnValueChanged(bool newValue)
    {
        mesh.SetActive(newValue);
        label.SetActive(newValue);
    }

    public void SetDisplayText(string text)
    {
        tagTextMesh.text = text;
    }

    public void SetAnchor(AnchorableObject anchor)
    {
        _anchor = anchor;
        
        if (tagTextMesh != null)
        {
            SetDisplayText(_anchor.WorldAnchorName);
        }
    }

    public void Delete()
    {
        IAnchorService anchorService;
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out anchorService))
        {
            anchorService.UnregisterAnchor(AnchorName);
        }
    }
}
