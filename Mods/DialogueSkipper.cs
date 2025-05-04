using System.Text;
using KappiMod.Config;
using KappiMod.Events;
using KappiMod.Mods.Extensions;

namespace KappiMod.Mods;

using DialogueMapping = Dictionary<string, int>;
using DialogueSceneMappings = Dictionary<string, Dictionary<string, int>>;

public static class DialogueSkipper
{
    private static readonly DialogueSceneMappings _ignoredDialogues = new()
    {
        {
            "Scene 7 - Backrooms",
            new DialogueMapping { { "KindMita 1 [Продолжает]", 203 }, { "KindMita 2", 204 } }
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
            new DialogueMapping { { "Player 6", 121 }, { "Mita 1 [Шепотом]", 123 } }
        },
        {
            "Scene 15 - BasementAndDeath",
            new DialogueMapping
            {
                { "Player 1", 68 },
                { "Player 2", 69 },
                { "Player 3", 70 },
            }
        },
    };

    public static bool Enabled
    {
        get => ConfigManager.DialogueSkipper.Value;
        set
        {
            if (value)
            {
                SubscribeEvents();
            }
            else
            {
                UnsubscribeEvents();
            }

            KappiModCore.Log(value ? "Enabled" : "Disabled");

            ConfigManager.DialogueSkipper.Value = value;
        }
    }

    public static void Init()
    {
        if (Enabled)
        {
            SubscribeEvents();
        }

        KappiModCore.Log("Initialized");
    }

    private static void SubscribeEvents()
    {
        DialogueEventSystem.OnPrefixDialogueStart += HandleDialogueSkip;
        DialogueEventSystem.OnPostfixDialogueStart += HandleDialogueSkip;

        ChibiMitaDialogueFixer.Init();
    }

    private static void UnsubscribeEvents()
    {
        DialogueEventSystem.OnPrefixDialogueStart -= HandleDialogueSkip;
        DialogueEventSystem.OnPostfixDialogueStart -= HandleDialogueSkip;

        ChibiMitaDialogueFixer.CleanUp();
    }

    private static void HandleDialogueSkip(object? sender, DialogueEventArgs args)
    {
        if (IsDialogueIgnored(args))
        {
            LogDialogueInfo(args, separator: '=');
            return;
        }

        args.DialogueInstance.SkipDialogue();

        LogDialogueInfo(args);
    }

    private static bool IsDialogueIgnored(DialogueEventArgs args) =>
        _ignoredDialogues.ContainsKey(args.SceneName)
        && _ignoredDialogues[args.SceneName].ContainsKey(args.ObjectName)
        && _ignoredDialogues[args.SceneName][args.ObjectName] == args.IndexString;

    private static void LogDialogueInfo(DialogueEventArgs args, char separator = '-')
    {
        if (string.IsNullOrWhiteSpace(args.Text))
        {
            return;
        }

        StringBuilder sb = new();
        sb.Append('\n');
        sb.AppendLine(new string(separator, 50));
        sb.AppendLine($"Dialogue name: {args.ObjectName}");
        sb.AppendLine($"Scene name: {args.SceneName}");
        sb.AppendLine($"Index string: {args.IndexString}");
        sb.AppendLine($"Text: {args.Text}");
        sb.AppendLine($"Speaker: {args.Speaker?.name ?? "null"}");
        sb.AppendLine(new string(separator, 50));
        KappiModCore.Log(sb.ToString());
    }
}
