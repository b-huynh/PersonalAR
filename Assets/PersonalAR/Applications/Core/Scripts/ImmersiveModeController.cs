using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmersiveModeController : MonoBehaviour
{
    List<AppState> allKnownApps;

    // Start is called before the first frame update
    void Start()
    {
        allKnownApps = new List<AppState>();
        
        // Get all known apps on start.        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintLastApp()
    {
        Debug.Log(AppState.lastAppStarted?.name);
    }

    public void CloseAllAppsExcept(AppState lastApp)
    {

    }

    public void TransitionToImmersive()
    {
        // Stop other running apps except last started app
        
    }
}
