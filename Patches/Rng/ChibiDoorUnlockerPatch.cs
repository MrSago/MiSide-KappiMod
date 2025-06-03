using HarmonyLib;
using KappiMod.Constants;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Patches.Core;
using KappiMod.UI.Internal.EventDisplay;
using KappiMod.Utils;
using UniverseLib.Utility;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class ChibiDoorUnlockerPatch : IPatch
{
    public string Id => "com.kappimod.chibidoorunlocker";
    public string Name => "Chibi Door Unlocker";
    public string Description => "Unlocks the door without catching chibi guys";

    private const string DOOR_PATH = "House/Doors/DoorCage ChibiPlayers - NextLadder/DoorPhysic";

    private static ObjectDoor? _cachedDoor;
    private readonly HarmonyLib.Harmony _harmony;

    public ChibiDoorUnlockerPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(ChibiDoorUnlockerPatch));

        KappiCore.Loader.SceneWasLoaded += OnSceneWasLoaded;
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();

        KappiCore.Loader.SceneWasLoaded -= OnSceneWasLoaded;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ObjectInteractive), nameof(ObjectInteractive.OnDisable))]
    private static void ChibiDoorUnlock(ObjectInteractive __instance)
    {
        try
        {
            if (__instance.gameObject.name is not "BoxChibi")
            {
                return;
            }

            UnlockDoor();

            const string message = "Chibi door unlocked";
            EventManager.ShowEvent(new($"{nameof(BlessRng)}: {message}"));
            KappiLogger.Log(message);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to open door", exception: ex);
        }
    }

    private static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName is not SceneName.CHIBIMITA)
        {
            return;
        }

        try
        {
            bool found = TryFindDoor();

            string message = "Chibi door " + (found ? "found" : "not found");
            EventManager.ShowEvent(new($"{nameof(BlessRng)}: {message}"));
            KappiLogger.Log(message);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to check door on scene load", exception: ex);
        }
    }

    private static void UnlockDoor()
    {
        if (!TryFindDoor() || _cachedDoor == null)
        {
            KappiLogger.LogError($"Object {nameof(ObjectDoor)} not found!");
            return;
        }

        _cachedDoor.lockDoor = false;
    }

    private static bool TryFindDoor()
    {
        if (!UnityHelpers.IsNullOrDestroyed(_cachedDoor))
        {
            return true;
        }

        _cachedDoor = Helpers
            .GetRootTransform()
            ?.Find(DOOR_PATH)
            ?.gameObject?.GetComponent<ObjectDoor>();

        return !UnityHelpers.IsNullOrDestroyed(_cachedDoor);
    }
}
