using KappiMod.Config;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods;

public static class SitUnlocker
{
    public static bool Enabled
    {
        get => ConfigManager.SitUnlocker.Value;
        set
        {
            if (value)
            {
                KappiModCore.Loader.Update += OnUpdate;
            }
            else
            {
                KappiModCore.Loader.Update -= OnUpdate;
                SetPlayerSitState(false);
            }

            KappiModCore.Log(value ? "Enabled" : "Disabled");

            ConfigManager.SitUnlocker.Value = value;
        }
    }

    private static PlayerMove? _cachedPlayerMove;

    public static void Init()
    {
        if (Enabled)
        {
            KappiModCore.Loader.Update += OnUpdate;
        }

        KappiModCore.Log("Initialized");
    }

    public static void SetPlayerSitState(bool value)
    {
        try
        {
            TryFindPlayerMove();
            if (_cachedPlayerMove == null)
            {
                KappiModCore.LogError("PlayerMove component not found!");
                return;
            }

            _cachedPlayerMove.canSit = value;
        }
        catch (Exception e)
        {
            KappiModCore.LogError(e.Message);
        }
    }

    private static void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SetPlayerSitState(true);
        }
    }

    private static void TryFindPlayerMove()
    {
        if (!IsPlayerMoveValid())
        {
            _cachedPlayerMove = GameObject.Find("Player")?.GetComponent<PlayerMove>();
        }
    }

    private static bool IsPlayerMoveValid()
    {
        return _cachedPlayerMove != null && _cachedPlayerMove.gameObject != null;
    }
}
