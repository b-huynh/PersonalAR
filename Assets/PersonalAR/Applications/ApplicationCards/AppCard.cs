using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

[RequireComponent(typeof(Anchorable))]
public class AppCard : BaseEntity
{
    public AppState AppState;

    [SerializeField] private TMPro.TextMeshPro _headerText;
    // [SerializeField] private MeshFilter _headerBackPlate;

    [SerializeField] private Collider _pinnableBounds;
    [SerializeField] private ObjectManipulator _objectManipulator;

    protected override void OnValidate()
    {
        if (_headerText != null && AppState != null)
        {
            _headerText.text = AppState.appName;
        }
    }

    void OnEnable()
    {
        GetComponent<Anchorable>().OnAnchorSet.AddListener(OnAnchorSet);
        GetComponent<Anchorable>().OnAnchorRemoved.AddListener(OnAnchorRemoved);
    }

    void OnDisable()
    {
        GetComponent<Anchorable>().OnAnchorSet.RemoveListener(OnAnchorSet);
        GetComponent<Anchorable>().OnAnchorRemoved.RemoveListener(OnAnchorRemoved);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAnchorSet(AnchorableObject anchor)
    {
        Transform slate = transform.Find("Slate").transform;
        // slate.Find("TitleBar").GetComponent<ObjectManipulator>().enabled = false;
        slate.localPosition = Vector3.zero;
        slate.localRotation = Quaternion.identity;

        _pinnableBounds.enabled = false;
        _objectManipulator.enabled = false;
    }

    public void OnAnchorRemoved(AnchorableObject anchor)
    {
        Transform slate = transform.Find("Slate").transform;
        // slate.Find("TitleBar").GetComponent<ObjectManipulator>().enabled = true;

        _pinnableBounds.enabled = true;
        _objectManipulator.enabled = true;
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        Debug.Log($"Pointer: {eventData.Pointer.PointerName}, Count: {eventData.Count}");
    }
}
