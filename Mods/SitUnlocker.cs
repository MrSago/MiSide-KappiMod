using KappiMod.Config;
using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Properties;
using KappiMod.Utils;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods;

[ModInfo(
    name: "Sit Unlocker",
    description: "Unlocks the ability to sit in the game.",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public sealed class SitUnlocker : BaseMod
{
    private PlayerMove? _cachedPlayerMove;

    public override bool IsEnabled
    {
        get => base.IsEnabled && ConfigManager.SitUnlocker.Value;
        protected set
        {
            base.IsEnabled = value;
            ConfigManager.SitUnlocker.Value = value;
        }
    }

    protected override void OnInitialize()
    {
        if (ConfigManager.SitUnlocker.Value)
        {
            OnEnable();
            base.IsEnabled = true;
        }
    }

    protected override void OnEnable()
    {
        KappiCore.Loader.Update += OnUpdate;
    }

    protected override void OnDisable()
    {
        KappiCore.Loader.Update -= OnUpdate;
        if (Helpers.IsValid(_cachedPlayerMove))
        {
            SetPlayerSitState(false);
            _cachedPlayerMove = null;
        }
    }

    public void SetPlayerSitState(bool value)
    {
        try
        {
            if (!TryFindPlayerMove() || _cachedPlayerMove == null)
            {
                KappiLogger.LogError($"Object {nameof(PlayerMove)} not found!");
                return;
            }

            _cachedPlayerMove.canSit = value;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set player sit state", exception: ex);
        }
    }

    private void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SetPlayerSitState(true);
        }
    }

    private bool TryFindPlayerMove()
    {
        if (Helpers.IsValid(_cachedPlayerMove))
        {
            return true;
        }

        _cachedPlayerMove = GameObject.Find("Player")?.GetComponent<PlayerMove>();
        return Helpers.IsValid(_cachedPlayerMove);
    }
}
