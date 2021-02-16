using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Utilities;

using Recaug;

public class AppDrawerMenu : Singleton<AppTabs>
{
    public GridObjectCollection iconCollection;
    public GameObject iconTemplate;

    public List<GameObject> appIcons;
    public GameObject activeIndicator;

    private bool requestDrawerOpen = false;
    private bool requestDrawerClose = false;

    public bool appDrawerOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        SetInteractive(false);
    }

    public void OnAppSelect(GameObject iconObject)
    {
        int selectedID = iconObject.GetComponent<AppIcon>().AppID;
        if (appDrawerOpen)
        {
            // GameManager.Instance.SwitchApp(selectedID, false);
            GameManager.Instance.RequestAppFocus(selectedID);
            RequestDrawerClose();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.config.Experiment.Multitasking != MultitaskingType.Manual &&
            GameManager.Instance.config.Experiment.Multitasking != MultitaskingType.ManualInSitu)
        {
            return; // Not enabled
        }

        // Maintain activeIndicator position
        if (GameManager.Instance.currAppID != -1)
        {
            Vector3 indicatorPos = appIcons[GameManager.Instance.currAppID].transform.position;
            indicatorPos.y += -0.1f;
            activeIndicator.transform.position = indicatorPos;
        }

        if (requestDrawerOpen)
        {
            OpenAppDrawer();
            requestDrawerOpen = false;
        }
        if (requestDrawerClose)
        {
            CloseAppDrawer();
            requestDrawerClose = false;
        }

        // if (GameManager.Instance.LongPressed)
        if (Input.GetButtonDown("Menu"))
        {
            if (!appDrawerOpen)
            {
                RequestDrawerOpen();
                StatsTracker.Instance.LogMenuOpen();
            }
            else
            {
                RequestDrawerClose();
                StatsTracker.Instance.LogMenuClose();
            }
        }
    }

    private void CreateAppIcons()
    {
        IList<BasePervasiveApp> allApps = PervasiveAppRegistry.GetAllApps();
        foreach(BasePervasiveApp app in allApps)
        {
             
        }
    }

    public void RequestAppSwitch(int appID, bool animate = false, Action callback = null)
    {
        if (animate)
        {
            var start = 
                appIcons[GameManager.Instance.currAppID].transform.position;
            var end = appIcons[appID].transform.position;

            var animation = activeIndicator.GetComponent<SlideAnimation>();
            animation.animateStart = start;
            animation.animateEnd = end;
            animation.OnAnimationStop += delegate {
                if (callback != null)
                {
                    callback();
                }
            };
            animation.StartAnimation();
        }
        else
        {
            activeIndicator.transform.position =
                appIcons[appID].transform.position;
            if (callback != null)
            {
                callback();
            }
        }
    }

    public void RequestDrawerOpen()
    {
        requestDrawerOpen = true;
    }

    public void RequestDrawerClose()
    {
        requestDrawerClose = true;
    }

    private void OpenAppDrawer()
    {
        // Determine position of app drawer
        Vector3 start = Camera.main.transform.position;
        start += Camera.main.transform.forward * 2.0f;
        start -= Camera.main.transform.up * 2.0f;

        Vector3 end = Camera.main.transform.position;
        end += Camera.main.transform.forward * 1.0f;

        // Set up SlideAnimation
        var animation = GetComponent<SlideAnimation>();
        animation.animateSpeed = 3.0f;
        animation.animateStart = start;
        animation.animateEnd = end;
        animation.OnAnimationStop += delegate {
            appDrawerOpen = true;
        };

        // Maintain visual consistency
        transform.position = start;
        // SetRenderActive(true);
        SetInteractive(true);
        animation.StartAnimation();
    }

    private void CloseAppDrawer()
    {
        // SetRenderActive(false);
        SetInteractive(false);
        appDrawerOpen = false;
    }

    public void SetInteractive(bool val)
    {
        transform.Find("Header").gameObject.SetActive(val);
        transform.Find("ActiveIndicator").gameObject.SetActive(val);
        transform.Find("App Grid").gameObject.SetActive(val);
    }
}
