using KappiMod.Properties;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace KappiMod.UI.IMGUI;

/// <summary>
/// A simple message box dialog implementation using UniverseLib.
/// Allows displaying messages with optional title, buttons and callbacks.
/// </summary>
public class MessageBox : PanelBase
{
    #region Static Members

    private static UIBase? _uiBase;
    private static MessageBox? _activeMessageBox;

    /// <summary>
    /// Initializes the MessageBox system. Must be called before using any MessageBox functionality.
    /// </summary>
    public static void Init()
    {
        if (_uiBase is not null)
        {
            return;
        }

        _uiBase = UniversalUI.RegisterUI($"{BuildInfo.GUID}_MessageBox", null);
        _uiBase.Enabled = false;
    }

    /// <summary>
    /// Shows a simple message box with an OK button
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="callback">Optional callback when OK is clicked</param>
    public static void Show(string message, Action? callback = null)
    {
        ShowMessageBox(message, "OK", callback);
    }

    /// <summary>
    /// Shows a message box with OK and Cancel buttons
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="onOk">Callback when OK is clicked</param>
    /// <param name="onCancel">Optional callback when Cancel is clicked</param>
    public static void ShowConfirm(string message, Action? onOk = null, Action? onCancel = null)
    {
        ShowMessageBox(message, "OK", onOk, "Cancel", onCancel);
    }

    /// <summary>
    /// Shows a message box with Yes and No buttons
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="onYes">Callback when Yes is clicked</param>
    /// <param name="onNo">Optional callback when No is clicked</param>
    public static void ShowYesNo(string message, Action? onYes = null, Action? onNo = null)
    {
        ShowMessageBox(message, "Yes", onYes, "No", onNo);
    }

    /// <summary>
    /// Shows a custom message box with two buttons
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="button1Text">Text for the first button</param>
    /// <param name="button1Callback">Callback for the first button</param>
    /// <param name="button2Text">Text for the second button (optional)</param>
    /// <param name="button2Callback">Callback for the second button (optional)</param>
    public static void ShowMessageBox(
        string message,
        string button1Text,
        Action? button1Callback = null,
        string? button2Text = null,
        Action? button2Callback = null
    )
    {
        if (_uiBase is null)
        {
            throw new InvalidOperationException(
                $"MessageBox system not initialized. Call {nameof(Init)}() first"
            );
        }

        _activeMessageBox?.OnClosePanelClicked();

        _activeMessageBox = new(
            _uiBase,
            message,
            button1Text,
            button1Callback,
            button2Text,
            button2Callback
        );

        _uiBase.Enabled = true;
    }

    #endregion Static Members

    #region Instance Members

    private GameObject? _mainContainer;
    private GameObject? _buttonContainer;

    private readonly string _message;
    private readonly string _button1Text;
    private readonly Action? _button1Callback;
    private readonly string? _button2Text;
    private readonly Action? _button2Callback;

    public override string Name => nameof(MessageBox);
    public override int MinWidth => 350;
    public override int MinHeight => 200;
    public override Vector2 DefaultAnchorMin => new(0.5f, 0.5f);
    public override Vector2 DefaultAnchorMax => new(0.5f, 0.5f);
    public override bool CanDragAndResize => false;

    private MessageBox(
        UIBase owner,
        string message,
        string button1Text,
        Action? button1Callback,
        string? button2Text,
        Action? button2Callback
    )
        : base(owner)
    {
        _message = message;
        _button1Text = button1Text;
        _button1Callback = button1Callback;
        _button2Text = button2Text;
        _button2Callback = button2Callback;
        ConstructPanelRunTime();
    }

    protected override void ConstructPanelContent()
    {
        _mainContainer = UIFactory.CreateVerticalGroup(
            ContentRoot,
            "MainContainer",
            false,
            false,
            true,
            true,
            5,
            new(10, 10, 10, 10)
        );

        UIFactory.SetLayoutElement(
            _mainContainer,
            minHeight: 100,
            flexibleWidth: 9999,
            flexibleHeight: 9999
        );
    }

    private void ConstructPanelRunTime()
    {
        Text messageText = UIFactory.CreateLabel(
            _mainContainer,
            "MessageText",
            _message,
            TextAnchor.MiddleCenter,
            fontSize: 16
        );

        messageText.horizontalOverflow = HorizontalWrapMode.Wrap;
        messageText.verticalOverflow = VerticalWrapMode.Overflow;

        UIFactory.SetLayoutElement(
            messageText.gameObject,
            minHeight: 60,
            flexibleWidth: 9999,
            flexibleHeight: 200
        );

        _buttonContainer = UIFactory.CreateHorizontalGroup(
            _mainContainer,
            "ButtonContainer",
            false,
            true,
            true,
            true,
            5,
            new(5, 10, 5, 5)
        );

        UIFactory.SetLayoutElement(
            _buttonContainer,
            minHeight: 40,
            flexibleWidth: 9999,
            flexibleHeight: 0
        );

        ButtonRef button1 = UIFactory.CreateButton(_buttonContainer, "Button1", _button1Text);

        UIFactory.SetLayoutElement(
            button1.GameObject,
            minHeight: 30,
            minWidth: 100,
            flexibleHeight: 0,
            flexibleWidth: 9999
        );

        button1.OnClick += () =>
        {
            _button1Callback?.Invoke();
            OnClosePanelClicked();
        };

        if (!string.IsNullOrEmpty(_button2Text))
        {
            ButtonRef button2 = UIFactory.CreateButton(_buttonContainer, "Button2", _button2Text);

            UIFactory.SetLayoutElement(
                button2.GameObject,
                minHeight: 30,
                minWidth: 100,
                flexibleHeight: 0,
                flexibleWidth: 9999
            );

            button2.OnClick += () =>
            {
                _button2Callback?.Invoke();
                OnClosePanelClicked();
            };
        }
    }

    protected override void OnClosePanelClicked()
    {
        Owner.Enabled = false;
        Destroy();
        _activeMessageBox = null;
    }

    #endregion Instance Members
}
