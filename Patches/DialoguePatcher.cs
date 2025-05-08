using HarmonyLib;
using KappiMod.Events;
using KappiMod.Utils;
using UnityEngine.SceneManagement;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches;

public static class DialoguePatcher
{
    private static bool _isInitialized = false;
    public static bool IsInitialized => _isInitialized;

    private static HarmonyLib.Harmony _harmony = null!;

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiModCore.LogError($"{nameof(DialoguePatcher)} is already initialized");
            return;
        }

        _isInitialized = true;

        _harmony = new("com.miside.dialoguepatcher");
        _harmony.PatchAll(typeof(Patch));

        KappiModCore.Log("Initialized");
    }

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPatch(typeof(Dialogue_3DText), "Start")]
        private static void Prefix(Dialogue_3DText __instance)
        {
            if (!UnityHelpers.IsValid(__instance))
            {
                return;
            }

            try
            {
                DialogueEventArgs args = CreateDialogueEventArgs(__instance);
                DialogueEventSystem.InvokePrefixDialogueStart(args);
            }
            catch (Exception e)
            {
                KappiModCore.LogError(e.Message);
            }
        }

        [HarmonyPatch(typeof(Dialogue_3DText), "Start")]
        private static void Postfix(Dialogue_3DText __instance)
        {
            if (!UnityHelpers.IsValid(__instance))
            {
                return;
            }

            try
            {
                DialogueEventArgs args = CreateDialogueEventArgs(__instance);
                DialogueEventSystem.InvokePostfixDialogueStart(args);
            }
            catch (Exception e)
            {
                KappiModCore.LogError(e.Message);
            }
        }

        private static DialogueEventArgs CreateDialogueEventArgs(Dialogue_3DText instance) =>
            new(
                instance,
                instance.name,
                SceneManager.GetActiveScene().name,
                instance.indexString,
                instance.textPrint,
                instance.speaker
            );
    }
}
