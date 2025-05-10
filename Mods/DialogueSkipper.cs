using System.Text;
using KappiMod.Config;
using KappiMod.Events;
using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Mods.Extensions;
using KappiMod.Patches;
using KappiMod.Properties;

namespace KappiMod.Mods;

using DialogueMapping = Dictionary<string, int>;
using DialogueSceneMappings = Dictionary<string, Dictionary<string, int>>;

[ModInfo(
    name: "Dialogue Skipper",
    description: "Skip certain dialogue sections in the game",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public sealed class DialogueSkipper : BaseMod
{
    private static readonly DialogueSceneMappings _ignoredDialogues = new()
    {
        {
            "Scene 7 - Backrooms",
            new DialogueMapping { { "KindMita 1 [Продолжает]", 203 }, { "KindMita 2", 204 } }
        },
        {
            "Scene 17 - Dreamer",
            new DialogueMapping { { "Mita 3", 74 }, { "Mita 4", 75 } }
        },
        {
            "Scene 14 - MobilePlayer",
            new DialogueMapping { { "Mita 4", 118 } }
        },
        {
            "Scene 15 - BasementAndDeath",
            new DialogueMapping { { "Player 3", 70 } }
        },
    };

    public override bool IsEnabled
    {
        get => base.IsEnabled && ConfigManager.DialogueSkipper.Value;
        protected set
        {
            base.IsEnabled = value;
            ConfigManager.DialogueSkipper.Value = value;
        }
    }

    protected override void OnInitialize()
    {
        if (!DialoguePatcher.IsInitialized)
        {
            KappiLogger.LogError(
                $"{nameof(DialoguePatcher)} is not initialized. Mod can't be used."
            );
            return;
        }

        if (ConfigManager.DialogueSkipper.Value)
        {
            OnEnable();
            base.IsEnabled = true;
        }
    }

    protected override void OnEnable()
    {
        SubscribeEvents();
    }

    protected override void OnDisable()
    {
        UnsubscribeEvents();
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
            LogDialogueInfo(args, separator: '=', isWarning: true);
            return;
        }

        args.DialogueInstance.SkipDialogue();
        LogDialogueInfo(args);
    }

    private static bool IsDialogueIgnored(DialogueEventArgs args) =>
        _ignoredDialogues.ContainsKey(args.SceneName)
        && _ignoredDialogues[args.SceneName].ContainsKey(args.ObjectName)
        && _ignoredDialogues[args.SceneName][args.ObjectName] == args.IndexString;

    private static void LogDialogueInfo(
        DialogueEventArgs args,
        char separator = '-',
        bool isWarning = false
    )
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

        if (isWarning)
        {
            KappiLogger.LogWarning(sb.ToString());
        }
        else
        {
            KappiLogger.Log(sb.ToString());
        }
    }
}
