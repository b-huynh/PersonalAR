using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions; 

public class SmarthubMainActivity : BaseAppActivity
{
    public int numCodePieceObjects;

    private List<Guid> cachedObjectActivities;

    public AnchorService anchorService;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void StartActivity(ExecutionContext executionContext)
    {
        if (initialized == false)
        {
            // Register for new object events
            IAnchorService serviceInterface;
            if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out serviceInterface))
            {
                anchorService = (AnchorService)serviceInterface;
            }
            else
            {
                ARDebug.LogError("Failed to get anchor service in SmarthubMainActivity");
            }

            cachedObjectActivities = new List<Guid>();

            // Randomly select n number of objects to be code piece handlers
            System.Random rnd = new System.Random();
            List<AnchorableObject> anchorsCopy = anchorService.AnchoredObjects.Values.OrderBy((item) => rnd.Next()).ToList();
            for(int i = 0; i < numCodePieceObjects; ++i)
            {
                anchorService.AddHandler(anchorsCopy[i], appState);
            }

            initialized = true;
        }

        // TODO: This is extremely non-performant.
        // You should really cast into ObjectActivities so you can determine which anchor activities have already been started/suspended first.
        // As is, this creates many unnecessary duplicates.

        // Attempt to launch all object activities

        // Suspend all existing object activities
        foreach(var kv in appState.RunningActivities)
        {
            if (kv.Value == ActivityType.ObjectMenu && kv.Key != this.activityID)
            {
                ExecutionContext ec = new ExecutionContext(this.gameObject);
                appState.StopActivity(kv.Key, ec);
            }
        }

        // Get all anchors that this app is known to handle
        HashSet<AnchorableObject> handledAnchors = anchorService.handlersByApp[appState];
    
        // Start all object activities for handled anchors
        foreach(AnchorableObject anchor in handledAnchors)
        {
            if (anchor != null)
            {
                ExecutionContext ec = new ExecutionContext(this.gameObject);
                ec.Anchor = anchor;

                Guid appId = appState.StartActivity(ActivityType.ObjectMenu, ec, false);

                if (cachedObjectActivities.Contains(appId) == false)
                {
                    cachedObjectActivities.Add(appId);
                }
            }
        }
    }

    public override void StopActivity(ExecutionContext executionContext)
    {
        foreach(Guid cachedID in cachedObjectActivities)
        {
            ExecutionContext ec = new ExecutionContext(this.gameObject);
            appState.StopActivity(cachedID, ec);
        }
    }
}
