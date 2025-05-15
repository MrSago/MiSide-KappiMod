using HarmonyLib;
using KappiMod.Logging;
using KappiMod.Patches.Core;
using KappiMod.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.RngRemovals;

[HarmonyPatch]
internal sealed class NoChibiDoorUnlockerPatch : IPatch
{
    public string Id => "com.kappimod.nochibidoorunlocker";
    public string Name => "No Chibi Door Unlocker";
    public string Description => "Unlocks the door without catching chibi guys";

    private const string DOOR_PATH = "House/Doors/DoorCage ChibiPlayers - NextLadder/DoorPhysic";

    private static ObjectDoor? _cachedDoor;
    private readonly HarmonyLib.Harmony _harmony;

    public NoChibiDoorUnlockerPatch()
    {
        _harmony = new HarmonyLib.Harmony(Id);
        _harmony.PatchAll(typeof(NoChibiDoorUnlockerPatch));

        KappiCore.Loader.SceneWasLoaded += OnSceneWasLoaded;
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();

        KappiCore.Loader.SceneWasLoaded -= OnSceneWasLoaded;
    }

    [HarmonyPatch(typeof(ObjectInteractive), "OnDisable")]
    private static void Postfix(ObjectInteractive __instance)
    {
        try
        {
            if (__instance.gameObject.name is not "BoxChibi")
            {
                return;
            }

            UnlockDoor();
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to open door", exception: ex);
            return;
        }
    }

    private static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName is not ObjectNames.BACKROOMS_SCENE)
        {
            return;
        }

        bool found = TryFindDoor();
        KappiLogger.Log("Door " + (found ? "found" : "not found"));
    }

    private static void UnlockDoor()
    {
        if (!TryFindDoor() || _cachedDoor == null)
        {
            KappiLogger.LogError($"Object {nameof(ObjectDoor)} not found!");
            return;
        }

        _cachedDoor.lockDoor = false;
        KappiLogger.Log("Door unlocked");
    }

    private static Transform? GetRootTransform()
    {
        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name is ObjectNames.WORLD_ROOT_NAME)
            {
                return root.transform;
            }
        }

        return null;
    }

    private static bool TryFindDoor()
    {
        if (UnityHelpers.IsValid(_cachedDoor))
        {
            return true;
        }

        _cachedDoor = GetRootTransform()?.Find(DOOR_PATH)?.gameObject?.GetComponent<ObjectDoor>();
        return UnityHelpers.IsValid(_cachedDoor);
    }
}
