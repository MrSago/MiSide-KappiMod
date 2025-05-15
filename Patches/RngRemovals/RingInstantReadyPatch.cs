using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Patches.Core;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.RngRemovals;

[HarmonyPatch]
internal sealed class RingInstantReadyPatch : IPatch
{
    public string Id => "com.kappimod.ringinstantready";
    public string Name => "Ring Instant Ready Patch";
    public string Description => "Skips the ring wait event and instantly makes the ring ready";

    private readonly HarmonyLib.Harmony _harmony;

    public RingInstantReadyPatch()
    {
        _harmony = new HarmonyLib.Harmony(Id);
        _harmony.PatchAll(typeof(RingInstantReadyPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPatch(typeof(Location7_RingWork), "Start")]
    private static void Postfix(Location7_RingWork __instance)
    {
        try
        {
            __instance.ReadyTime();
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set ring wait event", exception: ex);
            return;
        }

        KappiLogger.Log("Ring wait event skipped");
    }
}
