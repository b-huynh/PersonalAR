using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Extensions;

public class MixedRealityKeyboardDisplayHelper : MonoBehaviour
{
    private TextMeshPro textMesh;
    private MixedRealityKeyboard mixedRealityKeyboard;

    private AnchorService _anchorService;
    void Awake()
    {
        if (!MixedRealityServiceRegistry.TryGetService<AnchorService>(out _anchorService))
        {
            ARDebug.LogError($"Failed to get AnchorService");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        mixedRealityKeyboard = GetComponent<MixedRealityKeyboard>();
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.text = mixedRealityKeyboard.Text;
    }

    public void OnCommitText()
    {
        _anchorService.RegisterAnchor(textMesh.text, transform.parent.position);
    }
}
