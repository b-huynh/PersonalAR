using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

using Recaug;

public class AnchorableObjectsManager : Singleton<AnchorableObjectsManager>
{
    private Dictionary<string, AnchorableObject> _anchoredObjects;
    public Dictionary<string, AnchorableObject> AnchoredObjects
    {
        get { return _anchoredObjects; }
        private set
        {
            _anchoredObjects = value;
        }
    }

    public TMPro.TextMeshProUGUI AnchorDebugText;

    public GameObject AnchorActorPrefab;
    private string _objectToPlace = null;
    private GameObject _rightHandPlacer;
    private GameObject _leftHandPlacer;

    // Start is called before the first frame update
    void Start()
    {
        AnchoredObjects = new Dictionary<string, AnchorableObject>();

        AnchorStoreManager.Instance.PropertyChanged += OnPropertyChanged;

        _rightHandPlacer = GameObject.Instantiate(AnchorActorPrefab, this.transform);
        // Destroy(_rightHandPlacer.GetComponent<AnchorableObject>());
        _rightHandPlacer.SetActive(false);

        _leftHandPlacer = GameObject.Instantiate(AnchorActorPrefab, this.transform);
        // Destroy(_leftHandPlacer.GetComponent<AnchorableObject>());
        _leftHandPlacer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_objectToPlace != null)
        {
            Vector3 rightEndPoint;
            if (PointerUtils.TryGetHandRayEndPoint(Handedness.Right, out rightEndPoint))
            {
                _rightHandPlacer.SetActive(true);
                _rightHandPlacer.transform.position = rightEndPoint;
            }
            else
            {
                _rightHandPlacer.SetActive(false);
            }

            Vector3 leftEndPoint;
            if (PointerUtils.TryGetHandRayEndPoint(Handedness.Left, out leftEndPoint))
            {
                _leftHandPlacer.SetActive(true);
                _leftHandPlacer.transform.position = leftEndPoint;
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

        DrawDebugText(true);
    }

    public void DrawDebugText(bool verbose = false)
    {
        if (AnchorStoreManager.Instance.AnchorStore == null)
        {
            AnchorDebugText.text = "Anchor store not initialized.\n";
            return;
        }

        int existingCount = AnchorStoreManager.Instance.AnchorStore.PersistedAnchorNames.Count();
        AnchorDebugText.text = $"<size=20><b>{existingCount} Existing Objects</b></size>\n";
        if (verbose)
        {
            foreach(string anchorName in AnchorStoreManager.Instance.AnchorStore.PersistedAnchorNames)
            {
                string debugLabel = AnchoredObjects[anchorName]._debugLabel.text.Replace("\r\n", ", ");
                AnchorDebugText.text += $"{debugLabel} \n";
            }
        }
        else
        {
            foreach(string anchorName in AnchorStoreManager.Instance.AnchorStore.PersistedAnchorNames)
            {
                AnchorDebugText.text += $"{anchorName}\n";
            }
        }
    }

    public void OnPropertyChanged(System.Object sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(AnchorStoreManager.Instance.AnchorStore))
        {
            LoadExistingAnchors();
        }
    }

    public void SetNextObject(string name)
    {
        _objectToPlace = name;

        // rightHandPlacer.GetComponentInChildren<TextMesh>().text = _objectToPlace;
        _rightHandPlacer.GetComponent<AnchorActor>().anchorName = _objectToPlace;
        _rightHandPlacer.SetActive(true);

        // leftHandPlacer.GetComponentInChildren<TextMesh>().text = _objectToPlace;
        _leftHandPlacer.GetComponent<AnchorActor>().anchorName = _objectToPlace;
        _leftHandPlacer.SetActive(true);
    }

    public void PlaceObject(MixedRealityPointerEventData eventData)
    {
        if (_objectToPlace == null) return;

        var rightPointers = PointerUtils.GetPointers<IMixedRealityPointer>(Handedness.Right, InputSourceType.Hand);
        var leftPointers = PointerUtils.GetPointers<IMixedRealityPointer>(Handedness.Left, InputSourceType.Hand);

        if (rightPointers.Contains(eventData.Pointer))
        {
            if (CreateAndSaveAnchor(_objectToPlace, _rightHandPlacer.transform.position))
            {
                _objectToPlace = null;
            }
        }
        else if (leftPointers.Contains(eventData.Pointer))
        {
            if (CreateAndSaveAnchor(_objectToPlace, _leftHandPlacer.transform.position))
            {
                _objectToPlace = null;
            }
        }
    }

    // Clone the AnchorActor game object prefab
    public GameObject CreateObject(string name)
    {
        GameObject newObject = GameObject.Instantiate(AnchorActorPrefab);
        newObject.GetComponent<AnchorActor>().anchorName = name;
        newObject.SetActive(true);
        return newObject;
    }

