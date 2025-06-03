using HarmonyLib;
using KappiMod.Events;
using KappiMod.Logging;
using KappiMod.Patches.Core;
using UniverseLib.Utility;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches;

[HarmonyPatch]
public sealed class DialogueStartPatch : IPatch
{
    public string Id => "com.kappimod.dialoguestartpatch";
    public string Name => "Dialogue Start Patch";
    public string Description => "Patches dialogue events to allow for custom handling";

    public event EventHandler<DialogueEventArgs>? OnPrefixDialogueStart;
    public event EventHandler<DialogueEventArgs>? OnPostfixDialogueStart;

    private readonly HarmonyLib.Harmony _harmony;

    private static DialogueStartPatch? _instance;

    public DialogueStartPatch()
    {
        _instance = this;

        _harmony = new(Id);
        _harmony.PatchAll(typeof(Patch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();

        _instance = null;
    }

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Dialogue_3DText), nameof(Dialogue_3DText.Start))]
        private static void OnDialogueStartPrefix(Dialogue_3DText __instance)
        {
            if (UnityHelpers.IsNullOrDestroyed(__instance))
            {
                return;
            }

            try
            {
                var args = DialogueEventArgs.Create(__instance, DialoguePatchType.Prefix);
                _instance?.OnPrefixDialogueStart?.Invoke(_instance, args);
            }
            catch (Exception ex)
            {
                KappiLogger.LogException("Failed to process prefix start dialogue", exception: ex);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Dialogue_3DText), nameof(Dialogue_3DText.Start))]
        private static void OnDialogueStartPostfix(Dialogue_3DText __instance)
        {
            if (UnityHelpers.IsNullOrDestroyed(__instance))
            {
                return;
            }

            try
            {
                var args = DialogueEventArgs.Create(__instance, DialoguePatchType.Postfix);
                _instance?.OnPostfixDialogueStart?.Invoke(_instance, args);
            }
            catch (Exception ex)
            {
                KappiLogger.LogException("Failed to process postfix dialogue start", exception: ex);
            }
        }
    }
}
