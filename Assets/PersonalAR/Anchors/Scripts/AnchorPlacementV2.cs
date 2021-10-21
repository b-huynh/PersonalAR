using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class AnchorPlacementV2 : MonoBehaviour
{
    // Prefab to use for rendering anchor locations
    public GameObject anchorActorPrefab;
    private string _objectToPlace;
    private GameObject _rightHandPlacer;
    private GameObject _leftHandPlacer;
    private IAnchorService _anchorService;

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

        // Create objects that visualize anchor placement using the anchor prefab
        _rightHandPlacer = GetInertAnchorPrefab();
        _rightHandPlacer.SetActive(false);

        _leftHandPlacer = GetInertAnchorPrefab();
        _leftHandPlacer.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        // Do nothing if there is no requested object to be placed
        if (string.IsNullOrEmpty(_objectToPlace))
        {
            _rightHandPlacer.SetActive(false);
            _leftHandPlacer.SetActive(false);
            return;
        }

        // Set visualization positions for left and right hands depending on if a hand ray end point exists
        Vector3 rightEndPoint;
        bool rightRayExists = PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out rightEndPoint);
        _rightHandPlacer.SetActive(rightRayExists);
        if (rightRayExists)
        {
            _rightHandPlacer.transform.position = rightEndPoint + new Vector3(0, 0.01f, 0);
        }

        Vector3 leftEndPoint;
        bool leftRayExists = PointerUtils.TryGetHandRayEndPoint(Handedness.Left, out leftEndPoint);
        _leftHandPlacer.SetActive(leftRayExists);
        if (leftRayExists)
        {
            _leftHandPlacer.transform.position = leftEndPoint + new Vector3(0, 0.01f, 0);
        }
    }

    // Returns a new prefab clone with colliders disabled.
    private GameObject GetInertAnchorPrefab()
    {
        GameObject inertAnchor = GameObject.Instantiate(anchorActorPrefab, this.transform);
        foreach(Collider coll in inertAnchor.GetComponentsInChildren<Collider>())
        {
            coll.enabled = false;
        }
        return inertAnchor;
    }

    public void SetNextObject(string name)
    {
        _objectToPlace = name;

        _rightHandPlacer.GetComponentInChildren<AnnotationVisuals>().SetDisplayText(_objectToPlace);
        // _rightHandPlacer.SetActive(true);

        _leftHandPlacer.GetComponentInChildren<AnnotationVisuals>().SetDisplayText(_objectToPlace);
        // _leftHandPlacer.SetActive(true);
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
        }
        else if (leftPointers.Contains(eventData.Pointer) &&
                 _anchorService.RegisterAnchor(_objectToPlace, _leftHandPlacer.transform.position))
        {
            _objectToPlace = null;
        }
    }
}
