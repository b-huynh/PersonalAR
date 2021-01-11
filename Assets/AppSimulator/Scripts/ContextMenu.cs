using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.UI;

using Recaug;

public class ContextMenu : MonoBehaviour
{
    public GameObject appIconPrefab;

    private Dictionary<int, GameObject> availableApps =
        new Dictionary<int, GameObject>();

    public List<Vector3> appIconPositions = new List<Vector3> {
        new Vector3(-2.5f, -1.25f, 0),
        new Vector3(-2.5f, 0, 0),
        new Vector3(-2.5f, -2.5f, 0)
    };

    public bool isOpen { get; private set; } = false;

    // public static bool showAll = false;

    // public bool show = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get Object Name
        var reg = GetComponentInParent<ObjectRegistration>();
        var textMesh = transform.Find("Prompt").GetComponent<TextMesh>();
        textMesh.text = string.Format("( {0} )\nOpen With", reg.className);

        Close();
    }

    // Update is called once per frame
    void Update()
    {
        // bool render = show || showAll;

        // if (render)
        // {
        //     UpdateApplist();
        // }
        // else
        // {
        //     foreach(var renderer in GetComponentsInChildren<Renderer>())
        //     {
        //         if (renderer.gameObject.name != "LockOn")
        //         {
        //             renderer.enabled = false;
        //         }
        //     }
        // }
    }

    private void UpdateButtonPositions()
    {
        int idx = 0;
        foreach(var kv in availableApps)
        {
            kv.Value.transform.localPosition = appIconPositions[idx];
            idx = Math.Min(idx + 1, appIconPositions.Count - 1);
        }
    }

    public void OnClick()
    {
        if (isOpen) Close();
        else Open();
    }

    public void Open()
    {
        if (GameManager.Instance.config.Experiment.Multitasking != MultitaskingType.InSitu &&
            GameManager.Instance.config.Experiment.Multitasking != MultitaskingType.ManualInSitu)
        {
            return; // Context menu not enabled without these multitasking options
        }

        // foreach(var renderer in GetComponentsInChildren<Renderer>())
        // {
        //     renderer.enabled = true;
        // }


        transform.Find("ButtonGroup").gameObject.SetActive(true);
        transform.Find("Prompt").gameObject.SetActive(true);
        transform.Find("UI Plane").gameObject.SetActive(true);
        isOpen = true;

        string className = GetComponentInParent<ObjectRegistration>().className;
        StatsTracker.Instance.LogContextMenuOpen(className);
    }

    public void Close()
    {
        // foreach(var renderer in GetComponentsInChildren<Renderer>())
        // {
        //     renderer.enabled = false;
        // }

        transform.Find("ButtonGroup").gameObject.SetActive(false);
        transform.Find("Prompt").gameObject.SetActive(false);
        transform.Find("UI Plane").gameObject.SetActive(false);
        isOpen = false;

        string className = GetComponentInParent<ObjectRegistration>().className;
        StatsTracker.Instance.LogContextMenuClose(className);
    }

    public static void OpenAll()
    {
        GameObject.FindObjectsOfType<ContextMenu>().ToList().ForEach(
            menu => menu.Open());
    }

    public static void CloseAll()
    {
        GameObject.FindObjectsOfType<ContextMenu>().ToList().ForEach(
            menu => menu.Close());
    }

    public void AddApp(int appID)
    {
        if (!availableApps.ContainsKey(appID))
        {
            var registration = 
                transform.parent.gameObject.GetComponent<ObjectRegistration>();

            Transform buttonGroup = transform.Find("ButtonGroup");

            var appIcon = GameObject.Instantiate(appIconPrefab, buttonGroup);
            appIcon.layer = LayerMask.NameToLayer("UI");

            appIcon.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            appIcon.transform.localScale = new Vector3(1.0f, 1.0f, 0.2f);
            appIcon.GetComponent<AppIcon>().AppID = appID;

            appIcon.GetComponent<Interactable>().OnClick.AddListener(delegate {
                GameManager.Instance.RequestAppFocus(appID, registration);
                CloseAll();
            });

            availableApps[appID] = appIcon;

            UpdateButtonPositions();
        }
    }

    public void RemoveApp(int appID)
    {
        if (availableApps.ContainsKey(appID))
        {
            // GameObject.Destroy(availableApps[appID]);
            availableApps[appID].SetActive(false);
            availableApps.Remove(appID);

            if(availableApps.Count == 0)
            {
                // Disable further interaction with this object... somehow
                var pObj = transform.parent.gameObject;
            }

            UpdateButtonPositions();
        }
    }
}
