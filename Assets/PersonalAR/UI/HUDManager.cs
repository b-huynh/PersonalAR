using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LaunchPoints))]
public class HUDManager : Singleton<HUDManager>
{
    [NonSerialized]
    public LaunchPoints launchPoints;

    // Start is called before the first frame update
    void Start()
    {
        launchPoints = GetComponent<LaunchPoints>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
