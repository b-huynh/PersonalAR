using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Extensions;
using Microsoft.MixedReality.Toolkit.Utilities;

public class ContextMenuVisuals : MonoBehaviour
{
    // To be assigned at runtime
    [ReadOnly] public AnchorableObject anchor;

    // To be assigned in editor
    [SerializeField] private GameObject appButtonPrefab;
    [SerializeField] private GridObjectCollection objectCollection;

    // Intermediate state variables
    private HashSet<AppState> currentApps;
    private AnchorService anchorService;

    // Start is called before the first frame update
    void Start()
    {
        anchor = GetComponentInParent<IAnchorable>().Anchor;

        // Initialize private members
        currentApps = new HashSet<AppState>();

        IAnchorService serviceInterface;
        if (MixedRealityServiceRegistry.TryGetService<IAnchorService>(out serviceInterface))
        {
            anchorService = (AnchorService)serviceInterface;
        }
        else
        {
            ARDebug.LogWarning($"[{gameObject.name}] Failed to get AnchorService.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (anchor == null) { return; }

        if (anchorService.handlers.ContainsKey(anchor))
        {
            if (anchorService.handlers[anchor].Count != currentApps.Count)
            {
                // Update current apps list
                currentApps = anchorService.handlers[anchor];
                UpdateAppList(currentApps);
            }
        }
    }

    // OPTIMIZE: Don't destroy and instantiate everything from scratch, probably use anchorService events.
    void UpdateAppList(HashSet<AppState> appSet)
    {
        // Remove any existing app buttons
        foreach(Transform child in objectCollection.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Create new app buttons
        foreach(AppState app in appSet)
        {
            GameObject appButton = GameObject.Instantiate(appButtonPrefab, objectCollection.transform);
            appButton.GetComponent<AppButtonVisuals>().App = app;
        }

        // Update grid object collection
        objectCollection.UpdateCollection();
    }

    // // *** IMenu Implementation ***
    // public bool Visible { get; private set; }
    // public bool ToggleValue { get; }

    // public void Open()
    // {
    //     GetComponent<ScaleTween>().TweenIn();
    //     Visible = true;
    // }

    // public void Close()
    // {
    //     GetComponent<ScaleTween>().TweenOut();
    //     Visible = false;
    // }

    // public void Toggle()
    // {
    //     if (Visible) { Close(); }
    //     else { Open(); }
    // }
    // // *** end IMenu ***
}
