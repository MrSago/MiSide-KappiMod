using KappiMod.Config;
using UnityEngine;

namespace KappiMod.Mods;

public static class FpsLimit
{
    public static int CurrentFpsLimit
    {
        get => ConfigManager.FpsLimit.Value;
        private set => ConfigManager.FpsLimit.Value = value;
    }

    public static void Init()
    {
        KappiModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;

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
