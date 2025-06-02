using KappiMod.Config;
using KappiMod.Constants;
using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Mods.Core;
using KappiMod.Patches;
using KappiMod.Properties;
using KappiMod.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace KappiMod.UI.IMGUI;

public class MainPanel : PanelBase
{
    private const ulong AUTHOR_STEAM_ID = 76561198034791437;

    private readonly List<Toggle> _modToggles = new();
    private readonly List<Toggle> _speedrunModToggles = new();

    private GameObject? _togglesColumnsLayout;
    private GameObject? _togglesLeftColumn;
    private GameObject? _togglesRightColumn;

    private GameObject? _speedrunModsColumnsLayout;
    private GameObject? _speedrunModsLeftColumn;

    private GameObject? _modsSettingsColumnsLayout;
    private GameObject? _modsSettingsLeftColumn;
    private GameObject? _fpsLimitRow;

    private GameObject? _updateButton;

    public MainPanel(UIBase owner)
        : base(owner) { }

    public override string Name => $"{BuildInfo.NAME} v{BuildInfo.VERSION}";
    public override int MinWidth => 420;
    public override int MinHeight => 550;
    public override Vector2 DefaultAnchorMin => new(0.25f, 0.25f);
    public override Vector2 DefaultAnchorMax => new(0.25f, 0.25f);
    public override bool CanDragAndResize => true;

    protected Text StatusBar { get; private set; } = null!;

    protected override void ConstructPanelContent()
    {
        _togglesColumnsLayout = CreateColumnsLayout(
            ContentRoot,
            "TogglesColumnsLayout",
            minHeight: 150
        );

        _togglesLeftColumn = CreateVerticalGroup(_togglesColumnsLayout, "TogglesLeftColumn");
        _togglesRightColumn = CreateVerticalGroup(_togglesColumnsLayout, "TogglesRightColumn");

        CreateLabel(_togglesLeftColumn, "ToggleModsLabel", "Toggle Mods");
        CreateFlashlightIncreaserToggle(_togglesLeftColumn);
        CreateSitUnlockerToggle(_togglesLeftColumn);
        CreateSprintUnlockerToggle(_togglesLeftColumn);
        CreateTimeScaleScrollerToggle(_togglesLeftColumn);

        CreateLabel(_togglesRightColumn, "TogglePatchesLabel", "Toggle Patches");
        CreateIntroSkipperToggle(_togglesRightColumn);

        _speedrunModsColumnsLayout = CreateColumnsLayout(
            ContentRoot,
            "SpeedrunModsColumnsLayout",
            minHeight: 150
        );

        _speedrunModsLeftColumn = CreateVerticalGroup(
            _speedrunModsColumnsLayout,
            "SpeedrunModsLeftColumn"
        );

        CreateLabel(_speedrunModsLeftColumn, "SpeedrunModsLabel", "Speedrun Mods");
        CreateBlessRngModToggle(_speedrunModsLeftColumn);
        CreateDialogueSkipperToggle(_speedrunModsLeftColumn);
        CreateCustomRngToggle(_speedrunModsLeftColumn);

        _modsSettingsColumnsLayout = CreateColumnsLayout(
            ContentRoot,
            "ModsSettingsColumnsLayout",
            minHeight: 150
        );

        _modsSettingsLeftColumn = CreateVerticalGroup(
            _modsSettingsColumnsLayout,
            "ModsSettingsLeftColumn"
        );

        CreateLabel(_modsSettingsLeftColumn, "ModsSettingsLabel", "Mods Settings");
        CreateFpsLimitField(_modsSettingsLeftColumn);

        CreateStatusBar();

        CheckForUpdates();

        OnClosePanelClicked();
    }

    protected override void OnClosePanelClicked()
    {
        Owner.Enabled = !Owner.Enabled;
    }

    protected void UpdateStatusBar(string text)
    {
        StatusBar.text = text;
    }

    #region Toggle Mods

    private void CreateFlashlightIncreaserToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<FlashlightIncreaser>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(FlashlightIncreaser)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out var toggle, out var text);
        _modToggles.Add(toggle);

        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;

        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value == mod.IsEnabled)
                {
                    return;
                }

                if (value && !mod.IsEnabled)
                {
                    mod.Enable();
                }
                else if (!value && mod.IsEnabled)
                {
                    mod.Disable();
                }

                if (value != mod.IsEnabled)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private void CreateSitUnlockerToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<SitUnlocker>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(SitUnlocker)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out var toggle, out var text);
        _modToggles.Add(toggle);

        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;

        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value == mod.IsEnabled)
                {
                    return;
                }

                if (value && !mod.IsEnabled)
                {
                    mod.Enable();
                }
                else if (!value && mod.IsEnabled)
                {
                    mod.Disable();
                }

                if (value != mod.IsEnabled)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private void CreateSprintUnlockerToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<SprintUnlocker>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(SprintUnlocker)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out var toggle, out var text);
        _modToggles.Add(toggle);

        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;

        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value == mod.IsEnabled)
                {
                    return;
                }

                if (value && !mod.IsEnabled)
                {
                    mod.Enable();
                }
                else if (!value && mod.IsEnabled)
                {
                    mod.Disable();
                }

                if (value != mod.IsEnabled)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private void CreateTimeScaleScrollerToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<TimeScaleScroller>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(TimeScaleScroller)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out var toggle, out var text);
        _modToggles.Add(toggle);

        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;

        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value == mod.IsEnabled)
                {
                    return;
                }

                if (value && !mod.IsEnabled)
                {
                    mod.Enable();
                }
                else if (!value && mod.IsEnabled)
                {
                    mod.Disable();
                }

                if (value != mod.IsEnabled)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    #endregion Toggle Mods

    #region Toggle Patches

    private static void CreateIntroSkipperToggle(GameObject parent)
    {
        UIFactory.CreateToggle(
            parent,
            $"{nameof(IntroSkipPatch)}Toggle",
            out var toggle,
            out var text
        );

        text.text = "Skip menu intro";
        toggle.isOn = IntroSkipPatch.Enabled;

        toggle.onValueChanged.AddListener(
            (value) =>
            {
                IntroSkipPatch.Enabled = value;
                if (IntroSkipPatch.Enabled != value)
                {
                    toggle.isOn = IntroSkipPatch.Enabled;
                }
            }
        );
    }

    #endregion Toggle Patches

    #region Speedrun Mods

    private void CreateBlessRngModToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<BlessRng>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(BlessRng)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out var toggle, out var text);
        _speedrunModToggles.Add(toggle);

        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;

        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value == mod.IsEnabled)
                {
                    return;
                }

                if (
                    SceneManager.GetActiveScene().name is not SceneName.MAIN_MENU
                    && SteamHelper.Instance?.GetSteamID() != AUTHOR_STEAM_ID
                )
                {
                    MessageBox.Show("This mod toggled only in the main menu");
                    toggle.isOn = !value;
                    return;
                }

                if (value && !mod.IsEnabled)
                {
                    toggle.isOn = false;
                    toggle.interactable = false;

                    MessageBox.ShowYesNo(
                        "BlessRng mod is designed primarily for speedrunners, "
                            + "so you will sometimes see pop-up messages during the game. "
                            + "This is done to prevent cheating in clean runs. Do you want to enable it?",
                        () =>
                        {
                            mod.Enable();
                            DisableAllModToggles();
                            toggle.interactable = true;
                            toggle.isOn = true;
                        },
                        () =>
                        {
                            toggle.interactable = true;
                        }
                    );
                }
                else if (!value && mod.IsEnabled)
                {
                    mod.Disable();
                    EnableAllModToggles();
                }

                if (value != mod.IsEnabled)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private void CreateDialogueSkipperToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<DialogueSkipper>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(DialogueSkipper)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out var toggle, out var text);
        _speedrunModToggles.Add(toggle);

        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;

        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value == mod.IsEnabled)
                {
                    return;
                }

                if (
                    SceneManager.GetActiveScene().name is not SceneName.MAIN_MENU
                    && SteamHelper.Instance?.GetSteamID() != AUTHOR_STEAM_ID
                )
                {
                    MessageBox.Show("This mod toggled only in the main menu");
                    toggle.isOn = !value;
                    return;
                }

                if (value && !mod.IsEnabled)
                {
                    mod.Enable();
                    DisableAllModToggles();
                }
                else if (!value && mod.IsEnabled)
                {
                    mod.Disable();
                    EnableAllModToggles();
                }

                if (value != mod.IsEnabled)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private void CreateCustomRngToggle(GameObject parent)
    {
        if (!ConfigManager.DebugMode.Value)
        {
            return;
        }

        var mod = ModManager.GetMod<CustomRng>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(CustomRng)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out var toggle, out var text);
        _speedrunModToggles.Add(toggle);

        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;

        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value == mod.IsEnabled)
                {
                    return;
                }

                if (value && !mod.IsEnabled)
                {
                    mod.Enable();
                    DisableAllModToggles();
                }
                else if (!value && mod.IsEnabled)
                {
                    mod.Disable();
                    EnableAllModToggles();
                }

                if (value != mod.IsEnabled)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private void EnableAllModToggles()
    {
        if (_speedrunModToggles.Any(toggle => toggle.isOn))
        {
            return;
        }

        _modToggles.ForEach(
            (toggle) =>
            {
                toggle.interactable = true;
            }
        );
    }

    private void DisableAllModToggles()
    {
        if (SteamHelper.Instance?.GetSteamID() == AUTHOR_STEAM_ID)
        {
            return;
        }

        _modToggles.ForEach(
            (toggle) =>
            {
                toggle.isOn = false;
                toggle.interactable = false;
            }
        );
    }

    #endregion Speedrun Mods

    #region Mods Settings

    private void CreateFpsLimitField(GameObject parent)
    {
        var mod = ModManager.GetMod<FpsLimit>();
        if (mod is null)
        {
            KappiLogger.LogError("FpsLimit mod not found!");
            return;
        }

        _fpsLimitRow = UIFactory.CreateHorizontalGroup(
            parent,
            "FpsLimitRow",
            false,
            true,
            true,
            true,
            2,
            new(2, 2, 2, 2)
        );
        UIFactory.SetLayoutElement(
            _fpsLimitRow,
            minHeight: 25,
            minWidth: 200,
            flexibleHeight: 0,
            flexibleWidth: 0
        );

        Text fpsLimitLabel = UIFactory.CreateLabel(
            _fpsLimitRow,
            "FpsLimitLabel",
            "Fps limit (-1 unlimited):",
            TextAnchor.MiddleLeft
        );
        UIFactory.SetLayoutElement(fpsLimitLabel.gameObject, minWidth: 110, flexibleWidth: 50);

        InputFieldRef fpsLimitField = UIFactory.CreateInputField(
            _fpsLimitRow,
            "FpsLimitField",
            "fps"
        );
        UIFactory.SetLayoutElement(
            fpsLimitField.UIRoot,
            minHeight: 25,
            minWidth: 50,
            flexibleHeight: 0,
            flexibleWidth: 0
        );

        fpsLimitField.Text = FpsLimit.CurrentFpsLimit.ToString();

        fpsLimitField.OnValueChanged += (value) =>
        {
            if (value.Length > 4)
            {
                value = value[..4];
                fpsLimitField.Text = value;
            }

            if (int.TryParse(value, out int fpsLimit))
            {
                FpsLimit.SetFpsLimit(fpsLimit);
            }
        };
    }

    #endregion Mods Settings

    #region Checking for updates

    private void CheckForUpdates()
    {
        UpdateStatusBar("Checking for updates...");

        VersionChecker.CheckForUpdatesAsync();
        KappiCore.Loader.Update += OnVersionCheckUpdate;
    }

    private void OnVersionCheckUpdate()
    {
        if (VersionChecker.IsCheckingVersion)
        {
            return;
        }

        KappiCore.Loader.Update -= OnVersionCheckUpdate;

        if (VersionChecker.UpdateAvailable)
        {
            UpdateStatusBar(
                $"Update available! v{VersionChecker.CurrentVersion} -> v{VersionChecker.LatestVersion}"
            );
            CreateUpdateButton();
        }
        else
        {
            UpdateStatusBar("Ready! You are using the latest version.");
        }
    }

    private void CreateUpdateButton()
    {
        if (_updateButton != null)
        {
            return;
        }

        GameObject buttonContainer = UIFactory.CreateHorizontalGroup(
            UIRoot,
            "UpdateButtonContainer",
            false,
            true,
            true,
            true,
            2,
            new(2, 2, 2, 2)
        );

        UIFactory.SetLayoutElement(
            buttonContainer,
            minHeight: 30,
            minWidth: 200,
            flexibleHeight: 0,
            flexibleWidth: 0
        );

        ButtonRef updateButton = UIFactory.CreateButton(
            buttonContainer,
            "UpdateButton",
            "Download Update"
        );

        _updateButton = updateButton.Component.gameObject;
        updateButton.OnClick += OpenDownloadPage;

        UIFactory.SetLayoutElement(
            _updateButton,
            minHeight: 25,
            minWidth: 150,
            flexibleHeight: 0,
            flexibleWidth: 0
        );
    }

    private void OpenDownloadPage()
    {
        Application.OpenURL(VersionChecker.DownloadUrl);
    }

    #endregion Checking for updates

    #region UI Factory

    private static GameObject CreateColumnsLayout(
        GameObject parent,
        string name,
        int? minHeight = null,
        int flexibleHeight = 1,
        int flexibleWidth = 9999
    )
    {
        GameObject layout = UIFactory.CreateHorizontalGroup(
            parent,
            name,
            false,
            true,
            true,
            true,
            0,
            new(2, 2, 2, 2)
        );

        UIFactory.SetLayoutElement(
            layout,
            minHeight: minHeight,
            flexibleHeight: flexibleHeight,
            flexibleWidth: flexibleWidth
        );

        return layout;
    }

    private static GameObject CreateVerticalGroup(
        GameObject parent,
        string name,
        int minWidth = 200,
        int flexibleHeight = 0,
        int flexibleWidth = 0
    )
    {
        GameObject group = UIFactory.CreateVerticalGroup(
            parent,
            name,
            false,
            false,
            true,
            true,
            3,
            new(2, 2, 2, 2)
        );

        UIFactory.SetLayoutElement(
            group,
            minWidth: minWidth,
            flexibleHeight: flexibleHeight,
            flexibleWidth: flexibleWidth
        );

        return group;
    }

    private static void CreateLabel(
        GameObject parent,
        string name,
        string text,
        TextAnchor anchor = TextAnchor.MiddleLeft,
        int fontSize = 18
    )
    {
        UIFactory.CreateLabel(parent, name, text, anchor, fontSize: fontSize);
    }

    private void CreateStatusBar()
    {
        StatusBar = UIFactory.CreateLabel(UIRoot, "StatusBar", "Ready!", TextAnchor.MiddleLeft);
        StatusBar.horizontalOverflow = HorizontalWrapMode.Wrap;
        UIFactory.SetLayoutElement(
            StatusBar.gameObject,
            minHeight: 25,
            flexibleWidth: 9999,
            flexibleHeight: 200
        );
    }

    #endregion UI Factory
}
