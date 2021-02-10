using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AnchorsInfoGUI : MonoBehaviour
{
    public bool isVerbose;
    private TextMeshProUGUI _textMesh;

    // Start is called before the first frame update
    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _textMesh.text = GetAnchorsInfoText(isVerbose);
    }

    public string GetAnchorsInfoText(bool verbose = false)
    {
        if (AnchorStoreManager.Instance.AnchorStore == null)
        {
            return "Anchor store not initialized.\n";
        }

        int numAnchors = AnchorStoreManager.Instance.AnchorStore.PersistedAnchorNames.Count;
        string infoText = $"<size=20><b>{numAnchors} Existing Objects</b></size>\n";
        if (verbose)
        {
            foreach(string anchorName in AnchorStoreManager.Instance.AnchorStore.PersistedAnchorNames)
            {
                string debugLabel = AnchorableObjectsManager.Instance.AnchoredObjects[anchorName]._debugLabel.text.Replace("\r\n", ", ");
                infoText += $"{debugLabel} \n";
            }
        }
        else
        {
            foreach(string anchorName in AnchorStoreManager.Instance.AnchorStore.PersistedAnchorNames)
            {
                infoText += $"{anchorName}\n";
            }
        }
        return infoText;
    }
}
