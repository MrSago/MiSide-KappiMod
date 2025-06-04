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
internal sealed class PassableDummiesPatch : IPatch
{
    public string Id => "com.kappimod.passabledummies";
    public string Name => "Passable Dummies Patch";
    public string Description => "Makes all dummies passable, removing the RNG from mini-game";

    private readonly HarmonyLib.Harmony _harmony;

    public PassableDummiesPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(PassableDummiesPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MakeManeken_Main), nameof(MakeManeken_Main.SiwtchGet))]
    private static void SetDummyProperties(MakeManeken_Main __instance, bool x)
    {
        try
        {
            if (x is true)
            {
                __instance.badIndexAnimation = 0;
                __instance.indexBedManeken = 0;
            }
            else
            {
                __instance.badIndexAnimation = 1;
                __instance.indexBedManeken = 1;
            }

            const string MESSAGE = "Dummy properties set";
            EventManager.ShowEvent(new($"{nameof(BlessRng)}: {MESSAGE}"));
            KappiLogger.Log(MESSAGE);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set dummy properties", exception: ex);
        }
    }
}
