using HarmonyLib;
using KappiMod.Constants;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Patches.Core;
using KappiMod.UI.Internal.EventDisplay;
using KappiMod.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [HarmonyPatch(typeof(ObjectInteractive), nameof(ObjectInteractive.OnDisable))]
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
        if (sceneName is not SceneName.CHIBIMITA_SCENE)
        {
            return;
        }

        bool found = TryFindDoor();

        string message = "Chibi door " + (found ? "found" : "not found");
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {message}"));
        KappiLogger.Log(message);
    }

    private static void UnlockDoor()
    {
        if (!TryFindDoor() || _cachedDoor == null)
        {
            KappiLogger.LogError($"Object {nameof(ObjectDoor)} not found!");
            return;
        }

        _cachedDoor.lockDoor = false;

        const string message = "Chibi door unlocked";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {message}"));
        KappiLogger.Log(message);
    }

    private static Transform? GetRootTransform()
    {
        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name is SceneName.WORLD_ROOT_NAME)
            {
                return root.transform;
            }
        }

        return null;
    }

    private static bool TryFindDoor()
    {
        if (Helpers.IsValid(_cachedDoor))
        {
            return true;
        }

        _cachedDoor = GetRootTransform()?.Find(DOOR_PATH)?.gameObject?.GetComponent<ObjectDoor>();
        return Helpers.IsValid(_cachedDoor);
    }
}
