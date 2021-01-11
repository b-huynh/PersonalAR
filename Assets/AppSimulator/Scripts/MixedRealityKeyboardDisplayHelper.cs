using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

using Recaug;

public class MixedRealityKeyboardDisplayHelper : MonoBehaviour
{
    private TextMeshPro textMesh;
    private MixedRealityKeyboard mixedRealityKeyboard;

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
        ObjectRegistry.Instance.Register(textMesh.text, transform.parent.position);
    }
}
