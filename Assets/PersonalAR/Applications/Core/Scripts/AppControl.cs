using System;
using UnityEngine;

public class AppControl : MonoBehaviour
{
    public AppState appState;
    public ActivityType activityToLaunch;

    [Header("Optional Event Data")]
    public AnchorableObject anchor;

    private Guid cachedGuid;

    void OnEnable()
    {
        cachedGuid = Guid.Empty;
    }

    public void StartActivity()
    {
        var executionContext = new ExecutionContext(gameObject)
        {
            Anchor = anchor
        };
        cachedGuid = appState.StartActivity(activityToLaunch, executionContext);
    }

    public void StopActivity()
    {
        var executionContext = new ExecutionContext(gameObject)
        {
            Anchor = anchor
        };
        appState.StopActivity(cachedGuid, executionContext);
        cachedGuid = Guid.Empty;
    }

    public void ToggleActivity()
    {
        if (cachedGuid.Equals(Guid.Empty))
        {
            StartActivity();
        }
        else
        {
            StopActivity();
        }
    }
}