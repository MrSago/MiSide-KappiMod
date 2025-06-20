﻿using KappiMod.Config;
using KappiMod.Loader;
using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Patches;
using KappiMod.Properties;
using KappiMod.UI.IMGUI;
using KappiMod.UI.Internal.EventDisplay;
using KappiMod.Utils;
using UniverseLib;

namespace KappiMod;

public static class KappiCore
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

        Loader.OnLogMessage($"{BuildInfo.NAME} v{BuildInfo.VERSION} initializing...");

        ConfigManager.Init(Loader.ConfigHandler);

        KappiLogger.Init(Loader, ConfigManager.DebugMode.Value);

        Universe.Init(
            ConfigManager.StartupDelayTime.Value,
            LateInitUI,
            (message, logType) => KappiLogger.Log(message, null, logType),
            new()
            {
                Disable_EventSystem_Override = ConfigManager.DisableEventSystemOverride.Value,
                Force_Unlock_Mouse = ConfigManager.ForceUnlockMouse.Value,
                Unhollowed_Modules_Folder = Loader.UnhollowedModulesDirectory,
            }
        );

        InitUtils();
        InitPatches();
        ModManager.Init();

        KappiLogger.Log($"Steam ID: {SteamHelper.Instance?.GetSteamID()}");
        KappiLogger.Log($"Persona Name: {SteamHelper.Instance?.GetPersonaName()}");
    }

    private static void LateInitUI()
    {
        KappiLogger.Log("Loading UI...");
        UIManager.Init();
        KappiLogger.Log($"{BuildInfo.NAME} v{BuildInfo.VERSION} initialized!");
    }

    private static void InitUtils()
    {
        VersionChecker.CheckForUpdatesAsync();
        SceneTracker.Init();
        EventManager.Init();
    }

    private static void InitPatches()
    {
        IntroSkipPatch.Init();
        NativeResolutionOption.Init();
    }
}
