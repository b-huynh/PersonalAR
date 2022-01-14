using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions; 

public class MeshNetworkApp : BaseApp
{
    private IAnchorService anchorService;
    // Start is called before the first frame update
    void Start()
    {
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out anchorService))
        {
            // anchorService.OnRegistered += OnObjectRegistered;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