    // Create and save and
    public bool CreateAndSaveAnchor(string name, Vector3 position)
    {   
        // Create object model (for debug/visualization purposes)
        GameObject newAnchoredObject = CreateObject(name);
        newAnchoredObject.transform.position = position;

        // Attempt to save anchor at provided location
        var anchor = newAnchoredObject.GetComponent<AnchorableObject>();
        anchor._worldAnchorName = name;
        if (anchor.SaveAnchor())
        {
            // Add to list tracking anchored game objects
            AnchoredObjects.Add(name, anchor);
            Debug.Log($"Saved anchor {name}");
            return true;
        }
        else
        {
            // Could not save, so destroy the newly created game object
            Destroy(newAnchoredObject);
            Debug.Log($"Failed to save anchor {name}");
            return false;
        }
    }

    private void LoadExistingAnchors()
    {
        // Retrieve and show all current anchors for the space.
        if (AnchorStoreManager.Instance.AnchorStore == null)
        {
            Debug.LogFormat("Cannot load existing anchors, no anchor store found");
            return;
        }

        var existingAnchors = AnchorStoreManager.Instance.AnchorStore.PersistedAnchorNames;
        Debug.LogFormat("Found {0} existing anchors", existingAnchors.Count);

        foreach (string anchorName in existingAnchors)
        {
            GameObject anchorMesh = CreateObject(anchorName);
            AnchorableObject anchor = anchorMesh.GetComponent<AnchorableObject>();
            anchor._worldAnchorName = anchorName;
            anchor.OnAnchorLoaded.AddListener(
                delegate 
                {
                    Debug.LogFormat("Loaded anchor {0}", anchorName);
                    AnchoredObjects.Add(anchorName, anchor);
                }
            );
            if (!anchor.LoadAnchor())
            {
                Debug.LogFormat("Unable to load anchor {0}", anchorName);
            }
        }
    }

    public void DeleteAnchor(string name)
    {
        AnchorStoreManager.Instance.AnchorStore.UnpersistAnchor(name);
        Destroy(AnchoredObjects[name].gameObject);
        AnchoredObjects.Remove(name);
    }

    public void ClearAnchors()
    {
        AnchorStoreManager.Instance.AnchorStore.Clear();
        foreach(var kv in AnchoredObjects)
        {
            Destroy(kv.Value.gameObject);
        }
        AnchoredObjects.Clear();
    }

    // public void SaveAnchors()
    // {
    //     string fileFormat = "anchors?.txt";
    //     string[] files = Directory.GetFiles(Application.persistentDataPath, fileFormat);
    //     int nextIndex = files.Length + 1;

    //     string filename = "anchors" + nextIndex.ToString() + ".txt";
    //     string nextFilePath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, filename));
    //     SaveAnchors(nextFilePath);
    // }

    // public void SaveAnchors(string saveFilePath)
    // {
    //     if (File.Exists(saveFilePath))
    //     {
    //         FileStream fileStream = File.Open(saveFilePath, FileMode.Open);
    //         fileStream.SetLength(0);
    //         fileStream.Close(); // This flushes the content, too.
    //     }
        
    //     using(StreamWriter file = new StreamWriter(saveFilePath))
    //     {
    //         foreach(var kv in anchoredObjects)
    //         {
    //             if (kv.Value.SaveAnchor())
    //             {
    //                 // Save anchors unique ID (for retrieval from world anchor store) and save object name.
    //                 file.WriteLine(kv.Value._worldAnchorName + "," + kv.Key);
    //             }
    //             else
    //             {
    //                 Debug.LogFormat("Could not save anchored object {0}", kv.Key);
    //             }
    //         }
    //     }

    //     Debug.LogFormat("Saved file as {0}", saveFilePath);
    // }

    // public void LoadAnchors(string saveFilePath)
    // {
    //     if (!File.Exists(saveFilePath))
    //     {
    //         Debug.LogFormat("Save file does not exist at {0}", saveFilePath);
    //         return;
    //     }

    //     ClearUnsavedObjects();

    //     string[] lines = File.ReadAllLines(saveFilePath);
    //     foreach(string entry in lines)
    //     {
    //         // Split line into column values
    //         string[] cols = entry.Split(',');
    //         string anchorName = cols[0];
    //         string objectName = cols[1];

    //         GameObject retrievedObject = CreateObject(objectName);
    //         var anchor = retrievedObject.AddComponent<AnchorableObject>();
    //         anchor._worldAnchorName = anchorName;
    //         if (anchor.LoadAnchor())
    //         {
    //             anchoredObjects.Add(objectName, anchor);
    //         }
    //     }
    // }
}
