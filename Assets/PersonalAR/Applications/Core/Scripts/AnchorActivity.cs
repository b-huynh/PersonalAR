using System;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

[Serializable]
public class AnchorActivity : BaseAppActivity
{
    [Header("Anchor Activity")]
    public GameObject entityToLaunch;

    protected IAnchorService anchorService;
    protected GameObject cachedEntity;

    private List<string> defaultAnchorNames = new List<string>()
    {
        "couch",
        "desk"
    };
    private List<string> runtimeAnchorNames;

    public virtual void Start()
    {
        // Check if appState has saved values for known anchors.
        runtimeAnchorNames = new List<string>(defaultAnchorNames);

        // Get AnchorService
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out anchorService))
        {
            // anchorService.OnRegistered += OnObjectRegistered;
        }
    }

    void OnDestroy()
    {
        Destroy(cachedEntity);
    }

    public override void StartActivity(ExecutionContext executionContext)
    {
        if (executionContext.Anchor == null)
        {
            return; // AnchorActivity requires an anchor...
        }

        cachedEntity = GameObject.Instantiate(entityToLaunch);
        cachedEntity.GetComponent<IAnchorable>().Anchor = executionContext.Anchor;
    }

    public override void StopActivity(ExecutionContext executionContext)
    {
        if (cachedEntity != null) 
        {
            Destroy(cachedEntity);
            cachedEntity = null;
        }
    }
 
}