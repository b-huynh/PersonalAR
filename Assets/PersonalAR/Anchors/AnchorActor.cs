using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

public class AnchorActor : MonoBehaviour, IAnchorable
{
    private AnchorableObject _anchor;
    public string AnchorName
    {
        get => _anchor.WorldAnchorName;
    }

    public TextMesh tagMesh;

    void OnEnable()
    {
        foreach(Transform child in transform)
        {
            AppUtils.SetLayerRecursive(child.gameObject, LayerMask.NameToLayer("Default"));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDisable()
    {
        foreach(Transform child in transform)
        {
            AppUtils.SetLayerRecursive(child.gameObject, LayerMask.NameToLayer("Ignore Raycast"));
        }
    }

    public void SetDisplayText(string text)
    {
        tagMesh.text = text;
    }

    public void SetAnchor(AnchorableObject anchor)
    {
        _anchor = anchor;
        
        if (tagMesh)
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
