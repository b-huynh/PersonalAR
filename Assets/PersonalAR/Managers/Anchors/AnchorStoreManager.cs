using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.WindowsMR;
 
using Recaug;

public class AnchorStoreManager : Singleton<AnchorStoreManager>
{
    public event PropertyChangedEventHandler PropertyChanged;
 
    private XRReferencePointSubsystem _anchorPointSubsystem;
    public XRReferencePointSubsystem AnchorPointsSubsystem
    {
        get { return _anchorPointSubsystem; }
        private set
        {
            _anchorPointSubsystem = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnchorPointsSubsystem)));
        }
    }

    private XRAnchorStore _anchorStore;
    public XRAnchorStore AnchorStore
    {
        get { return _anchorStore; }
        private set
        {
            _anchorStore = value;
 
            if (AnchorStore == null)
            {
                Debug.Log($"[{GetType()}] Anchor store not initialized.");
            }

            if (AnchorPointsSubsystem == null)
            {
                Debug.Log($"[{GetType()}] Reference Point Subsystem not initialized.");
            }

            if (AnchorStore != null)
            {
                Debug.Log($"[{GetType()}] Anchor store initialized.");
            }
 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnchorStore)));
        }
    }
 
    private string _anchorStoreDebugText;
    public string AnchorStoreDebugText
    {
        get { return _anchorStoreDebugText; }
        private set
        {
            _anchorStoreDebugText = value;
            Debug.Log("Anchor Store Debug Values Set");
        }
    }

    private Dictionary<string, XRReferencePoint> _anchors;
    public Dictionary<string, XRReferencePoint> Anchors
    {
        get { return _anchors; }
        private set
        {
            _anchors = value;
        }
    }

    private List<GameObject> _debugAnchors;

    private async void Start()
    {
        AnchorPointsSubsystem = CreateReferencePointSubSystem();
        AnchorStore = await _anchorPointSubsystem.TryGetAnchorStoreAsync();

        Anchors = new Dictionary<string, XRReferencePoint>();
        _debugAnchors = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CreateDebugAnchor("anchor1", new Pose(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity));
        }
    }

    protected override void OnDestroy()
    {
        if (AnchorPointsSubsystem != null)
        {
            AnchorPointsSubsystem.Stop();
        }
        if (AnchorStore != null)
        {
            AnchorStore.Dispose();
        }
        
        base.OnDestroy();
    }
 
    private XRReferencePointSubsystem CreateReferencePointSubSystem()
    {
        List<XRReferencePointSubsystemDescriptor> rpSubSystemsDescriptors = new List<XRReferencePointSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors(rpSubSystemsDescriptors);
 
        string descriptors = "";
        foreach (var descriptor in rpSubSystemsDescriptors)
        {
            descriptors += $"{descriptor.id} {descriptor.subsystemImplementationType}\r\n";
        }
 
        Debug.Log($"[{GetType()}] {rpSubSystemsDescriptors.Count} reference point subsystem descriptors:\r\n{descriptors}");
 
        XRReferencePointSubsystem rpSubSystem = null;
        if (rpSubSystemsDescriptors.Count > 0)
        {
            rpSubSystem = rpSubSystemsDescriptors[0].Create();
            rpSubSystem.Start();
        }
 
        return rpSubSystem;
    }

    public void LoadPersistedAnchors()
    {
        Dictionary<TrackableId, string> nameMap = new Dictionary<TrackableId, string>();
        Dictionary<TrackableId, string> addedStates = new Dictionary<TrackableId, string>();
        Dictionary<TrackableId, string> updateStates = new Dictionary<TrackableId, string>();

        if (AnchorStore == null || AnchorPointsSubsystem == null) return;

        var anchorNames = AnchorStore.PersistedAnchorNames;

        foreach(string name in anchorNames)
        {
            TrackableId trackableId = AnchorStore.LoadAnchor(name);
            nameMap.Add(trackableId, name);
        }

        TrackableChanges<XRReferencePoint> changes = AnchorPointsSubsystem.GetChanges(Allocator.Temp);
        foreach(XRReferencePoint anchor in changes.added)
        {
            if (nameMap.Keys.Contains(anchor.trackableId))
            {
                addedStates.Add(anchor.trackableId, $"added {anchor.pose.position.ToString("F2")} {anchor.trackingState}");
                string anchorName = nameMap[anchor.trackableId];
                Anchors.Add(anchorName, anchor);
                CreateDebugAnchor(anchorName, anchor.pose);
            }
        }

        string debugText = "Persisted Anchors State:\n";
        foreach(TrackableId trackableId in nameMap.Keys)
        {
            string addedState = addedStates.ContainsKey(trackableId) ? addedStates[trackableId] : "no added state";
            string updateState = updateStates.ContainsKey(trackableId) ? updateStates[trackableId] : "no update state";
            debugText += $"{nameMap[trackableId].PadRight(10, ' ')} | {addedState}\n";
        }
        AnchorStoreDebugText = debugText;
    }

    public void ToggleDebugAnchors(bool value)
    {
        foreach(GameObject debugAnchor in _debugAnchors)
        {
            debugAnchor.SetActive(value);
        }
    }

    public void CreateDebugAnchor(string anchorName, Pose anchorPose)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Renderer>().material.color = Color.red;

        var text = new GameObject("Text");
        text.transform.parent = sphere.transform;
        text.transform.localPosition = Vector3.zero;

        var textMesh = text.AddComponent<TMPro.TextMeshPro>();
        textMesh.text = anchorName;
        textMesh.rectTransform.sizeDelta = new Vector2(2, 2);
        textMesh.fontSize = 5;

        sphere.transform.position = anchorPose.position;
        sphere.transform.rotation = anchorPose.rotation;
        sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        _debugAnchors.Add(sphere);
    }
}