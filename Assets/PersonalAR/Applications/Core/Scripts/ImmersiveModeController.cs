using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoolVariableListener))]
public class ImmersiveModeController : Singleton<ImmersiveModeController>
{
    public AppList appList;

    public static bool IsImmersiveMode;
    public static bool IsLayerableMode
    {
        get 
        { 
            return !IsImmersiveMode; 
        }
    }

    void Awake()
    {
        GetComponent<BoolVariableListener>().OnToggle.AddListener(OnImmersiveModeToggle);
    }

    // Start is called before the first frame update
    void Start()
    {
        // allKnownApps = new List<AppState>();
        
        // Get all known apps on start.        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintLastApp()
    {
        Debug.Log(AppState.lastAppStarted?.name);
    }

    public void StopAllApps()
    {
        foreach(AppState app in appList.appList)
        {
            ExecutionContext executionContext = new ExecutionContext(gameObject);
            app.StopAllActivities(executionContext);
        }
    }

    public void StopAllAppsExceptLastOpened()
    {
        foreach(AppState app in appList.appList)
        {
            if (app != AppState.lastAppStarted)
            {
                ExecutionContext executionContext = new ExecutionContext(gameObject);
                app.StopAllActivities(executionContext);
            }
        }
    }

    private void OnImmersiveModeToggle(bool value)
    {
        IsImmersiveMode = value;
        if (IsImmersiveMode)
        {
            StopAllAppsExceptLastOpened();
        }
    }
}
