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

    private static void Raise3DDialogue(Dialogue_3DText dialogue, DialoguePatchType patchType)
    {
        if (dialogue.IsNullOrDestroyed())
        {
            return;
        }

        try
        {
            var args = Dialogue3DEventArgs.Create(dialogue, patchType);
            switch (patchType)
            {
                case DialoguePatchType.Prefix:
                    _instance?.OnPrefixDialogueStart?.Invoke(_instance, args);
                    break;
                case DialoguePatchType.Postfix:
                    _instance?.OnPostfixDialogueStart?.Invoke(_instance, args);
                    break;
            }
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to process 3D dialogue event", exception: ex);
        }
    }

    private static void RaiseNovellaDialogue(Location18_Novella novella)
    {
        if (novella.IsNullOrDestroyed())
        {
            return;
        }

        try
        {
            var args = Dialogue2DEventArgs.Create(novella, DialoguePatchType.Postfix);
            _instance?.OnPostfixDialogueStart?.Invoke(_instance, args);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to process novella dialogue event", exception: ex);
        }
    }

    private static void RaiseTicTacToeDialogue(Location18_TicTacToe ticTacToe)
    {
        if (ticTacToe.IsNullOrDestroyed())
        {
            return;
        }

        try
        {
            var args = DialogueTicTacToeEventArgs.Create(ticTacToe, DialoguePatchType.Postfix);
            _instance?.OnPostfixDialogueStart?.Invoke(_instance, args);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to process tic-tac-toe dialogue event", exception: ex);
        }
    }

    private static void RaiseTamagotchiDialogue(Tamagotchi_Dialogue dialogue)
    {
        if (dialogue.IsNullOrDestroyed())
        {
            return;
        }

        try
        {
            var args = DialogueTamagotchiEventArgs.Create(dialogue, DialoguePatchType.Postfix);
            _instance?.OnPostfixDialogueStart?.Invoke(_instance, args);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to process Tamagotchi dialogue event", exception: ex);
        }
    }

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Dialogue_3DText), nameof(Dialogue_3DText.Start))]
        private static void OnDialogueStartPrefix(Dialogue_3DText __instance) =>
            Raise3DDialogue(__instance, DialoguePatchType.Prefix);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Dialogue_3DText), nameof(Dialogue_3DText.Start))]
        private static void OnDialogueStartPostfix(Dialogue_3DText __instance) =>
            Raise3DDialogue(__instance, DialoguePatchType.Postfix);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Location18_Novella), nameof(Location18_Novella.Update))]
        private static void OnNovellaUpdatePostfix(Location18_Novella __instance)
        {
            if (
                __instance.IsNullOrDestroyed()
                || !__instance.controllDialogue
                || !(
                    __instance.playPrint || __instance is { dialogueShow: true, timeWasObject: 0f }
                )
            )
            {
                return;
            }

            RaiseNovellaDialogue(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Location18_TicTacToe), nameof(Location18_Novella.Update))]
        private static void OnTicTacToeUpdatePostfix(Location18_TicTacToe __instance)
        {
            if (
                __instance.IsNullOrDestroyed()
                || !(__instance.timeDialogueNext > 0f || __instance.waitClick)
            )
            {
                return;
            }

            RaiseTicTacToeDialogue(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Tamagotchi_Dialogue), nameof(Tamagotchi_Dialogue.Update))]
        private static void OnTamagotchiUpdatePostfix(Tamagotchi_Dialogue __instance)
        {
            if (__instance.IsNullOrDestroyed() || !__instance.enableDialogue || __instance.play)
            {
                return;
            }

            RaiseTamagotchiDialogue(__instance);
        }
    }
}
