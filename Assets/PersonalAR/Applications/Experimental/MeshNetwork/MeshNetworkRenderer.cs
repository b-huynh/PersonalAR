using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;

public class MeshNetworkRenderer : MonoBehaviour
{
    public string networkName;
    // public Gradient renderColor;
    public List<AnchorableObject> anchors;
    
    private List<GameObject> lineObjects;
    [SerializeField] private GameObject lineRendererPrefab;

    // Start is called before the first frame update
    void Start()
    {
        lineObjects = new List<GameObject>();
        // DestroyLineObjects();
        DrawLines();
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

    private void CreateLineObject(Vector3 start, Vector3 end)
    {
        GameObject newLine = GameObject.Instantiate(lineRendererPrefab);

        newLine.transform.position = start;
        Vector3 localEnd = newLine.transform.InverseTransformPoint(end);
        // Debug.Log($"Start: {start}, End: {end}");
        // Debug.Log($"Start: {start}, Local: {localEnd}");
        newLine.GetComponent<SimpleLineDataProvider>().EndPoint = new MixedRealityPose(localEnd);

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
