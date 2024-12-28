﻿using System.Runtime.CompilerServices;
using KappiMod.Config;
using KappiMod.Loader;
using KappiMod.Mods;
using KappiMod.Patches;
using KappiMod.Properties;
using KappiMod.UI;
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

        InitMods();
        InitPatches();
    }

    private static void LateInitUI()
    {
        Log("Loading UI...");

        UIManager.Init();

        Log($"{BuildInfo.NAME} v{BuildInfo.VERSION} initialized!");
    }

    private static void InitMods()
    {
        ConsoleUnlocker.Init();
        FlashlightIncreaser.Init();
        FpsLimit.Init();
        SitUnlocker.Init();
        SprintUnlocker.Init();
        TimeScaleScroller.Init();
    }

    private static void InitPatches()
    {
        DialogueSkipper.Init();
    }

    #region LOGGING

    public static void Log(object message, LogType logType) => InternalLog(message, null, logType);

    public static void Log(object message, [CallerFilePath] string? callerFilePath = null) =>
        InternalLog(message, callerFilePath, LogType.Log);

    public static void LogWarning(object message, [CallerFilePath] string? callerFilePath = null) =>
        InternalLog(message, callerFilePath, LogType.Warning);

    public static void LogError(object message, [CallerFilePath] string? callerFilePath = null) =>
        InternalLog(message, callerFilePath, LogType.Error);

    private static void InternalLog(
        object? message,
        string? callerFilePath = null,
        LogType logType = LogType.Log
    )
    {
        string log;
        if (callerFilePath is not null)
        {
            string callerClassName = Path.GetFileNameWithoutExtension(callerFilePath);
            log = $"[{callerClassName}] {message?.ToString() ?? ""}";
        }
        else
        {
            log = message?.ToString() ?? "";
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

    #endregion
}
