using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;

using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AnchorsInfoGUI : MonoBehaviour
{
    public bool isVerbose;
    private TextMeshProUGUI _textMesh;
    private IAnchorService _anchorService;

    // Start is called before the first frame update
    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();

        if (!MixedRealityServiceRegistry.TryGetService<IAnchorService>(out _anchorService))
        {
            Debug.LogError($"Could not get Anchor Service");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _textMesh.text = GetAnchorsInfoText(isVerbose);
    }

    public string GetAnchorsInfoText(bool verbose = false)
    {
        if (_anchorService.AnchorStore == null)
        {
            return "Anchor store not initialized.\n";
        }

        string infoText = $"<size=20><b>{_anchorService.AnchorCount} Placed Objects</b></size>\n";
        if (verbose)
        {
            foreach(string anchorName in _anchorService.AnchorNames)
            {
                var anchor = _anchorService.GetAnchor(anchorName);
                string debugLabel = anchor._debugLabel.text.Replace("\r\n", ", ");
                infoText += $"{debugLabel} \n";
            }
        }
        else
        {
            foreach(string anchorName in _anchorService.AnchorNames)
            {
                infoText += $"{anchorName}\n";
            }
        }
        return infoText;
    }
}
