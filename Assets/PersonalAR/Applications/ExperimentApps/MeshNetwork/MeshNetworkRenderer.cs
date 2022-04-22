using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

using TMPro;

using Microsoft.MixedReality.Toolkit.Utilities;

public class MeshNetworkRenderer : MonoBehaviour
{
    // public string networkName;
    // public List<AnchorableObject> subnet;
    public Subnet subnet;
    public Vector3 lineOffset; // Can be used to avoid overlapping lines
    
    private List<GameObject> lineObjects;
    [SerializeField] private GameObject lineRendererPrefab;
    private Gradient lineRendererColor;

    [SerializeField] private GameObject networkInfoDisplay;
    [SerializeField] private TextMeshPro infoText;
    [SerializeField] private TextMeshPro lineText;

    private bool RenderersActive;

    void Awake()
    {
        lineObjects = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateVisuals();

        if (subnet != null)
        {
            subnet.CollectionChanged += OnSubnetChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // foreach(GameObject lineObject in lineObjects)
        // {
        //     LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
        //     lineRenderer.useWorldSpace = false;

            
        //     if (lineRenderer.positionCount > 0)
        //     {
        //         MeshCollider meshCollider = lineRenderer.gameObject.GetComponent<MeshCollider>();
        //         Mesh mesh = new Mesh();
        //         lineRenderer.BakeMesh(mesh, Camera.main, false);
        //         meshCollider.sharedMesh = mesh;
        //     }
        // }
    }

    public void OnSubnetChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        DestroyLineObjects();
        DrawLines();
        DrawInfo();
    }

    IEnumerator BakeMeshAfterLineRendererPositionsSet(LineRenderer lineRenderer)
    {
        yield return new WaitUntil(() => lineRenderer.positionCount > 0);

        lineRenderer.useWorldSpace = false;

        Mesh mesh = new Mesh();
        MeshFilter meshFilter = lineRenderer.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshCollider meshCollider = lineRenderer.gameObject.AddComponent<MeshCollider>();
        lineRenderer.BakeMesh(mesh, Camera.main, true);
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;

        Rigidbody rb = lineRenderer.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        meshCollider.isTrigger = true;
    }

    public void DrawLines()
    {
        for(int i = 0; i < subnet.Count; ++i)
        {
            for(int j = i + 1; j < subnet.Count; ++j)
            {
                Vector3 lineStart = subnet[i].transform.position + lineOffset;
                Vector3 lineStop = subnet[j].transform.position + lineOffset;

                GameObject lineObject = CreateLineObject(lineStart, lineStop);
                GameObject lineText = CreateLineText(lineObject.GetComponent<MixedRealityLineRenderer>());
            }
        }
    }

    public void DrawInfo()
    {
        if (subnet.Count <= 0)
        {
            networkInfoDisplay.SetActive(false);
        }
        else
        {
            networkInfoDisplay.SetActive(true);

            Vector3 networkCenter = Vector3.zero;
            foreach(AnchorableObject anchor in subnet)
            {
                networkCenter += anchor.transform.position;
            }
            networkCenter = networkCenter / subnet.Count;

            networkInfoDisplay.transform.position = networkCenter;
            infoText.text = subnet.networkDescription;
        }

        networkInfoDisplay.SetActive(false);
    }

    private GameObject CreateLineObject(Vector3 start, Vector3 end)
    {
        // Instantiate new line renderer prefab
        GameObject newLine = GameObject.Instantiate(lineRendererPrefab, transform);

        // Set start and end points of line
        newLine.transform.position = start;
        Vector3 localEnd = newLine.transform.InverseTransformPoint(end);
        newLine.GetComponent<SimpleLineDataProvider>().EndPoint = new MixedRealityPose(localEnd);

        // Set line color
        var mrtkLineRender = newLine.GetComponent<MixedRealityLineRenderer>();
        mrtkLineRender.LineColor = lineRendererColor;

        // Set active state
        newLine.SetActive(true);

        // var lineRenderer = newLine.GetComponent<LineRenderer>();
        // StartCoroutine(BakeMeshAfterLineRendererPositionsSet(lineRenderer));

        // MeshCollider meshCollider = newLine.AddComponent<MeshCollider>();

        // Mesh mesh = new Mesh();
        // lineRenderer.BakeMesh(mesh, true);
        // meshCollider.sharedMesh = mesh;
        // meshCollider.convex = true;

        // Save for tracking purposes
        lineObjects.Add(newLine);

        return newLine;
    }

