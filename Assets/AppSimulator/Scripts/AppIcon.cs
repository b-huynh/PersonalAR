using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class stores information about how to display AppIcons
public class AppIcon : MonoBehaviour
{   
    public int _appID;
    public int AppID
    {
        get => _appID;
        set
        {
            _appID = value;
            SetDisplayProperties();
        }
    }

    private App appRef;

    private BasePervasiveApp _appRef;
    public BasePervasiveApp AppRef
    {
        get => _appRef;
        set
        {
            _appRef = value;
        }
    }

    [Tooltip("Optional gameobject for visualizing application active state.")]
    public GameObject activeIndicator;

    [Tooltip("Optional gameobject for visualizing application focus on/off state.")]
    public GameObject focusIndicator;

    private void SetDisplayProperties()
    {
        appRef = GameManager.Instance.GetApp(AppID);
        GetComponent<MeshRenderer>().material = appRef.appLogo;
        transform.Find("AppName").GetComponent<TextMesh>().text = appRef.appName;
    }

    // Start is called before the first frame update
    void Start()
    {
        // SetDisplayProperties();
    }

    // Update is called once per frame
    void Update()
    {
        DrawAppIcon();
    }

    void DrawAppIcon()
    {
        if (_appRef)
        {
            GetComponent<MeshRenderer>().material = _appRef.appState.appLogo;
            GetComponentInChildren<TMPro.TextMeshPro>().text = _appRef.appState.appName;
            if (activeIndicator)
            {
                activeIndicator.SetActive(_appRef.Rendering);
            }
        }
    }

    public void OnClick()
    {
        _appRef.ToggleStartOrSuspend();
    }

    public void OnFocusEnter()
    {
        // focusIndicator?.SetActive(true);
    }

    public void OnFocusExit()
    {
        // focusIndicator?.SetActive(false);
    }
}
