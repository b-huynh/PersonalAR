using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmersiveModeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintLastApp()
    {
        Debug.Log(AppState.lastAppStarted?.name);
    }

    public void TransitionToImmersive()
    {
        // Stop other running apps except last started app
        
    }
}
