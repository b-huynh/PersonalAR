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
        Transform launchPoint = LaunchPoints.HUDNear.GetLaunchPoint();
        cachedEntity = GameObject.Instantiate(entityToLaunch, transform);
        cachedEntity.transform.SetPositionAndRotation(launchPoint.position, launchPoint.rotation);
    }
    public override void StopActivity(ExecutionContext executionContext)
    {
        
    }
}