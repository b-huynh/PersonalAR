using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AppState : ScriptableObject
{
    public string appName;
    public string appDesc;
    public Material appLogo;
    private bool Rendering;
    private List<AppRenderStateHandler> listeners = new List<AppRenderStateHandler>();

    public void SetRenderState(bool value)
    {
        if (Rendering != value)
        {
            Rendering = value;
            foreach(var listener in listeners)
            {
                listener.OnRenderStateChanged(Rendering);
            }
            // OnRenderStateChanged?.Invoke(Rendering);
        }
    }

    public void ToggleRenderState()
    {
        SetRenderState(!Rendering);
    }

    public void RegisterListener(AppRenderStateHandler listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(AppRenderStateHandler listener)
    {
        listeners.Remove(listener);
    }
}
