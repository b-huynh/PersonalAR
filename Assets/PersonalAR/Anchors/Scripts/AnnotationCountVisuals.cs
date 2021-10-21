using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

public class AnnotationCountVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countTextMesh;

    private IAnchorService anchorService;

    // Start is called before the first frame update
    void Start()
    {
        if (!MixedRealityServiceRegistry.TryGetService<IAnchorService>(out anchorService))
        {
            ARDebug.LogError($"Failed to get AnchorService");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (countTextMesh != null)
        {
            string anchorCount = anchorService.AnchorCount.ToString();
            countTextMesh.text = $"Anchors Set: {anchorCount} / 15";
        }        
    }
}
