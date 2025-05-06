using KappiMod.Config;
using UnityEngine;

namespace KappiMod.Mods;

public static class FpsLimit
{
    private static bool _isInitialized = false;

    public static int CurrentFpsLimit
    {
        get => ConfigManager.FpsLimit.Value;
        private set => ConfigManager.FpsLimit.Value = value;
    }

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiModCore.LogError($"{nameof(FpsLimit)} is already initialized");
            return;
        }

        KappiModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;

        _isInitialized = true;
        KappiModCore.Log("Initialized");
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

            KappiModCore.Log($"FPS limit set to {(fps < 0 ? "unlimited" : fps.ToString())}");

            CurrentFpsLimit = fps;
        }
        catch (Exception e)
        {
            KappiModCore.LogError(e.Message);
        }
    }

    private static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is not ObjectNames.MAIN_MENU_SCENE)
        {
            return;
        }

        SetFpsLimit(ConfigManager.FpsLimit.Value);
    }
}
