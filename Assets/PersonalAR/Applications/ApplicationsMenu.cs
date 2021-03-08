using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;

using Recaug;

public class ApplicationsMenu : MonoBehaviour, IMenu
{
    // App Draw Display Elements
    public GridObjectCollection iconCollection;
    public AppIcon iconTemplate;

    // IMenu properties
    public bool Visible { get; private set; }

    public UnityEvent OnOpen;
    public UnityEvent OnClose;

    // Start is called before the first frame update
    void Start()
    {
        CreateAppIcons();
        Close();
    }

    public void OnAppSelect(GameObject iconObject)
    {
        int selectedID = iconObject.GetComponent<AppIcon>().AppID;
        Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            Toggle();
        }
    }

    private void CreateAppIcons()
    {
        IList<BasePervasiveApp> allApps = PervasiveAppRegistry.GetAllApps();
        Debug.Log($"Registry has {allApps.Count} apps");
        
        foreach(BasePervasiveApp app in allApps)
        {
            GameObject appIcon = Instantiate(iconTemplate.gameObject, iconCollection.transform);
            appIcon.GetComponent<AppIcon>().AppRef = app;
            appIcon.SetActive(true);
        }
        iconCollection.GetComponent<GridObjectCollection>().UpdateCollection();
    }

    public void Open()
    {
        Visible = true;
        OnOpen.Invoke();
    }

    public void Close()
    {
        Visible = false;
        OnClose.Invoke();
    }

    public void Toggle()
    {
        if (Visible)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    // private void OpenAppDrawer()
    // {
    //     // Determine position of app drawer
    //     Vector3 start = Camera.main.transform.position;
    //     start += Camera.main.transform.forward * 2.0f;
    //     start -= Camera.main.transform.up * 2.0f;

    //     Vector3 end = Camera.main.transform.position;
    //     end += Camera.main.transform.forward * 1.0f;

    //     // Set up SlideAnimation
    //     var animation = GetComponent<SlideAnimation>();
    //     animation.animateSpeed = 3.0f;
    //     animation.animateStart = start;
    //     animation.animateEnd = end;

    //     // Maintain visual consistency
    //     transform.position = start;
    //     AppUtils.SetLayerRecursive(gameObject, "Default");

    //     animation.StartAnimation();
    // }

    // private void CloseAppDrawer()
    // {
    //     AppUtils.SetLayerRecursive(gameObject, "Ignore Raycast");
    // }


    // public void RequestAppSwitch(int appID, bool animate = false, Action callback = null)
    // {
    //     if (animate)
    //     {
    //         var start = 
    //             appIcons[GameManager.Instance.currAppID].transform.position;
    //         var end = appIcons[appID].transform.position;

    //         var animation = activeIndicator.GetComponent<SlideAnimation>();
    //         animation.animateStart = start;
    //         animation.animateEnd = end;
    //         animation.OnAnimationStop += delegate {
    //             if (callback != null)
    //             {
    //                 callback();
    //             }
    //         };
    //         animation.StartAnimation();
    //     }
    //     else
    //     {
    //         activeIndicator.transform.position =
    //             appIcons[appID].transform.position;
    //         if (callback != null)
    //         {
    //             callback();
    //         }
    //     }
    // }

}
