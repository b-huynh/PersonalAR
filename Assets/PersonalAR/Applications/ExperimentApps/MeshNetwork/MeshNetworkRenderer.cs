using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using Microsoft.MixedReality.Toolkit.Utilities;

public class MeshNetworkRenderer : MonoBehaviour
{
    public string networkName;
    public List<AnchorableObject> anchors;
    
    private List<GameObject> lineObjects;
    [SerializeField] private GameObject lineRendererPrefab;
    private Gradient lineRendererColor;

    [SerializeField] private GameObject networkInfoDisplay;
    [SerializeField] private TextMeshPro infoText;


    // Start is called before the first frame update
    void Start()
    {
        lineObjects = new List<GameObject>();
        // DestroyLineObjects();
        DrawLines();
        DrawInfo();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrawLines()
    {
        for(int i = 0; i < anchors.Count; ++i)
        {
            for(int j = 0; j < anchors.Count; ++j)
            {
                if (i != j)
                {
                    CreateLineObject(anchors[i].transform.position, anchors[j].transform.position);
                }
            }
        }
    }

    public void DrawInfo()
    {
        Vector3 networkCenter = Vector3.zero;
        foreach(AnchorableObject anchor in anchors)
        {
            networkCenter += anchor.transform.position;
        }
        networkCenter = networkCenter / anchors.Count;

        networkInfoDisplay.transform.position = networkCenter;
        infoText.text = networkName;
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

    private void CreateLineObject(Vector3 start, Vector3 end)
    {
        GameObject newLine = GameObject.Instantiate(lineRendererPrefab);

        newLine.transform.position = start;
        Vector3 localEnd = newLine.transform.InverseTransformPoint(end);
        // Debug.Log($"Start: {start}, End: {end}");
        // Debug.Log($"Start: {start}, Local: {localEnd}");
        newLine.GetComponent<SimpleLineDataProvider>().EndPoint = new MixedRealityPose(localEnd);

        var mrtkLineRender = newLine.GetComponent<MixedRealityLineRenderer>();
        mrtkLineRender.LineColor = lineRendererColor;

        // newLine.GetComponent<SimpleLineDataProvider>().EndPoint = new MixedRealityPose(end, Quaternion.identity);

        newLine.SetActive(true);

        lineObjects.Add(newLine);

        // // Add line renderer components
        // GameObject newLine = new GameObject();
        // newLine.AddComponent<LineRenderer>();
        // var mrtkLineRender = newLine.AddComponent<MixedRealityLineRenderer>();
        // var dataProvider = newLine.AddComponent<SimpleLineDataProvider>();

        // // Create line data
        // mrtkLineRender.LineDataSource = dataProvider;
        // newLine.transform.position = start;
        // dataProvider.EndPoint = new MixedRealityPose(end);
        // mrtkLineRender.LineColor = new Gradient();
        // mrtkLineRender.LineMaterial = new Material(Shader.Find("Line"));

        // lineObjects.Add(newLine);
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
}
