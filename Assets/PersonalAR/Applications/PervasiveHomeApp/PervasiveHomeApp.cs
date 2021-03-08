using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions; 

public class PervasiveHomeApp : BasePervasiveApp
{
    [SerializeField]
    private AppEntity anchorView;

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

    void OnObjectRegistered(AnchorableObject anchor)
    {
        // Remove AnchorActors if leftover from startup staging.
        AnchorActor anchorActor;
        if (anchor.TryGetComponent<AnchorActor>(out anchorActor))
        {
            anchorActor.enabled = false;
        }

        GameObject newAnchorView = AppEntity.Instantiate(anchorView.gameObject, this);
        newAnchorView.GetComponent<IAnchorable>().SetAnchor(anchor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
