using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

public class AnchorServiceController : MonoBehaviour
{
    private AnchorService anchorService;

    void OnEnable()
    {
        IAnchorService service;
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out service))
        {
            anchorService = (AnchorService)service;
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

    public void UnregisterAnchor(string anchorName) => anchorService?.UnregisterAnchor(anchorName);
    public void Clear() => anchorService?.Clear();
}
