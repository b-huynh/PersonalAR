using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions; 

public class SmarthubApp : BaseApp
{
    public int numSmartObjects;
    [NonSerialized] private int numAssigned = 0;

    private List<string> keywords = new List<string>();

    private IAnchorService anchorService;

    // Start is called before the first frame update
    void Start()
    {
        // Register for new object events
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out anchorService))
        {
            anchorService.OnRegistered += OnObjectRegistered;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnObjectRegistered(AnchorableObject anchor)
    {
        // Register events from first numSmartObjects encountered.
        if (numAssigned < numSmartObjects)
        {
            anchorService.AddHandler(anchor, appState);
        }
    }

    public void LaunchSmartInfoMenu(AnchorableObject anchor)
    {
        // anchor.GetComponentInChildren<AnchorContentController>();
    }
}
