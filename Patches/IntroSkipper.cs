using HarmonyLib;
using KappiMod.Config;
using KappiMod.Logging;
using KappiMod.Utils;
using UnityEngine;
using UnityEngine.Playables;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches;

public static class IntroSkipper
{
    private static bool _isInitialized = false;
    private static HarmonyLib.Harmony _harmony = null!;

    public static bool Enabled
    {
        get => _isInitialized && ConfigManager.IntroSkipper.Value;
        set
        {
            if (!_isInitialized || value == Enabled)
            {
                return;
            }

            if (value)
            {
                KappiModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
            }
            else
            {
                KappiModCore.Loader.SceneWasInitialized -= OnSceneWasInitialized;
            }

            KappiLogger.Log(value ? "Enabled" : "Disabled");
            ConfigManager.IntroSkipper.Value = value;
        }
    }

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiLogger.LogError($"{nameof(IntroSkipper)} is already initialized");
            return;
        }

        _isInitialized = true;

        if (Enabled)
        {
            KappiModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
        }

        _harmony = new("com.kappimod.introskipper");
        _harmony.PatchAll(typeof(Patch));

        KappiLogger.Log("Initialized");
    }

    private static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is not ObjectNames.AIHASTO_INTRO_SCENE)
        {
            return;
        }

        try
        {
            if (SkipIntro())
            {
                KappiLogger.Log("Aihasto intro skipped");
            }
            else
            {
                KappiLogger.LogWarning("Aihasto intro not skipped");
            }
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to skip intro", exception: ex);
        }
    }

    private static bool SkipIntro()
    {
        PlayableDirector? playableDirector = GameObject
            .Find("Scene")
            ?.GetComponent<PlayableDirector>();
        if (playableDirector == null)
        {
            return false;
        }

        playableDirector.time = playableDirector.duration;

        return true;
    }

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPatch(typeof(Menu), "Start")]
        private static void Postfix(Menu __instance)
        {
            if (!Enabled)
            {
                return;
            }

            if (PreviousSceneTracker.Name is ObjectNames.ENDING_GAME_SCENE)
            {
                return;
            }

            InvokeSkipEvent(__instance);
            KappiLogger.Log("The opening menu cutscene should be skipped");
        }

        private static void InvokeSkipEvent(Menu __instance)
        {
            try
            {
                __instance.eventSkip.Invoke();
                __instance.SkipStart();
            }
            catch
            {
                /*
                    __instance.SkipStart() throws an exception
                    but it works anyway and we ignore this exception
                */
            }
        }
    }
}
