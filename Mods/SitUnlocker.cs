using KappiMod.Config;
using KappiMod.Utils;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods;

public static class SitUnlocker
{
    private static bool _isInitialized = false;
    private static PlayerMove? _cachedPlayerMove;

    public static bool Enabled
    {
        get => _isInitialized && ConfigManager.SitUnlocker.Value;
        set
        {
            if (!_isInitialized || value == Enabled)
            {
                return;
            }

            if (value)
            {
                KappiModCore.Loader.Update += OnUpdate;
            }
            else
            {
                KappiModCore.Loader.Update -= OnUpdate;
                if (UnityHelpers.IsValid(_cachedPlayerMove))
                {
                    SetPlayerSitState(false);
                }
            }

            KappiModCore.Log(value ? "Enabled" : "Disabled");
            ConfigManager.SitUnlocker.Value = value;
        }
    }

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiModCore.LogError($"{nameof(SitUnlocker)} is already initialized");
            return;
        }

        _isInitialized = true;

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
            if (!TryFindPlayerMove() || _cachedPlayerMove == null)
            {
                KappiModCore.LogError("PlayerMove component not found!");
                return;
            }

            _cachedPlayerMove.canSit = value;
        }
        catch (Exception ex)
        {
            KappiModCore.LogError(ex.Message);
        }
    }

    private static void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SetPlayerSitState(true);
        }
    }

    private static bool TryFindPlayerMove()
    {
        if (UnityHelpers.IsValid(_cachedPlayerMove))
        {
            return true;
        }

        _cachedPlayerMove = GameObject.Find("Player")?.GetComponent<PlayerMove>();
        return UnityHelpers.IsValid(_cachedPlayerMove);
    }
}
