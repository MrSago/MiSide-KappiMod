using KappiMod.Config;
using KappiMod.Constants;
using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Patches;
using KappiMod.Patches.Core;
using KappiMod.Properties;
using UnityEngine;
using UnityEngine.Playables;

namespace KappiMod.Mods;

[ModInfo(
    name: "Intro Skipper",
    description: "Skips the intro scene in the game",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public sealed class IntroSkipper : BaseMod
{
    public override bool IsEnabled
    {
        get => base.IsEnabled && ConfigManager.IntroSkipPatch.Value;
        protected set
        {
            base.IsEnabled = value;
            ConfigManager.IntroSkipPatch.Value = value;
        }
    }

    private readonly PatchManager _patchManager = new();

    protected override void OnInitialize()
    {
        if (ConfigManager.IntroSkipPatch.Value)
        {
            OnEnable();
            base.IsEnabled = true;
        }
    }

    protected override void OnEnable()
    {
        _patchManager.RegisterPatch<IntroSkipPatch>();
        KappiCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
    }

    protected override void OnDisable()
    {
        _patchManager.Dispose();
        KappiCore.Loader.SceneWasInitialized -= OnSceneWasInitialized;
    }

    private static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is not SceneName.AIHASTO_INTRO)
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
}
