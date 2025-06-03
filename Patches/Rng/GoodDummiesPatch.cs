using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Patches.Core;
using KappiMod.UI.Internal.EventDisplay;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class GoodDummiesPatch : IPatch
{
    public string Id => "com.kappimod.gooddummies";
    public string Name => "Good Dummies Patch";
    public string Description => "Makes all dummies good, removing the RNG from mini-game.";

    private readonly HarmonyLib.Harmony _harmony;

    public GoodDummiesPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(GoodDummiesPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MakeManeken_Main), nameof(MakeManeken_Main.SiwtchGet))]
    private static void InitializeDummyProperties(MakeManeken_Main __instance)
    {
        try
        {
            __instance.badIndexAnimation = 0;
            __instance.indexBedManeken = 0;

            const string message = "Good dummies properties set";
            EventManager.ShowEvent(new($"{nameof(BlessRng)}: {message}"));
            KappiLogger.Log(message);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set dummies properties", exception: ex);
        }
    }
}
