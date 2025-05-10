using KappiMod.Logging;
using KappiMod.Mods;
using KappiMod.Mods.Core;
using KappiMod.Patches;
using KappiMod.Properties;
using KappiMod.Utils;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace KappiMod.UI;

public class MainPanel : PanelBase
{
    private GameObject? _updateButton;

    private GameObject? _togglesColumnsLayout;
    private GameObject? _togglesLeftColumn;
    private GameObject? _togglesRightColumn;

    private GameObject? _modsSettingsColumnsLayout;
    private GameObject? _modsSettingsLeftColumn;
    private GameObject? _fpsLimitRow;

    public MainPanel(UIBase owner)
        : base(owner) { }

    public override string Name => $"{BuildInfo.NAME} v{BuildInfo.VERSION}";
    public override int MinWidth => 420;
    public override int MinHeight => 400;
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
        CreateDialogueSkipperToggle(_togglesLeftColumn);
        CreateFlashlightIncreaserToggle(_togglesLeftColumn);
        CreateSitUnlockerToggle(_togglesLeftColumn);
        CreateSprintUnlockerToggle(_togglesLeftColumn);
        CreateTimeScaleScrollerToggle(_togglesLeftColumn);

        CreateLabel(_togglesRightColumn, "TogglePatchesLabel", "Toggle Patches");
        CreateIntroSkipperToggle(_togglesRightColumn);

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

    #region TOGGLE_MODS

    private static void CreateDialogueSkipperToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<DialogueSkipper>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(DialogueSkipper)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out Toggle toggle, out Text text);
        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;
        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value)
                {
                    mod.Enable();
                }
                else
                {
                    mod.Disable();
                }

                if (mod.IsEnabled != value)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private static void CreateFlashlightIncreaserToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<FlashlightIncreaser>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(FlashlightIncreaser)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out Toggle toggle, out Text text);
        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;
        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value)
                {
                    mod.Enable();
                }
                else
                {
                    mod.Disable();
                }

                if (mod.IsEnabled != value)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private static void CreateSitUnlockerToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<SitUnlocker>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(SitUnlocker)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out Toggle toggle, out Text text);
        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;
        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value)
                {
                    mod.Enable();
                }
                else
                {
                    mod.Disable();
                }

                if (mod.IsEnabled != value)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private static void CreateSprintUnlockerToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<SprintUnlocker>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(SprintUnlocker)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out Toggle toggle, out Text text);
        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;
        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value)
                {
                    mod.Enable();
                }
                else
                {
                    mod.Disable();
                }

                if (mod.IsEnabled != value)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    private static void CreateTimeScaleScrollerToggle(GameObject parent)
    {
        var mod = ModManager.GetMod<TimeScaleScroller>();
        if (mod is null)
        {
            KappiLogger.LogError($"{nameof(TimeScaleScroller)} mod not found!");
            return;
        }

        UIFactory.CreateToggle(parent, $"{mod.Id}Toggle", out Toggle toggle, out Text text);
        text.text = mod.Name;
        toggle.isOn = mod.IsEnabled;
        toggle.onValueChanged.AddListener(
            (value) =>
            {
                if (value)
                {
                    mod.Enable();
                }
                else
                {
                    mod.Disable();
                }

                if (mod.IsEnabled != value)
                {
                    toggle.isOn = mod.IsEnabled;
                }
            }
        );
    }

    #endregion TOGGLE_MODS

    #region TOGGLE_PATCHES

    private static void CreateIntroSkipperToggle(GameObject parent)
    {
        UIFactory.CreateToggle(parent, "IntroSkipperToggle", out Toggle toggle, out Text text);
        text.text = "Skip menu intro";
        toggle.isOn = IntroSkipper.Enabled;
        toggle.onValueChanged.AddListener(
            (value) =>
            {
                IntroSkipper.Enabled = value;
                if (IntroSkipper.Enabled != value)
                {
                    toggle.isOn = IntroSkipper.Enabled;
                }
            }
        );
    }

    #endregion TOGGLE_PATCHES

    #region MODS_SETTINGS

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
            new Vector4(2, 2, 2, 2)
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

    #endregion MODS_SETTINGS

    #region CHECK_FOR_UPDATES

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
            return;

        GameObject buttonContainer = UIFactory.CreateHorizontalGroup(
            UIRoot,
            "UpdateButtonContainer",
            false,
            true,
            true,
            true,
            2,
            new Vector4(2, 2, 2, 2)
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

    #endregion CHECK_FOR_UPDATES

    #region CREATING_UI_HELPERS

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
            new Vector4(2, 2, 2, 2)
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
            new Vector4(2, 2, 2, 2)
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

    #endregion CREATING_UI_HELPERS
}
