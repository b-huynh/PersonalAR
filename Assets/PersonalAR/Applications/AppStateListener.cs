using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IAppStateListener
{
    void AppStart();
    void RenderStateChanged(bool newValue);
    void AppQuit();
}

public class AppStateListener : MonoBehaviour, IAppStateListener
{
    [SerializeField]
    private AppState appState;

    [Tooltip("Send AppState event messages to other components.")]
    [SerializeField]
    private bool RouteAppEvents;

    public UnityEvent OnAppStart;
    public UnityEvent OnAppRenderOn;
    public UnityEvent OnAppRenderOff;
    public UnityEvent OnAppQuit;

    void OnEnable()
    {
        // Match current app rendering state
        RenderStateChanged(appState.IsRendering);
        appState.AddListener(this);
    }

    void OnDisable()
    {
        appState?.RemoveListener(this);
    }

    public void SetAppState(AppState newAppState)
    {
        if (this.appState != newAppState)
        {
            OnDisable();
            this.appState = newAppState;
            OnEnable();
        }
    }

    public AppState GetAppState()
    {
        return this.appState;
    }

    public void AppStart()
    {
        OnAppStart.Invoke();

        if (RouteAppEvents)
            gameObject.SendMessage("AppStart", SendMessageOptions.DontRequireReceiver);
    }

    public void AppQuit()
    {
        OnAppQuit.Invoke();

        if (RouteAppEvents)
            gameObject.SendMessage("AppQuit", SendMessageOptions.DontRequireReceiver);
    }

    public void RenderStateChanged(bool newValue)
    {
        if (newValue == true)
        {
            // Rendering turned on
            OnAppRenderOn.Invoke();
        }
        else
        {
            // Rendering turned off
            OnAppRenderOff.Invoke();
        }

        if (RouteAppEvents)
            gameObject.SendMessage("RenderStateChanged", newValue, SendMessageOptions.DontRequireReceiver);
    }
}
