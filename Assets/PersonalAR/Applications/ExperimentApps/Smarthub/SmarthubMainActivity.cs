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

    private List<Guid> cachedObjectActivityGuids = new List<Guid>();
    private List<SmarthubObjectActivity> cachedObjectActivities = new List<SmarthubObjectActivity>();

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

            // Start all known anchors as object subactivities
            foreach(AnchorableObject anchor in anchorService.AnchoredObjects.Values)
            {
                ExecutionContext ec = new ExecutionContext(this.gameObject);
                ec.Anchor = anchor;

                GameObject newActivity = GameObject.Instantiate(this.appRuntime.anchorActivityTemplate.gameObject, this.transform);
                SmarthubObjectActivity newAnchorActivity = newActivity.GetComponent<SmarthubObjectActivity>();
                newAnchorActivity.StartActivity(ec);
                cachedObjectActivities.Add(newAnchorActivity);
            }

            // Subscribe to anchor changes
            anchorService.OnAfterRegistered += OnAfterAnchorRegistered;
            anchorService.OnBeforeRemoved += OnBeforeAnchorRemoved;

            initialized = true;
        }
        else
        {
            // Start all known anchors as object subactivities
            foreach(SmarthubObjectActivity anchorActivity in cachedObjectActivities)
            {
                ExecutionContext ec = new ExecutionContext(this.gameObject);
                ec.Anchor = anchorActivity.anchor;
                anchorActivity.StartActivity(ec);
            }
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

        // Suspend all cached sub activities
        // cachedObjectActivities.ForEach(appGuid => appState.StopActivity(appGuid, new ExecutionContext(this.gameObject)));



        // // Get all anchors that this app is known to handle
        // HashSet<AnchorableObject> handledAnchors = anchorService.handlersByApp[appState];
    
        // // Start all object activities for handled anchors
        // foreach(AnchorableObject anchor in handledAnchors)
        // {
        //     if (anchor != null)
        //     {
        //         ExecutionContext ec = new ExecutionContext(this.gameObject);
        //         ec.Anchor = anchor;

        //         Guid appId = appState.StartActivity(ActivityType.ObjectMenu, ec, false);

        //         if (cachedObjectActivities.Contains(appId) == false)
        //         {
        //             cachedObjectActivities.Add(appId);
        //         }
        //     }
        // }
    }

    public override void StopActivity(ExecutionContext executionContext)
    {
        // foreach(Guid cachedID in cachedObjectActivityGuids)
        // {
        //     ExecutionContext ec = new ExecutionContext(this.gameObject);
        //     appState.StopActivity(cachedID, ec);
        // }

        // Debug.Log("SmarthubMainActivity StopActivity");
        foreach(SmarthubObjectActivity anchorActivity in cachedObjectActivities)
        {
            ExecutionContext ec = new ExecutionContext(this.gameObject);
            anchorActivity.StopActivity(ec);
        }
    }

    public void OnAfterAnchorRegistered(AnchorableObject newAnchor)
    {
        // Create new object activity 
        GameObject newActivity = GameObject.Instantiate(this.appRuntime.anchorActivityTemplate.gameObject, this.transform);
        SmarthubObjectActivity newAnchorActivity = newActivity.GetComponent<SmarthubObjectActivity>();
        // newAnchorActivity.anchor = newAnchor;

        // Match current execution state
        cachedObjectActivities.Add(newAnchorActivity);
        if (appState.ExecutionState == ExecutionState.RunningFull)
        {
            ExecutionContext ec = new ExecutionContext(this.gameObject);
            ec.Anchor = newAnchor;
            newAnchorActivity.StartActivity(ec);
        }
        else
        {
            ExecutionContext ec = new ExecutionContext(this.gameObject);
            ec.Anchor = newAnchor;
            newAnchorActivity.StartActivity(ec);
            newAnchorActivity.StopActivity(ec);
        }
    }

    public void OnBeforeAnchorRemoved(AnchorableObject toRemoveAnchor)
    {
        SmarthubObjectActivity toRemoveActivity = 
            cachedObjectActivities.Find(activity => activity.anchor == toRemoveAnchor);
        
        ExecutionContext ec = new ExecutionContext(this.gameObject);
        toRemoveActivity.StopActivity(ec);

        cachedObjectActivities.Remove(toRemoveActivity);
        Destroy(toRemoveActivity.gameObject);
    }
}
