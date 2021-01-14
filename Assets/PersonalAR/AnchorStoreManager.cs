using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.WindowsMR;
 
public class AnchorStoreManager : MonoBehaviour
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
 
            if (AnchorStore != null)
            {
                Debug.Log($"[{GetType()}] Anchor store initialized.");
            }
 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnchorStore)));
        }
    }
 
    private async void Start()
    {
        AnchorPointsSubsystem = CreateReferencePointSubSystem();
        AnchorStore = await _anchorPointSubsystem.TryGetAnchorStoreAsync();
    }
 
    private void OnDestroy()
    {
        if (AnchorPointsSubsystem != null)
        {
            AnchorPointsSubsystem.Stop();
        }
        if (AnchorStore != null)
        {
            AnchorStore.Dispose();
        }
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
}