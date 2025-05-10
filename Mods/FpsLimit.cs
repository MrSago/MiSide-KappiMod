using KappiMod.Config;
using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Properties;
using UnityEngine;

namespace KappiMod.Mods;

[ModInfo(
    name: "FPS Limit",
    description: "Sets the FPS limit for the game.",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public sealed class FpsLimit : BaseMod
{
    public override bool IsEnabled => true;

    public static int CurrentFpsLimit
    {
        get => ConfigManager.FpsLimit.Value;
        private set => ConfigManager.FpsLimit.Value = value;
    }

    protected override void OnInitialize()
    {
        if (ConfigManager.FpsLimit.Value < 0)
        {
            ConfigManager.FpsLimit.Value = -1;
        }

        OnEnable();
    }

    protected override void OnEnable()
    {
        KappiCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
    }

    protected override void OnDisable()
    {
        KappiCore.Loader.SceneWasInitialized -= OnSceneWasInitialized;
    }

    public static void SetFpsLimit(int fpsLimit)
    {
        try
        {
            int fps = fpsLimit < 0 ? -1 : Mathf.Max(10, fpsLimit);
            if (Application.targetFrameRate == fps)
            {
                return;
            }

            Application.targetFrameRate = fps;
            KappiLogger.Log($"FPS limit set to {(fps < 0 ? "unlimited" : fps.ToString())}");
            CurrentFpsLimit = fps;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set FPS limit", exception: ex);
        }
    }

    private void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is not ObjectNames.MAIN_MENU_SCENE)
        {
            return;
        }

        SetFpsLimit(ConfigManager.FpsLimit.Value);
    }
}
