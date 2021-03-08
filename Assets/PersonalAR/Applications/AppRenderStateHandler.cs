using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AppRenderStateHandler : MonoBehaviour
{
    [SerializeField]
    private BasePervasiveApp respondToApp;

    public UnityEvent OnAppRenderOn;
    public UnityEvent OnAppRenderOff;

    // Start is called before the first frame update
    void Start()
    {
        respondToApp.OnRenderStateChanged += OnRenderStateChanged;
    }

    public void OnRenderStateChanged(bool changedTo)
    {
        if (changedTo == true)
        {
            // Rendering turned on
            OnAppRenderOn.Invoke();
        }
        else
        {
            // Rendering turned off
            OnAppRenderOff.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
