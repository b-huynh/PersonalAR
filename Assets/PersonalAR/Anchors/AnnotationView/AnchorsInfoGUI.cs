using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AnchorsInfoGUI : MonoBehaviour
{
    public bool isVerbose;
    private TextMeshProUGUI _textMesh;
    private IAnchorService _anchorService;

    private Dictionary<TrackableId, string> _anchorNameMap;

    // Start is called before the first frame update
    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _anchorNameMap = new Dictionary<TrackableId, string>();

        if (!MixedRealityServiceRegistry.TryGetService<IAnchorService>(out _anchorService))
        {
            Debug.LogError($"Could not get Anchor Service");
        }
        else
        {
            _anchorService.OnRegistered += OnAnchorRegistered;
        }
    }

    private void OnAnchorRegistered(AnchorableObject anchor)
    {
        if (!_anchorNameMap.ContainsKey(anchor._xrAnchorId))
        {
            _anchorNameMap.Add(anchor._xrAnchorId, anchor.WorldAnchorName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _textMesh.text = GetAnchorsInfoText(isVerbose);
    }

    public string GetAnchorsInfoText(bool verbose = false)
    {
        if (_anchorService.AnchorStore == null)
        {
            return "Anchor store not initialized.\n";
        }

        string infoText = $"<size=20><b>{_anchorService.AnchorCount} Placed Objects</b></size>\n";
        if (verbose)
        {
            foreach(string anchorName in _anchorService.AnchorNames)
            {
                AnchorableObject anchor = _anchorService.GetAnchor(anchorName);
                string debugLabel = anchor._debugLabel.text.Replace("\r\n", ", ");
                infoText += $"{debugLabel} \n";
            }
        }
        else
        {
            foreach(string anchorName in _anchorService.AnchorNames)
            {
                infoText += $"{anchorName}\n";
            }
        }
        return infoText;
    }

    private string GetAnchorChangesInfoText(NativeArray<XRReferencePoint> changes)
    {
        string infoText = string.Empty;
        foreach (XRReferencePoint anchor in changes)
        {
            // If we have a user-defined name for this anchor, use it, otherewise use TrackableId
            string displayText = _anchorNameMap.ContainsKey(anchor.trackableId) ? 
                _anchorNameMap[anchor.trackableId] : anchor.trackableId.ToString();
            displayText = displayText.TruncateEllipsis(10);

            string trackingState = anchor.trackingState.ToString();
            string trackingPos = anchor.pose.position.ToString("F3");

            infoText += $"{displayText} | {trackingState} | {trackingPos}\n";
        }
        return infoText;
    }

    private string GetAnchorChangesInfoText(NativeArray<TrackableId> changes)
    {
        string infoText = string.Empty;
        foreach (TrackableId trackable in changes)
        {
            // If we have a user-defined name for this anchor, use it, otherewise use TrackableId
            string displayText = _anchorNameMap.ContainsKey(trackable) ? 
                _anchorNameMap[trackable] : trackable.ToString();
            displayText = displayText.TruncateEllipsis(10);

            infoText += $"{displayText}\n";
        }
        return infoText;
    }

    public string GetAnchorsInfoTextV2(bool verbose = false)
    {
        if (_anchorService.AnchorStore == null)
        {
            return "Anchor store not initialized.\n";
        }

        string infoText = $"<size=20><b>{_anchorService.AnchorCount} Placed Objects</b></size>\n";
        if (verbose)
        {
            TrackableChanges<XRReferencePoint> anchorChanges = _anchorService.AnchorPointsSubsystem.GetChanges(Allocator.Temp);

            // Log changes for added/updated/removed
            infoText += $"ADDED {anchorChanges.added.Length}\n{new string('_', 20)}\n";
            infoText += GetAnchorChangesInfoText(anchorChanges.added);
            infoText += $"UPDATED {anchorChanges.updated.Length}\n{new string('_', 20)}\n";
            infoText += GetAnchorChangesInfoText(anchorChanges.updated);
            infoText += $"REMOVED {anchorChanges.removed.Length}\n{new string('_', 20)}\n";
            infoText += GetAnchorChangesInfoText(anchorChanges.removed);
        }
        else
        {
            foreach(string anchorName in _anchorService.AnchorNames)
            {
                infoText += $"{anchorName}\n";
            }
        }
        return infoText;
    }
}
