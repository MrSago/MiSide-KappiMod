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

namespace KappiMod.Patches.RngRemovals;

[HarmonyPatch]
internal sealed class GoodManekenPatch : IPatch
{
    public string Id => "com.kappimod.goodmaneken";
    public string Name => "Good Maneken Patch";
    public string Description => "Makes all manekens good by removing the RNG from mini-game";

    private readonly HarmonyLib.Harmony _harmony;

    public GoodManekenPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(GoodManekenPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPatch(typeof(MakeManeken_Main), nameof(MakeManeken_Main.SiwtchGet))]
    private static void Prefix(MakeManeken_Main __instance)
    {
        try
        {
            __instance.badIndexAnimation = 0;
            __instance.indexBedManeken = 0;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set maneken properties", exception: ex);
            return;
        }

        const string message = "Good maneken properties set";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {message}"));
        KappiLogger.Log(message);
    }
}
