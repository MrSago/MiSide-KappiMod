using KappiMod.Properties;
using UnityEngine;
using UniverseLib.UI;

namespace KappiMod.UI.IMGUI;

public static class UIManager
{
    public static bool Enabled
    {
        get => UiBase is not null and { Enabled: true };
        set
        {
            if (UiBase is null || value == UiBase.Enabled)
            {
                return;
            }

            UniversalUI.SetUIActive(BuildInfo.GUID, value);
        }
    }

    internal static UIBase? UiBase { get; private set; }
    internal static MainPanel? Panel { get; private set; }

    public static void Init()
    {
        UiBase = UniversalUI.RegisterUI(BuildInfo.GUID, null);

        Panel = new(UiBase);

        MessageBox.Init();

        KappiCore.Loader.Update += OnUpdate;
    }

    private static void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Enabled = !Enabled;
        }
    }
}
