using System.Runtime.CompilerServices;
using KappiMod.Config;
using KappiMod.Loader;
using KappiMod.Mods.Core;
using KappiMod.Patches;
using KappiMod.Properties;
using KappiMod.UI;
using KappiMod.Utils;
using UnityEngine;
using UniverseLib;

namespace KappiMod;

public static class KappiModCore
{
    public const string MOD_DIRECTORY_NAME = "KappiMod";

    public static IKappiModLoader Loader { get; private set; } = null!;

    public static void Init(IKappiModLoader loader)
    {
        if (Loader is not null)
        {
            throw new Exception($"{BuildInfo.NAME} is already initialized");
        }

        Loader = loader;

        Log($"{BuildInfo.NAME} v{BuildInfo.VERSION} initializing...");

        ConfigManager.Init(Loader.ConfigHandler);

        Universe.Init(
            ConfigManager.StartupDelayTime.Value,
            LateInitUI,
            Log,
            new()
            {
                Disable_EventSystem_Override = ConfigManager.DisableEventSystemOverride.Value,
                Force_Unlock_Mouse = ConfigManager.ForceUnlockMouse.Value,
                Unhollowed_Modules_Folder = Loader.UnhollowedModulesDirectory,
            }
        );

        InitUtils();
        InitPatches();
        InitMods();
    }

    private static void LateInitUI()
    {
        Log("Loading UI...");

        UIManager.Init();

        Log($"{BuildInfo.NAME} v{BuildInfo.VERSION} initialized!");
    }

    private static void InitUtils()
    {
        VersionChecker.CheckForUpdatesAsync();
        PreviousSceneTracker.Init();
    }

    private static void InitPatches()
    {
        DialoguePatcher.Init();
        IntroSkipper.Init();
        NativeResolutionOption.Init();
    }

    private static void InitMods()
    {
        Log("Initializing mod system...");
        ModManager.Initialize();
        Log($"Mod system initialized with {ModManager.RegisteredMods.Count} mods");
    }

    #region LOGGING

    public static void Log(object? message, LogType logType) => InternalLog(message, logType);

    public static void Log(object? message, [CallerFilePath] string? callerFilePath = null) =>
        InternalLog(message, LogType.Log, callerFilePath);

    public static void LogWarning(
        object? message,
        [CallerFilePath] string? callerFilePath = null
    ) => InternalLog(message, LogType.Warning, callerFilePath);

    public static void LogError(object? message, [CallerFilePath] string? callerFilePath = null) =>
        InternalLog(message, LogType.Error, callerFilePath);

    private static void InternalLog(
        object? message,
        LogType logType = LogType.Log,
        string? callerFilePath = null
    )
    {
        string log;
        if (callerFilePath is not null)
        {
            string callerClassName = Path.GetFileNameWithoutExtension(callerFilePath);
            log = $"[{callerClassName}] {message?.ToString() ?? string.Empty}";
        }
        else
        {
            log = message?.ToString() ?? string.Empty;
        }

        switch (logType)
        {
            case LogType.Log:
            case LogType.Assert:
                Loader.OnLogMessage(log);
                break;

            case LogType.Warning:
                Loader.OnLogWarning(log);
                break;

            case LogType.Error:
            case LogType.Exception:
                Loader.OnLogError(log);
                break;

            default:
                break;
        }
    }

    #endregion LOGGING
}
