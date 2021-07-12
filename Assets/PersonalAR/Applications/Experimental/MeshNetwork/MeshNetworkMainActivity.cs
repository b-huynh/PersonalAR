using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshNetworkMainActivity : BaseAppActivity
{
    [Header("MeshNetwork Main Activity")]
    [SerializeField] public RandomPinCodes codeSet;
    [SerializeField] public GameObject rendererPrefab;

    // Internal intermediate data
    private Dictionary<string, List<AnchorableObject>> networks;
    private List<GameObject> lineRenderers;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DetermineNetworks()
    {
        var assignment = codeSet.Assignment;
        foreach(var kv in assignment)
        {
            if (networks.ContainsKey(kv.Value.Label) == false)
            {
                networks.Add(kv.Value.Label, new List<AnchorableObject>());
            }
            networks[kv.Value.Label].Add(kv.Key);
        }

        // ************ OLD CODE DELETE *********************************************
        // List<string> letters = new List<string> { "A", "B", "C", "D", "E" };
        // foreach(var kv in SmarthubObjectActivity.assignedCodeSetLabels)
        // {
        //     foreach(string letter in letters)
        //     {
        //         if (kv.Value.Contains(letter))
        //         {
        //             networks[letter].Add(kv.Key);
        //         }
        //     }
        // }
        //*****************************************************************************
    }

    public void DrawNetwork(string networkName, List<AnchorableObject> anchors)
    {
        GameObject newChild = GameObject.Instantiate(rendererPrefab);
        newChild.transform.parent = this.transform;
        
        var networkRenderer = newChild.GetComponent<MeshNetworkRenderer>();
        networkRenderer.networkName = networkName;
        networkRenderer.anchors = anchors;
        // networkRenderer.DrawLines();

        lineRenderers.Add(newChild);
    }

    public override void StartActivity(ExecutionContext ec)
    {
        Debug.Log($"Start Activity {activityID}");

        networks = new Dictionary<string, List<AnchorableObject>>();
        lineRenderers = new List<GameObject>();
        // networks.Add("A", new List<AnchorableObject>());
        // networks.Add("B", new List<AnchorableObject>());
        // networks.Add("C", new List<AnchorableObject>());
        // networks.Add("D", new List<AnchorableObject>());
        // networks.Add("E", new List<AnchorableObject>());

        DetermineNetworks();
        foreach(var kv in networks)
        {
            if (kv.Value.Count >= 2)
            {
                DrawNetwork(kv.Key, kv.Value);
            }
        }
    }

    public override void StopActivity(ExecutionContext ec)
    {
        Debug.Log($"Stop Activity {activityID}");

        for(int i = 0; i < lineRenderers.Count; ++i)
        {
            Destroy(lineRenderers[i]);
        }
        lineRenderers.Clear();
    }
}
