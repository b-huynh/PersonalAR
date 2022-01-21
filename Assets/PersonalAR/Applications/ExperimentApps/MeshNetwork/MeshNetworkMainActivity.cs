using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

public class Subnet : List<AnchorableObject>
{
    public string networkName;
    public string networkDescription;

    // For tracking code pieces assigned to this object
    private RandomPinCodes codeSet;
    private CodePiece codePiece;

    public Subnet(string networkName)
    {
        this.networkName = networkName;
    }

    ~Subnet()
    {
        if (codePiece != null)
        {
            codePiece.Code.OnCodeEntryComplete.RemoveListener(OnCodeEntryComplete);
        }
    }

    public override string ToString()
    {
        string anchors = "";
        foreach(AnchorableObject anchor in this)
        {
            anchors += anchor.WorldAnchorName + " | ";
        }
        return $"{networkName}: ( {anchors} )";
    }
    public void InitWithCodes(RandomPinCodes codes)
    {
        codeSet = codes;
        codePiece = codeSet.GetAssignment(this, 1);
        networkDescription = networkName + $"\n\n (CODE) {codePiece.Label}-{codePiece.Value}";
        codePiece.Code.OnCodeEntryComplete.AddListener(OnCodeEntryComplete);
    }

    public void OnCodeEntryComplete()
    {
        codePiece.Code.OnCodeEntryComplete.RemoveListener(OnCodeEntryComplete);
        InitWithCodes(codeSet);
    }
}

public class MeshNetworkMainActivity : BaseAppActivity
{
    [Header("MeshNetwork Main Activity")]
    [SerializeField] public RandomPinCodes codeSet;
    [SerializeField] public GameObject rendererPrefab;

    [Header("Subnet Generation Params")]
    [Range(1, 20)]
    [SerializeField] public int numGroups = 2;
    [Range(2, 10)]
    [SerializeField] public int avgGroupSize = 3;


    // Internal intermediate data
    // private Dictionary<string, List<AnchorableObject>> networks;
    private List<Subnet> networks;
    private List<GameObject> lineRenderers;
    private AnchorService anchorService;

    private List<Color> colorList = new List<Color>()
    {
        Color.blue,
        Color.red,
        Color.green,
        Color.cyan,
        Color.grey,
        Color.magenta,
        Color.yellow,
        Color.white
    };
    private CircularBuffer<Color> colorPool;

    void Awake()
    {
        if (MixedRealityServiceRegistry.TryGetService<AnchorService>(out anchorService) == false)
        {
            Debug.LogWarning("Can't get anchorservice");
        }

        colorPool = new CircularBuffer<Color>(colorList.Count, colorList.ToArray());
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Create random groups of objects as Subnets
    public void DetermineNetworks()
    {
        // Parameters

        // bool maintainCategories = false; // Not used currently

        // Create initial anchors. These should be pulled from first to guarantee assignment to all objects.
        List<AnchorableObject> initialSet = new List<AnchorableObject>(anchorService.AnchoredObjects.Values);

        var rnd = new System.Random(codeSet.randomSeed);
        // Create Subnets
        for (int i = 0; i < numGroups; ++i)
        {
            // Determine group size
            int groupSize = rnd.Next(avgGroupSize - 1, avgGroupSize + 1);

            // Draw randomly from initial set
            Subnet subnet = new Subnet($"Group {i}");
            for(int j = 0; j < groupSize; ++j)
            {
                if (initialSet.Count > 0)
                {
                    // Draw from initial set until it's empty to guarantee all objects in at least 1 Subnet
                    int pullIndex = rnd.Next(initialSet.Count);
                    subnet.Add(initialSet[pullIndex]);
                    initialSet.RemoveAt(pullIndex);
                }
                else
                {
                    // Once initial set is empty, we can draw randomly
                    var otherSet = anchorService.AnchoredObjects.Values.Except(subnet).ToList();
                    int pullIndex = rnd.Next(otherSet.Count);
                    subnet.Add(otherSet[pullIndex]);
                }
            }
            networks.Add(subnet);
        }
    }

    public void DrawNetworks()
    {
        foreach(Subnet subnet in networks)
        {
            Debug.Log(subnet);
            DrawNetwork(subnet);
        }
    }

    public void DrawNetwork(Subnet subnet)
    {
        GameObject newChild = GameObject.Instantiate(rendererPrefab);
        newChild.transform.parent = this.transform;
        
        var networkRenderer = newChild.GetComponent<MeshNetworkRenderer>();
        // networkRenderer.networkName = subnet.networkName;
        // networkRenderer.networkName = subnet.networkDescription;
        networkRenderer.subnet = subnet;

        Color nextColor = colorPool.Front();
        networkRenderer.SetSolidLineColor(nextColor);
        colorPool.PushBack(nextColor);
        // networkRenderer.DrawLines();

        lineRenderers.Add(newChild);
    }

    public override void StartActivity(ExecutionContext ec)
    {
        Debug.Log($"Start Activity {activityID}");

        // networks = new Dictionary<string, List<AnchorableObject>>();

        if (initialized == false)
        {
            networks = new List<Subnet>();
            lineRenderers = new List<GameObject>();

            DetermineNetworks();

            // Get code piece assignment
            // var assignment = codeSet.AssignCodePieces(networks, 1);
            networks.ForEach(subnet => subnet.InitWithCodes(codeSet));

            DrawNetworks();
            initialized = true;
        }

        lineRenderers.ForEach(lr => lr.SetActive(true));
    }

    public override void StopActivity(ExecutionContext ec)
    {
        lineRenderers.ForEach(lr => lr.SetActive(false));

        // Debug.Log($"Stop Activity {activityID}");

        // for(int i = 0; i < lineRenderers.Count; ++i)
        // {
        //     Destroy(lineRenderers[i]);
        // }
        // lineRenderers.Clear();
    }
}
