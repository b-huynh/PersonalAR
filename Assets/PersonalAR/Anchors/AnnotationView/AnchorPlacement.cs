using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class AnchorPlacement : MonoBehaviour
{
    // Prefab to use for rendering anchor locations
    public GameObject anchorActorPrefab;
    private string _objectToPlace;
    private GameObject _rightHandPlacer;
    private GameObject _leftHandPlacer;
    private IAnchorService _anchorService;

    public UnityEvent OnObjectPlaced;

    void Awake()
    {
        if (!MixedRealityServiceRegistry.TryGetService<IAnchorService>(out _anchorService))
        {
            ARDebug.LogError($"Failed to get AnchorService");
        }

        if (!anchorActorPrefab)
        {
            ARDebug.LogError($"AnchorActorPrefab not assigned.");
        }

        _rightHandPlacer = GameObject.Instantiate(anchorActorPrefab, this.transform);
        _rightHandPlacer.SetActive(false);

        _leftHandPlacer = GameObject.Instantiate(anchorActorPrefab, this.transform);
        _leftHandPlacer.transform.Find("AnchorView").gameObject.SetActive(false);
        _leftHandPlacer.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // OPTIMIZE: Probably don't need to SetActive constantly.
    void Update()
    {
        if (_objectToPlace != null)
        {
            Vector3 rightEndPoint;
            if (PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out rightEndPoint))
            {
                _rightHandPlacer.SetActive(true);
                _rightHandPlacer.transform.position = rightEndPoint + new Vector3(0, 0.01f, 0);
            }
            else
            {
                _rightHandPlacer.SetActive(false);
            }

            Vector3 leftEndPoint;
            if (PointerUtils.TryGetHandRayEndPoint(Handedness.Left, out leftEndPoint))
            {
                _leftHandPlacer.SetActive(true);
                _leftHandPlacer.transform.position = leftEndPoint + new Vector3(0, 0.01f, 0);
            }
            else
            {
                _leftHandPlacer.SetActive(false);
            }
        }
        else
        {
            _rightHandPlacer.SetActive(false);
            _leftHandPlacer.SetActive(false);
        }
    }

    public void SetNextObject(string name)
    {
        _objectToPlace = name;

        _rightHandPlacer.GetComponentInChildren<AnnotationView>().SetDisplayText(_objectToPlace);
        _rightHandPlacer.transform.Find("AnchorView").gameObject.GetComponent<BoxCollider>().enabled = false;
        _rightHandPlacer.SetActive(true);

        _leftHandPlacer.GetComponentInChildren<AnnotationView>().SetDisplayText(_objectToPlace);
        _leftHandPlacer.transform.Find("AnchorView").gameObject.GetComponent<BoxCollider>().enabled = false;
        _leftHandPlacer.SetActive(true);
    }

    public void PlaceObject(MixedRealityPointerEventData eventData)
    {
        if (string.IsNullOrEmpty(_objectToPlace)) return;

        var rightPointers = PointerUtils.GetPointers<IMixedRealityPointer>(Handedness.Right, InputSourceType.Hand);
        var leftPointers = PointerUtils.GetPointers<IMixedRealityPointer>(Handedness.Left, InputSourceType.Hand);

        if (rightPointers.Contains(eventData.Pointer) &&
            _anchorService.RegisterAnchor(_objectToPlace, _rightHandPlacer.transform.position))
        {
            _objectToPlace = null;
            OnObjectPlaced?.Invoke();
        }
        else if (leftPointers.Contains(eventData.Pointer) &&
                 _anchorService.RegisterAnchor(_objectToPlace, _leftHandPlacer.transform.position))
        {
            _objectToPlace = null;
            OnObjectPlaced?.Invoke();
        }
    }

    // public void CreateAnchorIndicator(string anchorName, Pose anchorPose)
    // {
    //     var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //     sphere.GetComponent<Renderer>().material.color = Color.red;

    //     var text = new GameObject("Text");
    //     text.transform.parent = sphere.transform;
    //     text.transform.localPosition = Vector3.zero;

    //     var textMesh = text.AddComponent<TMPro.TextMeshPro>();
    //     textMesh.text = anchorName;
    //     textMesh.rectTransform.sizeDelta = new Vector2(2, 2);
    //     textMesh.fontSize = 5;

    //     sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    // }
}
