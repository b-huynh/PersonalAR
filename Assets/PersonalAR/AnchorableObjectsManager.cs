using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class AnchorableObjectsManager : MonoBehaviour
{
    public Dictionary<string, AnchorableObject> anchoredObjects;

    private string objectToPlace = null;

    public GameObject rightHandPlacer;
    public GameObject leftHandPlacer;

    // Start is called before the first frame update
    void Start()
    {
        anchoredObjects = new Dictionary<string, AnchorableObject>();

        rightHandPlacer.SetActive(false);
        leftHandPlacer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // rightHandPlacer.SetActive(objectToPlace != null);
        // leftHandPlacer.SetActive(objectToPlace != null);
        if (objectToPlace != null)
        {
            Vector3 rightEndPoint;
            if (PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out rightEndPoint))
            {
                rightHandPlacer.SetActive(true);
                rightHandPlacer.transform.position = rightEndPoint;
            }
            else
            {
                rightHandPlacer.SetActive(false);
            }

            Vector3 leftEndPoint;
            if (PointerUtils.TryGetHandRayEndPoint(Handedness.Left, out leftEndPoint))
            {
                leftHandPlacer.SetActive(true);
                leftHandPlacer.transform.position = leftEndPoint;
            }
            else
            {
                leftHandPlacer.SetActive(false);
            }
        }
    }

    public void SetNextObject(string name)
    {
        objectToPlace = name;
        
        rightHandPlacer.GetComponentInChildren<TextMesh>().text = objectToPlace;
        rightHandPlacer.SetActive(true);
        leftHandPlacer.GetComponentInChildren<TextMesh>().text = objectToPlace;
        leftHandPlacer.SetActive(true);
    }

    public void PlaceObject(MixedRealityPointerEventData eventData)
    {
        if (objectToPlace == null) return;

        var rightPointers = PointerUtils.GetPointers<IMixedRealityPointer>(Handedness.Right, InputSourceType.Hand);
        var leftPointers = PointerUtils.GetPointers<IMixedRealityPointer>(Handedness.Left, InputSourceType.Hand);

        if (rightPointers.Contains(eventData.Pointer))
        {
            CreateAnchoredObject(objectToPlace, rightHandPlacer.transform.position);
            objectToPlace = null;
        }
        else if (leftPointers.Contains(eventData.Pointer))
        {
            CreateAnchoredObject(objectToPlace, leftHandPlacer.transform.position);
            objectToPlace = null;
        }
    }

    public GameObject CreateObject(string name)
    {
        // GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // newObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        // newObject.GetComponent<Renderer>().material.color = Color.red;

        GameObject newObject = GameObject.Instantiate(rightHandPlacer);
        newObject.SetActive(true);
        
        //TODO: Change this to a prefab or at least add text label to it.

        return newObject;
    }

    public void CreateAnchoredObject(string name, Vector3 position)
    {   
        // Create object model (for debug/visualization purposes)
        GameObject newAnchoredObject = CreateObject(name);
        newAnchoredObject.transform.position = position;

        // Attach anchor
        var anchor = newAnchoredObject.AddComponent<AnchorableObject>();
        anchor._worldAnchorName = name;

        // Add to known anchors list
        anchoredObjects.Add(name, anchor);
    }

    public void SaveAnchors(string saveFilePath)
    {
        if (File.Exists(saveFilePath))
        {
            FileStream fileStream = File.Open(saveFilePath, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close(); // This flushes the content, too.
        }
        
        using(StreamWriter file = new StreamWriter(saveFilePath))
        {
            foreach(var kv in anchoredObjects)
            {
                if (kv.Value.SaveAnchor())
                {
                    file.WriteLine(kv.Key);
                }
            }
        }
    }

    public void LoadAnchors(string saveFilePath)
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogFormat("Save file does not exist at {0}", saveFilePath);
            return;
        }

        anchoredObjects.Clear();

        string[] lines = File.ReadAllLines(saveFilePath);
        foreach(string name in lines)
        {
            GameObject retrievedObject = CreateObject(name);
            var anchor = retrievedObject.AddComponent<AnchorableObject>();
            anchor._worldAnchorName = name;
            if (anchor.LoadAnchor())
            {
                anchoredObjects.Add(name, anchor);
            }
        }
    }
}
