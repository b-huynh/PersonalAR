using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions; 

public class AnchorViewController : MonoBehaviour
{
    [SerializeField]
    private AnchorView anchorViewPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Register for new object events
        IAnchorService anchorService;
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out anchorService))
        {
            anchorService.OnRegistered += OnObjectRegistered;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnObjectRegistered(AnchorableObject anchor)
    {
        // Create an AnchorView and attach to anchor.
        GameObject clone = GameObject.Instantiate(anchorViewPrefab.gameObject);
        clone.GetComponent<IAnchorable>().SetAnchor(anchor);
    }
}
