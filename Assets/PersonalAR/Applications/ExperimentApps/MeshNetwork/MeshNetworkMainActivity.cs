using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

public class Subnet : ObservableCollection<AnchorableObject>
{
    public string networkName;
    public string networkDescription { get; set; }

    // For tracking code pieces assigned to this object
    private RandomPinCodes codeSet;
    private CodePiece codePiece;

    public event EventHandler SubnetChanged;

    public Subnet(string networkName)
    {
        this.networkName = networkName;

        CollectionChanged += OnCollectionChanged;
        PropertyChanged += OnPropertyChanged;
    }

    ~Subnet()
    {
        CollectionChanged -= OnCollectionChanged;
        PropertyChanged -= OnPropertyChanged;

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
        networkDescription = networkName + $"\n\n (CODE) <color=#FF7F7F>{codePiece.Label}-{codePiece.Value}</color>";
        codePiece.Code.OnCodeEntryComplete.AddListener(OnCodeEntryComplete);

        SubnetChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnCodeEntryComplete()
    {
        codePiece.Code.OnCodeEntryComplete.RemoveListener(OnCodeEntryComplete);
        InitWithCodes(codeSet);
    }
    protected void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs eventArgs)
    {
        SubnetChanged?.Invoke(sender, EventArgs.Empty);
    }

