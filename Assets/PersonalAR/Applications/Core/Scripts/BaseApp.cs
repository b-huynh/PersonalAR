﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;

[Serializable]
public enum ActivityType { Default, MainMenu, ObjectMenu }

[Serializable]
public struct ActivityEntry
{
    public ActivityType activityType;
    public BaseAppActivity activity;
}

public class BaseApp : MonoBehaviour, IAppStateListener
{
    [Header("Activities")]
    public List<ActivityEntry> activities;
    public AnchorActivity anchorActivityTemplate
    {
        get
        {
            var entries = activities.Where(entry => entry.activityType == ActivityType.ObjectMenu);
            return entries.Count() >= 1 ? (AnchorActivity)entries.First().activity : null;
        }
    }

    private Dictionary<Guid, BaseAppActivity> runningActivities;
    private Dictionary<Guid, BaseAppActivity> suspendedActivities;


    [Header("App Settings")]
    public AppState appState;
    public string appId => appState.GetInstanceID().ToString();
    public bool Rendering => appState.IsRendering;

    public System.Action<bool> OnRenderStateChanged;

    void OnValidate()
    {
        // Test that activities does not contain more than one of each type.
        foreach(var entry in activities)
        {
            if (entry.activity != null)
            {
                entry.activity.appState = appState;
                entry.activity.appRuntime = this;
            }
        }
    }

    void Reset()
    {
        // Test that Content layer is there. If not, create one.
        if (transform.Find("ContentRoot") == null)
        {
            GameObject empty = new GameObject("ContentRoot");
            empty.transform.parent = this.transform;
        }
    }

    protected virtual void Awake()
    {
        runningActivities = new Dictionary<Guid, BaseAppActivity>();
        suspendedActivities = new Dictionary<Guid, BaseAppActivity>();

        // Add new app to app registry.
        AppRegistry.AddApp(this);

        foreach(ActivityEntry entry in activities)
        {
            entry.activity.appState = appState;
        }
    }

    void OnEnable()
    {
        appState.AddListener(this);
    }

    void OnDisable()
    {
        appState.RemoveListener(this);
    }

    public virtual void AppStart() {}
    public virtual void AppQuit() {}

    public virtual void RenderStateChanged(bool toValue)
    {
        OnRenderStateChanged?.Invoke(toValue);
    }

    public void ToggleStartOrSuspend()
    {
        appState.ToggleStartOrSuspend();
    }

    public virtual void BeforeFirstActivityStart() {}
    public virtual void AfterFirstActivityStart() {}

    public void OnActivityStart(ActivityEventData eventData)
    {
        // Launch activity
        foreach(ActivityEntry entry in activities)
        {
            if (entry.activityType == eventData.ActivityType)
            {
                // Resume existing activity
                if (suspendedActivities.ContainsKey(eventData.ActivityID))
                {
                    BaseAppActivity existingActivity = suspendedActivities[eventData.ActivityID];
                    existingActivity.StartActivity(eventData.StartContext);
                    suspendedActivities.Remove(eventData.ActivityID);
                    runningActivities.Add(eventData.ActivityID, existingActivity);
                }
                else
                {
                    // Debug.Log($"Running activities count: {runningActivities.Count()}");
                    if (runningActivities.Count() == 0)
                    {
                        // Debug.Log("Calling BeforeFirstActivityStart");
                        BeforeFirstActivityStart();
                    }

                    // Initialize activity state
                    GameObject newClone = GameObject.Instantiate(entry.activity.gameObject, transform);
                    BaseAppActivity newActivity = newClone.GetComponent<BaseAppActivity>();
                    newActivity.appState = appState;
                    newActivity.activityID = eventData.ActivityID;
                    newActivity.StartActivity(eventData.StartContext);
                    runningActivities.Add(eventData.ActivityID, newActivity);

                    // Debug.Log($"Running activities count: {runningActivities.Count()}");
                    if (runningActivities.Count() == 1)
                    {
                        AfterFirstActivityStart();
                    }
                }
            }
        }
    }

    public void OnActivityStop(ActivityEventData eventData)
    {
        System.Guid activityID = eventData.ActivityID;

        // Check that we own this activity. (should not happen)
        if (!runningActivities.ContainsKey(activityID))
        { 
            throw new ArgumentException("ActivityEventData contained invalid ActivityID");
        }
        
        // Stop activity.
        BaseAppActivity activity = runningActivities[activityID];
        activity.StopActivity(eventData.StopContext);
        // GameObject.Destroy(activity.gameObject);

        // Remove activity ID.
        runningActivities.Remove(activityID);
        suspendedActivities.Add(activityID, activity);
    }

    public void OnStateChanged(ExecutionState executionState) {}

    public void OnMessage(string methodName, object value, SendMessageOptions options)
    {
        this.gameObject.SendMessage(methodName, value, options);
        List<Guid> runningActivityIDs = runningActivities.Keys.ToList();
        foreach(Guid id in runningActivityIDs)
        {
            runningActivities[id].SendMessage(methodName, value, options);
        }
    }
}
