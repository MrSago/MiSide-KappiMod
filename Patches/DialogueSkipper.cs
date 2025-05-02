using System.Text;
using HarmonyLib;
using KappiMod.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches;

using DialogueMapping = Dictionary<string, int>;
using DialogueSceneMappings = Dictionary<string, Dictionary<string, int>>;

public static class DialogueSkipper
{
    public static bool Enabled
    {
        get => ConfigManager.DialogueSkipper.Value;
        set
        {
            KappiModCore.Log(value ? "Enabled" : "Disabled");

            ConfigManager.DialogueSkipper.Value = value;
        }
    }

    private static HarmonyLib.Harmony _harmony = null!;

    public static void Init()
    {
        _harmony = new("com.miside.dialogueskipper");
        _harmony.PatchAll(typeof(Patch));

        KappiModCore.Log("Initialized");
    }

    [HarmonyPatch]
    private static class Patch
    {
        private static readonly DialogueSceneMappings _ignoredDialogues = new()
        {
            {
                "Scene 7 - Backrooms",
                new DialogueMapping
                {
                    { "KindMita 1 [Продолжает]", 203 },
                    { "KindMita 2", 204 }
                }
            },
            {
                "Scene 9 - ChibiMita",
                new DialogueMapping
                {
                    { "3D TextFactory 4 H", 35 }
                }
            },
            {
                "Scene 17 - Dreamer",
                new DialogueMapping
                {
                    { "Mita 3", 74 },
                    { "Mita 4", 75 },
                    { "Mita 5", 76 },
                    { "Player 7", 100 },
                    { "Player 8", 101 },
                    { "Mita 1(Clone)", 106 },
                    { "Mita 2(Clone)", 105 },
                    { "Mita 3(Clone)", 104 },
                }
            },
            {
                "Scene 14 - MobilePlayer",
                new DialogueMapping
                {
                    { "Player 6", 121 },
                    { "Mita 1 [Шепотом]", 123 }
                }
            },
            {
                "Scene 15 - BasementAndDeath",
                new DialogueMapping
                {
                    { "Player 1", 68 },
                    { "Player 2", 69 }
                }
            },
        };

        [HarmonyPatch(typeof(Dialogue_3DText), "Start")]
        private static void Prefix(Dialogue_3DText __instance)
        {
            HandleSkipDialogue(__instance);
        }

        [HarmonyPatch(typeof(Dialogue_3DText), "Start")]
        private static void Postfix(Dialogue_3DText __instance)
        {
            HandleSkipDialogue(__instance);
        }

        private static void HandleSkipDialogue(Dialogue_3DText __instance)
        {
            if (!Enabled)
            {
                return;
            }

            try
            {
                SkipDialouge(__instance);
            }
            catch (Exception e)
            {
                KappiModCore.LogError(e.Message);
            }
        }

        private static void SkipDialouge(Dialogue_3DText __instance)
        {
            string objectName = __instance.name;
            string activeSceneName = SceneManager.GetActiveScene().name;
            int indexString = __instance.indexString;
            string text = __instance.textPrint;
            GameObject? speaker = __instance.speaker;

            if (
                _ignoredDialogues.ContainsKey(activeSceneName)
                && _ignoredDialogues[activeSceneName].ContainsKey(objectName)
                && _ignoredDialogues[activeSceneName][objectName] == indexString
            )
            {
                LogDialogueInfo(
                    objectName,
                    activeSceneName,
                    indexString,
                    text,
                    speaker,
                    separator: '='
                );
                return;
            }

            __instance.SkipDialogue();

            LogDialogueInfo(objectName, activeSceneName, indexString, text, speaker);
        }

        private static void LogDialogueInfo(
            string objectName,
            string activeSceneName,
            int indexString,
            string text,
            GameObject? speaker,
            char separator = '-'
        )
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            StringBuilder sb = new();
            sb.Append('\n');
            sb.AppendLine(new string(separator, 50));
            sb.AppendLine($"Dialogue: {objectName}");
            sb.AppendLine($"Scene name: {activeSceneName}");
            sb.AppendLine($"Index string: {indexString}");
            sb.AppendLine($"Text: {text}");
            sb.AppendLine($"Speaker: {speaker?.name ?? "null"}");
            sb.AppendLine(new string(separator, 50));
            KappiModCore.Log(sb.ToString());
        }
    }
}
