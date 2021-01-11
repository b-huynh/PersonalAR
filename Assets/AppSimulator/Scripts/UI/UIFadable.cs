using System;
using UnityEngine;

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

// [RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(BoxCollider))]
public class UIFadable : MonoBehaviour, IMixedRealityFocusHandler
{
    public float fadeOutTime = 1.0f; // seconds

    public float stopVal = 0.1f;

    private float exitTimer = 0.0f;

    private float currentAlpha = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        SetAlpha(stopVal);
    }

    public event Action OnFadeFocusEnter = delegate {};
    public event Action OnFadeFocusExit = delegate {};

    // Update is called once per frame
    void Update()
    {
        if (currentAlpha == 1.0f)
        {
            // Is focusing, check if not focusing.
            if (!IntersectGaze())
            {
                exitTimer = fadeOutTime;
                OnFadeFocusExit();
            }
        }
        
        if (exitTimer > stopVal)
        {
            exitTimer -= Time.deltaTime;
            float percentage = exitTimer / fadeOutTime;
            SetAlpha(percentage);
        }
    }

    private void SetAlpha(float val)
    {
        foreach(var rend in GetComponentsInChildren<Renderer>())
        {
            if (rend.gameObject.layer == LayerMask.NameToLayer("IgnoreFadable"))
            {
                continue;
            }
            Color currCol = rend.material.color;
            currCol.a = val;
            rend.material.color = currCol;
        }
        currentAlpha = val;
    }
    private bool IntersectGaze()
    {
        Ray gazeRay = new Ray(
            CoreServices.InputSystem.GazeProvider.GazeOrigin,
            CoreServices.InputSystem.GazeProvider.GazeDirection);
        return GetComponent<BoxCollider>().bounds.IntersectRay(gazeRay);
    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        exitTimer = 0.0f;
        SetAlpha(1.0f);

        OnFadeFocusEnter();
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        // if (!IntersectGaze())
        // {
        //     Debug.Log("Fade Exit");
        //     exitTimer = fadeOutTime;
        // }
        // Debug.Log("Focus Exit");
    }
}
