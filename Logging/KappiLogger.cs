using System.Runtime.CompilerServices;
using System.Text;
using KappiMod.Loader;
using UnityEngine;

namespace KappiMod.Logging;

public static class KappiLogger
{
    private static IKappiModLoader? _loader;
    private static bool _debugMode = false;

    public static void Init(IKappiModLoader loader, bool debugMode = false)
    {
        if (_loader is not null)
        {
            throw new Exception($"{nameof(KappiLogger)} is already initialized");
        }

        _loader = loader;
        _debugMode = debugMode;
    }

    public static void LogWarning(object? message, [CallerFilePath] string? prefix = null) =>
        Log(message, prefix, LogType.Warning);

    public static void LogError(object? message, [CallerFilePath] string? prefix = null) =>
        Log(message, prefix, LogType.Error);

    public static void LogException(
        object? message,
        [CallerFilePath] string? prefix = null,
        Exception? exception = null
    ) => Log(message, prefix, LogType.Exception, exception);

    public static void Log(
        object? message,
        [CallerFilePath] string? prefix = null,
        LogType logType = LogType.Log,
        Exception? exception = null
    )
    {
        if (logType is LogType.Assert && !_debugMode)
        {
            return;
        }

        StringBuilder sb = new(message?.ToString() ?? string.Empty);
        if (!string.IsNullOrEmpty(prefix))
        {
            string callerClassName = Path.GetFileNameWithoutExtension(prefix);
            sb.Insert(0, $"[{callerClassName}] ");
        }
        if (exception is not null)
        {
            sb.Append($"\nException: {exception.Message}\n{exception.StackTrace}");
        }

        string logMessage = sb.ToString();

        if (_loader is null)
        {
            Debug.Log(logMessage);
            return;
        }

        switch (logType)
        {
            case LogType.Log:
            case LogType.Assert:
                _loader.OnLogMessage(logMessage);
                break;

            case LogType.Warning:
                _loader.OnLogWarning(logMessage);
                break;

            case LogType.Error:
            case LogType.Exception:
                _loader.OnLogError(logMessage);
                break;

            default:
                break;
        }
    }
}
