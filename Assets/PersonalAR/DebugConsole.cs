using UnityEngine;
using System;
using System.IO;
using System.Text;

using TMPro;

using Recaug;

public class DebugConsole : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    public void OnEnable() 
    {
        Application.logMessageReceived += HandlelogMessageReceived;
    }

    public void OnDisable() 
    {
        Application.logMessageReceived -= HandlelogMessageReceived;
    }

    public void HandlelogMessageReceived(string condition, string stackTrace, LogType type)
    {
        var culture = new System.Globalization.CultureInfo("en-US");
        string timestamp = System.DateTime.Now.ToString(culture);

        string msg = "";
        if (type == LogType.Log)
        {
            msg = string.Format("[{0}: {1}] {2}\n", timestamp, type.ToString().ToUpper(), 
                condition);
        }
        else
        {
            msg = string.Format("[{0}: {1}] {2}{3}", timestamp, type.ToString().ToUpper(), 
                condition, "\n    " + stackTrace.Replace ("\n", "\n    "));
        }

        string history = GetComponent<TMPro.TextMeshProUGUI>().text;
        GetComponent<TMPro.TextMeshProUGUI>().text = msg + history;
    }
}