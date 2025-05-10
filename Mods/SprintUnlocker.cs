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
    name: "Sprint Unlocker",
    description: "Unlocks the sprinting ability in the game.",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public sealed class SprintUnlocker : BaseMod
{
    private PlayerMove? _cachedPlayerMove;

    public override bool IsEnabled
    {
        get => base.IsEnabled && ConfigManager.SprintUnlocker.Value;
        protected set
        {
            base.IsEnabled = value;
            ConfigManager.SprintUnlocker.Value = value;
        }
    }

    protected override void OnInitialize()
    {
        if (ConfigManager.SprintUnlocker.Value)
        {
            OnEnable();
            base.IsEnabled = true;
        }
    }

    protected override void OnEnable()
    {
        KappiModCore.Loader.Update += OnUpdate;
    }

    protected override void OnDisable()
    {
        KappiModCore.Loader.Update -= OnUpdate;
        if (UnityHelpers.IsValid(_cachedPlayerMove))
        {
            SetPlayerRunState(false);
            _cachedPlayerMove = null;
        }
    }

    public void SetPlayerRunState(bool value)
    {
        try
        {
            if (!TryFindPlayerMove() || _cachedPlayerMove == null)
            {
                KappiLogger.LogError($"Object {nameof(PlayerMove)} not found!");
                return;
            }

            _cachedPlayerMove.canRun = value;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set player run state", exception: ex);
        }
    }

    private void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SetPlayerRunState(true);
        }
    }

    private bool TryFindPlayerMove()
    {
        if (UnityHelpers.IsValid(_cachedPlayerMove))
        {
            return true;
        }

        _cachedPlayerMove = GameObject.Find("Player")?.GetComponent<PlayerMove>();
        return UnityHelpers.IsValid(_cachedPlayerMove);
    }
}
