using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HUDActivity : BaseAppActivity
{
    [Header("HUD Activity")]
    public GameObject entityToLaunch;

    protected GameObject cachedEntity;

    void OnDestroy()
    {
        Destroy(cachedEntity);
    }

    public override void StartActivity(ExecutionContext executionContext)
    {
        if (cachedEntity != null)
        {
            Transform launchPoint = LaunchPoints.HUDNear.GetLaunchPoint();
            cachedEntity.transform.SetPositionAndRotation(launchPoint.position, launchPoint.rotation);
            cachedEntity.SetActive(true);
        }
        else
        {
            // Instantiate for the first time
            Transform launchPoint = LaunchPoints.HUDNear.GetLaunchPoint();
            cachedEntity = GameObject.Instantiate(entityToLaunch, transform);
            cachedEntity.transform.SetPositionAndRotation(launchPoint.position, launchPoint.rotation);
        }
    }

    public override void StopActivity(ExecutionContext executionContext)
    {
        if (cachedEntity != null) 
        {
            cachedEntity.SetActive(false);
            // Destroy(cachedEntity);
            // cachedEntity = null;
        }
    }
}