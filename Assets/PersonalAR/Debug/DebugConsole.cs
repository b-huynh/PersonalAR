using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
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
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss", culture);
        string stackFormatted = stackTrace.Replace ("\n", "\n    ");

        string msg = "";
        if (type == LogType.Log)
        {
            msg = $"[{timestamp} {type.ToString().ToUpper()}] {condition}\n";
        }
        else
        {
            msg = $"[{timestamp} {type.ToString().ToUpper()}] {condition}\n {stackFormatted}\n";
        }

        string history = GetComponent<TMPro.TextMeshProUGUI>().text;
        GetComponent<TMPro.TextMeshProUGUI>().text = msg + history;
    }
}