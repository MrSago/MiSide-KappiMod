using System.Text;
using KappiMod.Config;
using KappiMod.Events;
using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Mods.Extensions;
using KappiMod.Patches;
using KappiMod.Properties;
using KappiMod.UI.Internal.EventDisplay;

namespace KappiMod.Mods;

using DialogueSceneMappings = Dictionary<string, Dictionary<string, int>>;

[ModInfo(
    name: "Dialogue Skipper",
    description: "Skip certain dialogue sections in the game",
    version: "1.2.0",
    author: BuildInfo.COMPANY
)]
public sealed class DialogueSkipper : BaseMod
{
    private readonly DialogueSceneMappings _ignoredDialogues = new()
    {
        {
            "Scene 7 - Backrooms",
            new() { { "KindMita 1 [Продолжает]", 203 }, { "KindMita 2", 204 } }
        },
        {
            "Scene 17 - Dreamer",
            new() { { "Mita 3", 74 }, { "Mita 4", 75 } }
        },
        {
            "Scene 14 - MobilePlayer",
            new() { { "Mita 4", 118 } }
        },
        {
            "Scene 15 - BasementAndDeath",
            new()
            {
                { "Player 1", 68 },
                { "Player 2", 69 },
                { "Player 3", 70 },
            }
        },
    };

    private readonly DialogueStartPatch _dialoguePatch;
    private readonly ChibiMitaDialogueFixer _chibiMitaDialogueFixer;

    public DialogueSkipper()
    {
        _dialoguePatch = new();
        _chibiMitaDialogueFixer = new(_dialoguePatch);
    }

    public override bool IsEnabled
    {
        get => base.IsEnabled && ConfigManager.DialogueSkipper.Value;
        protected set
        {
            base.IsEnabled = value;
            ConfigManager.DialogueSkipper.Value = value;
        }
    }

    protected override void OnInitialize() { }

    protected override void OnEnable()
    {
        KappiCore.Loader.SceneWasLoaded += OnSceneWasLoaded;
        SubscribeEvents();
        _chibiMitaDialogueFixer.Init();
    }

    protected override void OnDisable()
    {
        KappiCore.Loader.SceneWasLoaded -= OnSceneWasLoaded;
        UnsubscribeEvents();
        _chibiMitaDialogueFixer.CleanUp();
    }

    private static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        try
        {
            EventManager.ShowEvent(new($"{nameof(DialogueSkipper)} still enabled"));
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to show event", exception: ex);
            return;
        }
    }

    private void SubscribeEvents()
    {
        _dialoguePatch.OnPostfixDialogueStart += HandleDialogueSkip;
    }

    private void UnsubscribeEvents()
    {
        _dialoguePatch.OnPostfixDialogueStart -= HandleDialogueSkip;
    }

    private void HandleDialogueSkip(object? sender, DialogueEventArgs args)
    {
        if (IsDialogueIgnored(args))
        {
            LogDialogueInfo(args, separator: '=', isWarning: true);
            return;
        }

        args.DialogueInstance.SkipDialogue();
        LogDialogueInfo(args);
    }

    private bool IsDialogueIgnored(DialogueEventArgs args) =>
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

        StringBuilder sb = new("\n");
        sb.AppendLine(new string(separator, 50));
        sb.AppendLine($"Dialogue patch type: {args.PatchType}");
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
