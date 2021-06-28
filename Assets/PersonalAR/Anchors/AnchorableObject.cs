using System;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions; 

[System.Serializable]
public class AnchorableObject : MonoBehaviour
{
    public bool LoadAnchorOnStart = false;
 
    [SerializeField, Tooltip("World anchors must have a unique name. Enter a descriptive name if you like.")]
    private string _worldAnchorName;
    public string WorldAnchorName
    {
        get => _worldAnchorName;
        set => _worldAnchorName = value;
    }

    [SerializeField]
    public TMPro.TMP_Text _debugLabel;
 
    [Space(10)]
    public UnityEvent OnAnchorLoaded;
    public bool IsTrackingPosition { get; private set; }
 
    public TrackableId _xrAnchorId { get; private set; }
    // public AnchorStoreManager _anchorStoreManager;

    public IAnchorService _anchorService;
 
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_worldAnchorName))
        {
            _worldAnchorName = Guid.NewGuid().ToString();
        }
    }

    private void Awake()
    {
        // _anchorStoreManager = AnchorStoreManager.Instance;
        if (!MixedRealityServiceRegistry.TryGetService<IAnchorService>(out _anchorService))
        {
            ARDebug.LogError($"Could not get Anchor Service");
        }
    }

    private void Start()
    {

        if (_anchorService != null)
        {

            _anchorService.PropertyChanged += AnchorStore_PropertyChanged;
//  #if WINDOWS_UWP
            if (LoadAnchorOnStart && _anchorService.AnchorStore != null)
            {
                LoadAnchor();
            }
// #endif
        }
        else
        {
            LogDebugMessage($"No {nameof(IAnchorService)} present in scene.", true);
        }

        IsTrackingPosition = false;
    }

    private void LateUpdate()
    {
        UpdateAnchorPose();
    }
 
    public bool SaveAnchor(bool overwrite = true)
    {
        // if (_anchorService?.AnchorPointsSubsystem == null || _anchorService?.AnchorStore == null)
        if (!_anchorService.AnchorStoreInitialized)
        {
            LogDebugMessage($"Can't save anchor {_worldAnchorName}: reference point subsystem or anchor store have not been initialized.", true);
            return false;
        }
 
        XRReferencePoint anchor;
 
        if (overwrite)
        {
            // Delete the current anchor in the store, so we can persist the new position.
            _anchorService.AnchorStore.UnpersistAnchor(_worldAnchorName);
        }
 
        // Attempt to save the anchor.
        if (_anchorService.AnchorPointsSubsystem.TryAddReferencePoint(new Pose(transform.position, transform.rotation), out anchor))
        {
            if (_anchorService.AnchorStore.TryPersistAnchor(anchor.trackableId, _worldAnchorName))
            {
                _xrAnchorId = anchor.trackableId;
                LogDebugMessage($"Successfully saved anchor {_worldAnchorName}.");
                PositionFromAnchor(anchor);
                return true;
            }
            else
            {
                LogDebugMessage($"Failed to save anchor {_worldAnchorName}.", true);
            }
        }
        else
        {
            LogDebugMessage($"Failed to add reference point for anchor {_worldAnchorName}.", true);
        }
 
        return false;
    }
 
    public bool LoadAnchor()
    {
        // if (_anchorService?.AnchorPointsSubsystem == null || _anchorService?.AnchorStore == null)
        if (!_anchorService.AnchorStoreInitialized)
        {
            LogDebugMessage($"Can't load anchor {_worldAnchorName}: reference point subsystem or anchor store have not been initialized.", true);
            return false;
        }
 
        // Retrieve the trackable id from the anchor store.
        TrackableId trackableId = _anchorService.AnchorStore.LoadAnchor(_worldAnchorName);
 
        // Look for the matching anchor in the anchor point subsystem.
        TrackableChanges<XRReferencePoint> referencePointChanges = _anchorService.AnchorPointsSubsystem.GetChanges(Allocator.Temp);
        // ARDebug.Log($"[LoadAnchor] GetChanges added {referencePointChanges.added.Length}");
        foreach (XRReferencePoint anchor in referencePointChanges.added)
        {
            if (anchor.trackableId == trackableId)
            {
                _xrAnchorId = anchor.trackableId;
                PositionFromAnchor(anchor);
                OnAnchorLoaded.Invoke();

                // ARDebug.Log($"[LoadAnchor] Added {_worldAnchorName} at {anchor.pose.position.ToString("F3")}");
                LogDebugMessage($"Found anchor {_worldAnchorName} in added reference points.");
                
                return true;
            }
        }
 
        LogDebugMessage($"Did not find anchor {_worldAnchorName} in reference points subsystem after XRAnchorStore load.", true);
        return false;
    }

    // private bool MatchIdWithChanges(TrackableId trackableId, NativeArray<XRReferencePoint> trackedChanges)
    // {
    //     foreach (XRReferencePoint anchor in trackedChanges)
    //     {
    //         ARDebug.Log($"Found anchor {anchor}, id {trackableId}, position {anchor.pose.position}");
    //         if (anchor.trackableId == trackableId)
    //         {
    //             LogDebugMessage($"Found anchor {_worldAnchorName} in added reference points.");
    //             return true;
    //         }
    //     }
    //     return false;
    // }
 
    // public void ClearAnchor()
    // {
    //     _xrAnchorId = default;
 
    //     if (_anchorService.AnchorStore != null)
    //     {
    //         _anchorService.AnchorStore.UnpersistAnchor(_worldAnchorName);
    //     }
    // }
 
    private void UpdateAnchorPose()
    {
#if UNITY_EDITOR
        // PositionFromAnchor is not called in editor so just set tracking position status to always true in enditor.
        IsTrackingPosition = true;
#endif

        // if (_xrAnchorId == default || _anchorService?.AnchorPointsSubsystem == null)
        if (_xrAnchorId == default || !_anchorService.AnchorStoreInitialized)
        {
            return;
        }
 
        TrackableChanges<XRReferencePoint> anchorChanges = _anchorService.AnchorPointsSubsystem.GetChanges(Allocator.Temp);

        // TESTING.
        foreach (XRReferencePoint anchor in anchorChanges.added)
        {
            if (anchor.trackableId == _xrAnchorId)
            {
                PositionFromAnchor(anchor);
                break;
            }
        }

        foreach (XRReferencePoint anchor in anchorChanges.updated)
        {
            if (anchor.trackableId == _xrAnchorId)
            {
                PositionFromAnchor(anchor);
                break;
            }
        }
    }
 
    private void PositionFromAnchor(XRReferencePoint anchor)
    {
        if (_debugLabel)
        {
            _debugLabel.text = $"{_worldAnchorName} | status: {anchor.trackingState}\r\n{anchor.pose.position}\r\n{anchor.pose.rotation}";
        }

        if (anchor.pose.position == Vector3.zero)
        {
            IsTrackingPosition = false;
        }
        else
        {
            IsTrackingPosition = true;
            transform.position = anchor.pose.position;
            transform.rotation = anchor.pose.rotation;
        }
    }
 
    private void AnchorStore_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // if (LoadAnchorOnStart && e.PropertyName == nameof(_anchorService.AnchorStore) && _anchorService.AnchorStore != null)
        if (LoadAnchorOnStart && e.PropertyName == nameof(_anchorService.AnchorStore) && _anchorService.AnchorStoreInitialized)
        {
            LoadAnchor();
        }
    }
 
    private void LogDebugMessage(string message, bool isWarning = false)
    {
        if (_debugLabel)
        {
            _debugLabel.text = message;
        }
 
        if (isWarning)
        {
            ARDebug.LogWarning($"[{GetType()}] {message}");
        }
        else
        {
            ARDebug.Log($"[{GetType()}] {message}");
        }
    }
}