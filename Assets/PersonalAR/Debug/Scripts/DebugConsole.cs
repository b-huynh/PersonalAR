using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    // Provide reference in Editor
    [SerializeField] private TMPro.TextMeshProUGUI textMesh;

    private OrderedDictionary logStringCount;     // string/int pairs
    private OrderedDictionary stackTraceCache;     // string/string pairs

    [Tooltip("Has significant impact to performance, use ARDebug to minimize performance impact")]
    public bool HandleUnityDebugLogMessages;

    [Range(10, 1000)]
    public int TruncateLength;

    public string ConsoleOutput
    {
        get => textMesh.text;
    }

    void Awake()
    {
        logStringCount = new OrderedDictionary();
        stackTraceCache = new OrderedDictionary();

        if (HandleUnityDebugLogMessages)
        {
            Application.logMessageReceived += HandlelogMessageReceived;
        }
        ARDebug.logMessageReceived += HandlelogMessageReceived;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnEnable() 
    {

    }

    public void OnDisable() 
    {
        if (HandleUnityDebugLogMessages)
        {
            Application.logMessageReceived -= HandlelogMessageReceived;
        }
        ARDebug.logMessageReceived -= HandlelogMessageReceived;

        logStringCount?.Clear();
        stackTraceCache?.Clear();
    }

    public void SetLogToUnityConsole(bool value)
    {
        ARDebug.logToUnityConsole = value;
    }

    public void HandlelogMessageReceived(string logString, string stackTrace, LogType type)
    {
        // Ignore warnings.
        if (type == LogType.Warning) { return; }

        // Format stackTrace with timestamp and proper indentation.
        var culture = new System.Globalization.CultureInfo("en-US");
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss", culture);
        if (type == LogType.Log)
        {
            logString = $"({type.ToString().ToUpper()}) {logString}";
            stackTrace = $"[{timestamp}]";
        }
        else
        {
            logString = $"({type.ToString().ToUpper()}) {logString}";
            stackTrace = $"[{timestamp}]\n {stackTrace}";
        }

        // Cache log counts and stackTraces
        if (!logStringCount.Contains(logString))
        {
            logStringCount.Add(logString, 0);
            stackTraceCache.Add(logString, "");
        }
        logStringCount[logString] = (int)logStringCount[logString] + 1;
        stackTraceCache[logString] = stackTrace;

        textMesh.text = GetConsoleOutput();
    }

    private string TruncateStackTrace(string stackTrace, int numChars)
    {
        return stackTrace.Length > numChars ? stackTrace.Substring(0, numChars) + "..." : stackTrace; 
    }

    private string GetConsoleOutput()
    {
        string output = "";
        foreach(DictionaryEntry de in logStringCount)
        {
            string logString = (string)de.Key;
            int logCount = (int)de.Value;
            string stackTrace = (string)stackTraceCache[logString];
            string truncatedStackTrace = TruncateStackTrace(stackTrace, TruncateLength);
            output += $"({logCount}) {logString} {truncatedStackTrace}\n";
        }
        return output;
    }
}