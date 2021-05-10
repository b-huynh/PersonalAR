using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class SimulatedObjectViewController : MonoBehaviour
{
    public string SimulatedObjectName;
    public Material ObjectMaterial;

    private GameObject textMeshGameObject;
    private TextMeshPro textMesh;

    void OnEnable()
    {
        textMeshGameObject = new GameObject("Label");
        textMeshGameObject.transform.parent = this.transform;

        textMesh = textMeshGameObject.AddComponent<TextMeshPro>();
        textMesh.fontSize = 1;

        if (string.IsNullOrEmpty(SimulatedObjectName))
        {
            SimulatedObjectName = this.name;
        }
        textMesh.text = SimulatedObjectName;


        textMesh.rectTransform.anchoredPosition3D = Vector3.zero;
        textMesh.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
        textMesh.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
    }

    void OnDisable()
    {
        Destroy(textMeshGameObject);
        Destroy(textMesh);
    }

    void OnValidate()
    {
        if (textMesh != null)
        {
            textMesh.text = SimulatedObjectName;
        }
        
        foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = ObjectMaterial;
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
}
