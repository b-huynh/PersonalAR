using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

using g3;

using Recaug;

[RequireComponent(typeof(WorldAnchorManager))]
public class AnnotationModeManager : Singleton<AnnotationModeManager>, IMixedRealityPointerHandler
{
    public GameObject objectAnnotationPrefab;

    public GameObject prefab1x1;

    private bool primed = false;
    private bool readyTwoPointAnnotation;
    private List<Vector3> pointCache;
    private List<double> pointWeights;

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);

    }

    // Start is called before the first frame update
    void Start()
    {
        pointCache = new List<Vector3>();
        pointWeights = new List<double>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pointCache.Clear();
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            CreateBoxFromPoints(pointCache);
            // pointCache.Clear();
        }
    }

    void FixedUpdate()
    {
        if (pointCache.Count >= 2)
        {
            var orientedBoundingBox = GetOrientedBox3(pointCache);

            var center = (Vector3)orientedBoundingBox.Box.Center;

            var axisX = (Vector3)orientedBoundingBox.Box.AxisX;
            var axisY = (Vector3)orientedBoundingBox.Box.AxisY;
            var axisZ = (Vector3)orientedBoundingBox.Box.AxisZ;
            var extends = (Vector3)orientedBoundingBox.Box.Extent;

            var A = center - extends.z * axisZ - extends.x * axisX - axisY * extends.y;
            var B = center - extends.z * axisZ + extends.x * axisX - axisY * extends.y;
            var C = center - extends.z * axisZ + extends.x * axisX + axisY * extends.y;
            var D = center - extends.z * axisZ - extends.x * axisX + axisY * extends.y;

            var E = center + extends.z * axisZ - extends.x * axisX - axisY * extends.y;
            var F = center + extends.z * axisZ + extends.x * axisX - axisY * extends.y;
            var G = center + extends.z * axisZ + extends.x * axisX + axisY * extends.y;
            var H = center + extends.z * axisZ - extends.x * axisX + axisY * extends.y;

            // And finally visualize it
            Debug.DrawLine(A, B);
            Debug.DrawLine(B, C);
            Debug.DrawLine(C, D);
            Debug.DrawLine(D, A);

            Debug.DrawLine(E, F);
            Debug.DrawLine(F, G);
            Debug.DrawLine(G, H);
            Debug.DrawLine(H, E);

            Debug.DrawLine(A, E);
            Debug.DrawLine(B, F);
            Debug.DrawLine(D, H);
            Debug.DrawLine(C, G);
        }
    }

    public void Prime()
    {   
        primed = true;
    }

    public void ReadyTwoPointAnnotation()
    {
        readyTwoPointAnnotation = true;
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {

        // if (eventData.Pointer.Result.CurrentPointerTarget.layer != 
        //     LayerMask.NameToLayer("Spatial Awareness"))
        // {
        //     Debug.Log
        // }
        //     return;
        
        // Debug.Log("Hit object in layer: " + LayerMask.LayerToName(eventData.Pointer.Result.CurrentPointerTarget.layer));
        if (primed)
        {
            Vector3 hitPoint = eventData.Pointer.Result.Details.Point;
            GameObject.Instantiate(objectAnnotationPrefab, hitPoint, Quaternion.identity, transform);
            eventData.Use();
        }
        primed = false;
    
        // if (readyTwoPointAnnotation)
        if (Input.GetKey(KeyCode.P))
        {
            Vector3 hitPoint = eventData.Pointer.Result.Details.Point;

            var point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            point.transform.position = hitPoint;
            point.transform.parent = this.transform;
            point.GetComponent<Renderer>().material.color = Color.red;

            pointCache.Add(hitPoint);

            if (pointCache.Count <=2)
            {
                pointWeights.Add(1.0f);
            }
            else
            {
                pointWeights.Add(0.25f);
            }

            // if (pointCache.Count >=2)
            // {
            //     CreateTwoPointAnnotation(pointCache[0], pointCache[1]);
            //     pointCache.Clear();
            //     readyTwoPointAnnotation = false;
            // }
        }
    }

    private void CreateTwoPointAnnotation(Vector3 point1, Vector3 point2)
    {
        // Vector3 center = (point1 + point2) / 2.0f; 
        // Vector3 size = point1 - point2;
        // Bounds bounds = new Bounds(center, size);
        // GameObject bbox = GameObject.Instantiate(objectAnnotationPrefab, center, Quaternion.identity, transform);
        // bbox.GetComponent<Collider>().center = center;

        var points3d = new Vector3d[] {point1, point2};
        var orientedBbox = new ContOrientedBox3(points3d);

        var center = (Vector3)orientedBbox.Box.Center;
        var forward = (Vector3)orientedBbox.Box.AxisZ;
        var upward = (Vector3)orientedBbox.Box.AxisY;
        var rotation = Quaternion.LookRotation(forward, upward);

        var extents = (Vector3)orientedBbox.Box.Extent;

        GameObject bbox = GameObject.Instantiate(prefab1x1, center, Quaternion.identity, transform);
        bbox.transform.localScale = extents;
    }

    private void CreateBoxFromPoints(List<Vector3> points)
    {
        var orientedBbox = GetOrientedBox3(points);

        var center = (Vector3)orientedBbox.Box.Center;
        var forward = (Vector3)orientedBbox.Box.AxisZ;
        var upward = (Vector3)orientedBbox.Box.AxisY;
        var rotation = Quaternion.LookRotation(forward, upward);

        var extents = (Vector3)orientedBbox.Box.Extent;

        Debug.LogFormat(
        @"Center:   {0}
          Forward:  {1}
          Upward:   {2}
          Rotation: {3}
          Extents:  {4}",
          center, forward, upward, rotation, extents);

        GameObject bbox = GameObject.Instantiate(prefab1x1, center, Quaternion.identity, transform);
        bbox.transform.localScale = extents * 2.0f;
    }

    private ContOrientedBox3 GetOrientedBox3(List<Vector3> points)
    {
        var points3d = new Vector3d[points.Count];
        for (int i = 0; i < points.Count; ++i)
        {
            points3d[i] = points[i];
        }

        return new ContOrientedBox3(points3d, pointWeights);
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) {}
}
