using KappiMod.Config;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods;

public static class AlwaysRunEnabler
{
    public static bool Enabled
    {
        get => ConfigManager.AlwaysRunEnabler.Value;
        set
        {
            if (value)
            {
                KappiModCore.Loader.Update += OnUpdate;
            }
            else
            {
                KappiModCore.Loader.Update -= OnUpdate;
            }

            KappiModCore.Log($"[{nameof(AlwaysRunEnabler)}] " + (value ? "Enabled" : "Disabled"));

            ConfigManager.AlwaysRunEnabler.Value = value;
        }
    }

    public static void Init()
    {
        if (Enabled)
        {
            KappiModCore.Loader.Update += OnUpdate;
        }

        KappiModCore.Log($"[{nameof(AlwaysRunEnabler)}] Initialized");
    }

    public static void SetPlayerRunState(bool value)
    {
        try
        {
            PlayerMove? playerMove = UnityEngine.Object.FindObjectOfType<PlayerMove>();
            if (playerMove is not null)
            {
                playerMove.canRun = value;
            }
        }
        catch (Exception e)
        {
            KappiModCore.LogError($"[{nameof(AlwaysRunEnabler)}] {e.Message}");
        }
    }

    private static void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SetPlayerRunState(true);
        }
    }
}