    protected void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs eventArgs)
    {
        SubnetChanged?.Invoke(sender, EventArgs.Empty);
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
    private List<Subnet> networks;
    private List<MeshNetworkRenderer> meshNetworkRenderers;
    private AnchorService anchorService;

    private List<Color> colorList = new List<Color>()
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        new Color(255, 128, 0), // orange
        new Color(127, 255, 0), // chartreuse
        new Color(0, 255, 127), // spring green
        new Color(0, 127, 255), // azure
        new Color(127, 0, 255), // violet
        new Color(255, 0, 128), // rose
        Color.grey,
        Color.white,
        Color.black,
    };

    private Dictionary<string, List<string>> groupMapping = new Dictionary<string, List<string>>()
    {
        {
            "Lighting", new List<string>()
            {
                "Lamp", "Light Switch", "Window Blinds"
            }
        },
        {
            "Temperature Control", new List<string>()
            {
                "Fan", "Heater", "Thermostat"
            }
        },
        {
            "Coffee", new List<string>()
            {
                "Coffee Grinder", "Kettle", "Coffee Maker"
            }
        },
        {
            "Office Equipment", new List<string>()
            {
                "Printer", "Desktop Computer", "Laptop"
            }
        },
        {
            "Communications", new List<string>()
            {
                "Router", "Telephone", "Speakers"
            }
        },
        // {
        //     "Office Furniture", new List<string>()
        //     {
        //         "Desk", "Chair", "Rug"
        //     }
        // },
        // {
        //     "Living Room Furniture", new List<string>()
        //     {
        //         "Sofa", "Bookshelf", "Filing Cabinet"
        //     }
        // },
        // {
        //     "Switchable", new List<string>()
        //     {
        //         "TV", "PC", "Laptop", "Fan", "Heater", "Vacuum", "Speakers", "Printer", "Coffee Grinder", "Kettle", "Router", "Telephone", "Lamp", "Light Switch", "Electrical Outlet", "Thermostat", "Coffee Maker", 
        //     }
        // },
        // {
        //     "Appliances", new List<string>()
        //     {
        //         "Fan", "Heater", "Kettle"
        //     }
        // },
        // {
        //     "Refillable", new List<string>()
        //     {
        //         "Coffee Grinder", "Kettle", "Printer"
        //     }
        // },
        // {
        //     "Office", new List<string>()
        //     {
        //         "Desk", "Chair", "Filing Cabinet", "PC", "Printer"
        //     }
        // },
        // {
        //     "Entertainment", new List<string>()
        //     {
        //         "TV", "Speakers"
        //     }
        // }
    };

    private CircularBuffer<Color> colorPool;

    void Awake()
    {
        // Initialize Containers
        networks = new List<Subnet>();
        meshNetworkRenderers = new List<MeshNetworkRenderer>();
        colorPool = new CircularBuffer<Color>(colorList.Count, colorList.ToArray());

        if (MixedRealityServiceRegistry.TryGetService<AnchorService>(out anchorService) == false)
        {
            Debug.LogWarning("Can't get anchorservice");
        }
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

    public void CreateNetworkRenderers()
    {
        // foreach(Subnet subnet in networks)
        for(int i = 0; i < networks.Count; ++i)
        {
            Subnet subnet = networks[i];

            int negativeFactor = (i % 2 == 0) ? -1 : 1;
            float offsetFactor = Mathf.Floor((float)i / 2.0f + 0.5f) * negativeFactor;
            float offsetAmount = offsetFactor * 0.015f;
            Vector3 lineOffset = new Vector3(0, offsetAmount, 0);

            CreateNetworkRenderer(subnet, lineOffset);
        }
    }

    public void CreateNetworkRenderer(Subnet subnet, Vector3 lineOffset = default(Vector3))
    {
        GameObject newChild = GameObject.Instantiate(rendererPrefab);
        newChild.transform.parent = this.transform;
        
        var networkRenderer = newChild.GetComponent<MeshNetworkRenderer>();
        networkRenderer.subnet = subnet;
        networkRenderer.lineOffset = lineOffset;

        Color nextColor = colorPool.Front();
        networkRenderer.SetSolidLineColor(nextColor);
        colorPool.PushBack(nextColor);

        meshNetworkRenderers.Add(networkRenderer);
    }

    public override void StartActivity(ExecutionContext ec)
    {
        if (initialized == false)
        {
            // Subscribe to Anchor Service Events
            if (anchorService != null)
            {
                anchorService.OnAfterRegistered += OnAfterRegisteredHandler;
                anchorService.OnBeforeRemoved += OnBeforeRemovedHandler;
            }

            // Add existing anchors to groups and initialize their renderers
            anchorService.AnchoredObjects.Values
                .ToList()
                .ForEach(anchor => AddAnchorToGroups(anchor));

            initialized = true;
        }

        meshNetworkRenderers.ForEach(lr => lr.SetRenderersActive(true));
    }

    public override void StopActivity(ExecutionContext ec)
    {
        meshNetworkRenderers.ForEach(lr => lr.SetRenderersActive(false));
    }

    private void AddAnchorToGroups(AnchorableObject anchor)
    {
        List<string> groupsContainingAnchor = 
            groupMapping.Where(kv => kv.Value.Contains(anchor.WorldAnchorName, StringComparer.OrdinalIgnoreCase))
                        .Select(kv => kv.Key)
                        .ToList();

        foreach(string groupName in groupsContainingAnchor)
        {
            // Lazy initialize groups/subnets and their renderers once added
            if (networks.Count(network => string.Equals(network.networkName, groupName, StringComparison.OrdinalIgnoreCase)) <= 0)
            {
                Subnet newSubnet = new Subnet(groupName);
                newSubnet.InitWithCodes(codeSet);
                networks.Add(newSubnet);
                CreateNetworkRenderer(newSubnet);
            }

            networks.Where(network => string.Equals(network.networkName, groupName, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .ForEach(network => network.Add(anchor));
        }
    }

    private void RemoveAnchorFromGroups(AnchorableObject anchor)
    {
        networks.Where(network => network.Contains(anchor))
                .ToList()
                .ForEach(network => network.Remove(anchor));

        //TODO: Do we need to remove NetworkRenderers if you delete all anchors in a group?...
    }

    public void OnAfterRegisteredHandler(AnchorableObject anchor)
    {
        AddAnchorToGroups(anchor);
    }

    public void OnBeforeRemovedHandler(AnchorableObject anchor)
    {
        RemoveAnchorFromGroups(anchor);
    }
}