    public GameObject CreateLineText(MixedRealityLineRenderer mrtkLineRenderer)
    {
        // Calculate center point of line
        Vector3 lineStart = mrtkLineRenderer.LineDataSource.FirstPoint;
        Vector3 lineEnd = mrtkLineRenderer.LineDataSource.LastPoint;
        Vector3 lineCenter = (lineEnd - lineStart) / 2.0f;

        // Create new 3D text mesh pro object
        GameObject textMeshObject = new GameObject($"LineText ({subnet.networkName}");
        textMeshObject.transform.SetParent(mrtkLineRenderer.transform, true);
        textMeshObject.transform.position = lineStart + lineCenter;

        // Calculate rotation of text mesh
        Vector3 upDirection = Vector3.Cross((lineEnd - lineStart), -mrtkLineRenderer.transform.right);
        Vector3 forwardDirection = Vector3.Cross((lineEnd - lineStart), mrtkLineRenderer.transform.up);
        
        // Set billboarding of along the angle of the line
        LookAtUser lookAtUser = textMeshObject.AddComponent<LookAtUser>();
        lookAtUser.forwardDirection = forwardDirection;
        lookAtUser.upDirection = upDirection;

        // Set offset away from line
        lookAtUser.SetOriginalPosition(textMeshObject.transform.position);
        lookAtUser.offsetTowardsUserAmount = 0.001f;

        // Calculate width, height of text mesh
        TextMeshPro textMesh = textMeshObject.AddComponent<TextMeshPro>();
        float width = (lineEnd - lineStart).magnitude;
        float height = 0.1f;
        RectTransform textMeshTransform = textMesh.GetComponent<RectTransform>();
        textMeshTransform.sizeDelta = new Vector2(width, height);
        textMesh.fontSize = 0.25f;
        textMesh.alignment = TextAlignmentOptions.Center;

        // Set text
        textMesh.text = $"Device Group: {subnet.networkDescription}";

        return textMeshObject;
    }

    public void SetSolidLineColor(Color color)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        colorKey = new GradientColorKey[2];
        colorKey[0].color = color;
        colorKey[0].time = 0.0f;
        colorKey[1].color = color;
        colorKey[1].time = 1.0f;

        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);

        lineRendererColor = gradient;
    }

    private string GradientToString(Gradient gradient)
    {
        string stringRep = $"(Gradient object {gradient.GetHashCode()}) ";

        stringRep += "ColorKey: ";
        foreach(GradientColorKey colorKey in gradient.colorKeys)
        {
            stringRep += $"({colorKey.color}, {colorKey.time}) ";
        }

        stringRep += "AlphaKey: ";
        foreach(GradientAlphaKey alphaKey in gradient.alphaKeys)
        {
            stringRep += $"({alphaKey.alpha}, {alphaKey.time}) ";
        }

        return stringRep;
    }

    private void DestroyLineObjects()
    {
        for(int i = 0; i < lineObjects.Count; ++i)
        {
            Destroy(lineObjects[i]);
        }
        lineObjects.Clear();
    }

    public void OnDestroy()
    {
        DestroyLineObjects();
    }

    public void SetRenderersActive(bool value)
    {
        int renderingLayer;
        if (value == true)
        {
            renderingLayer = LayerMask.NameToLayer("Default");
            // lineObjects.ForEach(lr => lr.SetActive(true));
            // networkInfoDisplay.SetActive(true);
        }
        else
        {
            renderingLayer = LayerMask.NameToLayer("SuspendRendering");
            // lineObjects.ForEach(lr => lr.SetActive(false));
            // networkInfoDisplay.SetActive(false);
        }

        lineObjects.ForEach(lr => lr.SetLayerInChildren(renderingLayer));
        networkInfoDisplay.SetLayerInChildren(renderingLayer);

        RenderersActive = value;
    }
}
