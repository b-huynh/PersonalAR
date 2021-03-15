using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject Instantiate(this GameObject obj, AppState appState)
    {
        // Cache active state
        bool activeState = obj.activeSelf;

        // Deactivate object so that OnEnable isn't called initially
        obj.SetActive(false);

        // Clone object and add AppStateListener if needed
        GameObject clone = GameObject.Instantiate(obj);
        AppStateListener listener;
        if (!clone.TryGetComponent<AppStateListener>(out listener))
        {
            listener = clone.AddComponent<AppStateListener>();
        }

        // Initialize AppStateListener
        listener.SetAppState(appState);

        // Reset object and clone to original active state
        obj.SetActive(activeState);
        clone.SetActive(activeState);

        return clone;
    }
}
