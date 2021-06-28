using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public enum RenderState { Off, Full, Partial, Background, Unknown }

[CreateAssetMenu]
public class AppState : ScriptableObject
{
    // Application meta info
    public string appName;
    public string appDesc;
    public Material appLogo;


    // Application save data
    public string appDataFile;


    // Application execution state
    public bool IsInitialized { get; private set; }
    public bool IsRendering { get; private set; }
    // public RenderState RenderState
    // {
    //     get
    //     {
    //         if (RunningActivities.Count == 0)
    //         {
    //             return RenderState.Off;
    //         }

    //         if (RunningActivities.
    //     }
    //     private set {}
    // }
    public Dictionary<Guid, ActivityType> RunningActivities = new Dictionary<Guid, ActivityType>();


    // Application state handlers
    private List<IAppStateListener> listeners = new List<IAppStateListener>();

    void Awake()
    {
        IsInitialized = false;
        IsRendering = false;
    }

    // ***** VIEW / LISTENER INTERFACE *****
    public void AddListener(IAppStateListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(IAppStateListener listener)
    {
        listeners.Remove(listener);
    }

    private void AppStart()
    {
        if (IsInitialized == false)
        {
            foreach(var listener in listeners)
            {
                listener.AppStart();
            }
            // OnAppStart.Invoke();
            IsInitialized = true;
        }
    }

    private void AppQuit()
    {
        if (IsInitialized == true)
        {
            foreach(var listener in listeners)
            {
                listener.AppQuit();
            }   
            // OnAppQuit.Invoke();
            IsInitialized = false;
        }
    }

    // ***** CONTROLLER INTERFACE *****
    public void Save(object obj)
    {
        if (!string.IsNullOrEmpty(appDataFile))
        {
            string json = JsonUtility.ToJson(obj, true);
            string filePath = Path.Combine(Application.persistentDataPath, appDataFile);
            using(StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(json);
            }
        }
        else
        {
            ARDebug.Log("Cannot save file. No file name specified");
        }
    }

    public T Load<T>()
    {
        // If save file not specified, return default object. Can't be saved until file specified.
        if (string.IsNullOrEmpty(appDataFile))
        {
            return default(T);
        }

        // If file specified but doesn't exist, return new default object. File will be created next save.
        string filePath = Path.Combine(Application.persistentDataPath, appDataFile);
        if (!File.Exists(filePath))
        {
            return default(T);
        }

        // If file specified and exists, try to deserialize object from json.
        T obj = default(T);
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();
                obj = JsonUtility.FromJson<T>(json);
            }
        }
        catch (System.Exception ex)
        {
            ARDebug.LogError($"Failed to load object in file {filePath}. Exception {ex}");
        }
        return obj;
    }

    private void SetRenderState(bool value)
    {
        if (IsRendering != value)
        {
            IsRendering = value;
            foreach(var listener in listeners)
            {
                listener.RenderStateChanged(IsRendering);
            }
            // OnRenderStateChanged.Invoke(IsRendering);
        }
    }

    public void ToggleStartOrSuspend()
    {
        // If app has not yet been initialized, call AppStart and start rendering.
        if (IsInitialized == false)
        {
            AppStart();
            SetRenderState(true);
        }
        else
        {
            // If already initialized, toggle the rendering state.
            SetRenderState(!IsRendering);
        }
    }

    public Guid StartActivity(ActivityType activityType, ExecutionContext executionContext) 
    {
        // Update internal state
        Guid activityID = System.Guid.NewGuid();
        RunningActivities.Add(activityID, activityType);

        // Create Event Data
        ActivityEventData eventData = new ActivityEventData
        {
            EventTime = System.DateTime.Now,
            ActivityID = activityID,
            ActivityType = activityType,
            StartContext = executionContext
        };

        // Invoke listeners / view updates
        foreach(var listener in listeners)
        {
            listener.OnActivityStart(eventData);
        }

        return activityID;
    }

    public void StopActivity(Guid activityID, ExecutionContext executionContext)
    {
        if (RunningActivities.ContainsKey(activityID) == false) { return; }

        // Create Event Data
        ActivityEventData eventData = new ActivityEventData
        {
            EventTime = System.DateTime.Now,
            ActivityID = activityID,
            ActivityType = RunningActivities[activityID],
            StopContext = executionContext,
        };

        // Update internal state
        RunningActivities.Remove(activityID);

        // Invoke listeners / view updates
        foreach (var listener in listeners)
        {
            listener.OnActivityStop(eventData);
        }
    }
}
