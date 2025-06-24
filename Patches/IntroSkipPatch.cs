using HarmonyLib;
using KappiMod.Constants;
using KappiMod.Logging;
using KappiMod.Patches.Core;
using KappiMod.Utils;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches;

[HarmonyPatch]
public sealed class IntroSkipPatch : IPatch
{
    public string Id => "com.kappimod.introskippatch";
    public string Name => "Intro Skip Patch";
    public string Description => "Skips the intro scene in the game";

    private HarmonyLib.Harmony _harmony = null!;

    public IntroSkipPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(IntroSkipPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Menu), nameof(Menu.Start))]
    private static void MenuStartSkipHandler(Menu __instance)
    {
        if (SceneTracker.Name is SceneName.ENDING_GAME)
        {
            return;
        }

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

        KappiLogger.Log("The opening menu cutscene should be skipped");
    }
}
