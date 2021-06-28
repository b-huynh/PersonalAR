using UnityEngine;

public static class ARDebug
{
    public static bool logToUnityConsole { get; set; }

    public static event Application.LogCallback logMessageReceived;

    private static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
    {
        if (logToUnityConsole)
        {
            Debug.LogFormat(logType, logOptions, context, format, args);
        }

        logMessageReceived?.Invoke(string.Format(format, args), string.Empty, logType);
    }
    public static void LogFormat(Object context, string format, params object[] args)
    {
        LogFormat(LogType.Log, LogOption.None, context, format, args);
    }
    public static void LogFormat(string format, params object[] args)
    {
        LogFormat(null, format, args);
    }

    public static void Log(object message, Object context)
    {
        LogFormat(context, message.ToString());
    }
    public static void Log(object message) => Log(message, null);

    public static void LogWarning(object message, Object context)
    {
        LogFormat(LogType.Warning, LogOption.None, context, message.ToString());
    }
    public static void LogWarning(object message) => LogWarning(message, null);

    public static void LogError(object message, Object context)
    {
        LogFormat(LogType.Error, LogOption.None, context, message.ToString());
    }
    public static void LogError(object message) => LogError(message, null);

    public static void LogException(System.Exception ex, Object context)
    {
        LogFormat(LogType.Exception, LogOption.None, context, ex.ToString());
    }
    public static void LogException(System.Exception ex) => LogException(ex, null);

    public static void LogAssertion(object message, Object context)
    {
        LogFormat(LogType.Assert, LogOption.None, context, message.ToString());
    }
    public static void LogAssertion(object message) => LogAssertion(message, null);
}
