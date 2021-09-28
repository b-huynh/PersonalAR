using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.WindowsMR;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

 using Microsoft.MixedReality.Toolkit.Extensions;

// public class AnchorServiceV2 : BaseExtensionService, IAnchorService, IMixedRealityExtensionService
public class AnchorServiceV2 : MonoBehaviour
{
    ARAnchorManager anchorManager;

    public XRAnchorSubsystem AnchorPointsSubsystem
    {
        get => anchorManager?.subsystem;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool RegisterAnchor(string anchorName, Vector3 position)
    {
        var trackables = anchorManager.trackables;

        foreach(var tracked in trackables)
        {
            tracked.trackableId.ToString();
        }

        ARAnchor referencePoint = anchorManager.AddAnchor(new Pose(position, Quaternion.identity));

        if (referencePoint.IsNull()) { return false; }
    
        anchorManager.StartCoroutine(PersistAnchor(referencePoint));

        return referencePoint != null;
    }

    private IEnumerator PersistAnchor(ARAnchor referencePoint)
    {
        yield return new WaitUntil(() => referencePoint.pending == false);
    }
}
