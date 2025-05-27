using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Patches.Core;
using KappiMod.UI.Internal.EventDisplay;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
internal sealed class FixedItemSpawnPatch : IPatch
{
    public string Id => "com.kappimod.fixeditemspawn";
    public string Name => "Fixed Item Spawn Patch";
    public string Description => "Set fixed item spawn positions in Chapter 2";

    // private const string PENCIL_OBJECT_NAME = "ItemPencil";
    // private const string BOW_OBJECT_NAME = "ItemBow";
    // private const string SPOON_OBJECT_NAME = "ItemSpoon";

    // private const int PENCIL_TRANSFORM_INDEX = 5;
    // private const int BOW_TRANSFORM_INDEX = 3;
    // private const int SPOON_TRANSFORM_INDEX = 3;

    private static readonly TransformPositions _pencilTransform = new()
    {
        position = new(new Vector3[1] { new(-7.448f, 0.8886f, 1.95f) }),
        rotation = new(new Vector3[1] { new(90.0f, 0.0f, -351.993f) }),
        target = null,
    };

    private static readonly TransformPositions _bowTransform = new()
    {
        position = new(new Vector3[1] { new(-11.812f, 1.1907f, 2.106f) }),
        rotation = new(new Vector3[1] { new(-80.229f, -204.517f, 86.24f) }),
        target = null,
    };

    private static readonly TransformPositions _spoonTransform = new()
    {
        position = new(new Vector3[1] { new(7.279f, 0.8284f, 2.377f) }),
        rotation = new(new Vector3[1] { new(-90.0f, 0.0f, -102.211f) }),
        target = null,
    };

    private readonly HarmonyLib.Harmony _harmony;

    public FixedItemSpawnPatch()
    {
        _harmony = new(Id);
        _harmony.PatchAll(typeof(FixedItemSpawnPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    [HarmonyPatch(typeof(Location2Main), nameof(Location2Main.Start))]
    private static void Prefix(Location2Main __instance)
    {
        try
        {
            var newTransforms = new Il2CppReferenceArray<TransformPositions>(3);

            newTransforms[0] = _pencilTransform;
            newTransforms[0].target = __instance.items[0].target;

            newTransforms[1] = _bowTransform;
            newTransforms[1].target = __instance.items[1].target;

            newTransforms[2] = _spoonTransform;
            newTransforms[2].target = __instance.items[2].target;

            __instance.items = newTransforms;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set item positions", exception: ex);
            return;
        }

        const string message = "Fixed items positions set";
        EventManager.ShowEvent(new($"{nameof(BlessRng)}: {message}"));
        KappiLogger.Log(message);
    }
}
