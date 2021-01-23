using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.WindowsMR;
using TMPro;

public class AnchorStoreInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string bodyText = "";
        if (AnchorStoreManager.Instance.AnchorStore != null)
        {
            bodyText = AnchorStoreManager.Instance.AnchorStoreDebugText;
        }
        else
        {
            bodyText = "Anchor Store is null";
        }
        GetComponent<TMPro.TextMeshProUGUI>().text = bodyText;
    }
}
