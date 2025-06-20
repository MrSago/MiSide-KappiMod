using KappiMod.Config;
using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Properties;
using UnityEngine;
using UniverseLib.Utility;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods;

[ModInfo(
    name: "Flashlight Increaser",
    description: "Enhances the flashlight range and angle for better visibility",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public sealed class FlashlightIncreaser : BaseMod
{
    private const float NOT_INITIALIZED = -1.0f;
    private const float FLASHLIGHT_RANGE = 1000.0f;
    private const float FLASHLIGHT_SPOT_ANGLE = 70.0f;

    private bool _isFlashlightEnabled = false;
    private float _savedFlashlightRange = NOT_INITIALIZED;
    private float _savedFlashlightSpotAngle = NOT_INITIALIZED;
    private WorldPlayer? _cachedWorldPlayer;

    public override bool IsEnabled
    {
        get => base.IsEnabled && ConfigManager.FlashlightIncreaser.Value;
        protected set
        {
            base.IsEnabled = value;
            ConfigManager.FlashlightIncreaser.Value = value;
        }
    }

    protected override void OnInitialize()
    {
        if (ConfigManager.FlashlightIncreaser.Value)
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
        if (_isFlashlightEnabled)
        {
            Toggle();
        }
    }

    public bool Toggle()
    {
        _isFlashlightEnabled = !_isFlashlightEnabled;
        if (_isFlashlightEnabled)
        {
            ActivateFlashlightFeatures();
        }
        else
        {
            RevertFlashlightState();
        }

        KappiLogger.Log($"Flashlight {(_isFlashlightEnabled ? "increased" : "restored")}");
        return _isFlashlightEnabled;
    }

    private void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Toggle();
        }
    }

    private void ActivateFlashlightFeatures()
    {
        try
        {
            if (!TryFindWorldPlayer() || _cachedWorldPlayer == null)
            {
                KappiLogger.LogError($"Object {nameof(WorldPlayer)} not found!");
                ResetState();
                return;
            }

            _savedFlashlightRange = _cachedWorldPlayer.flashLightRange;
            _savedFlashlightSpotAngle = _cachedWorldPlayer.flashLightSpotAngle;

            _cachedWorldPlayer.flashLightRange = FLASHLIGHT_RANGE;
            _cachedWorldPlayer.flashLightSpotAngle = FLASHLIGHT_SPOT_ANGLE;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to activate flashlight features", exception: ex);
            ResetState();
        }
    }

    private void RevertFlashlightState()
    {
        try
        {
            if (
                _cachedWorldPlayer == null
                || _savedFlashlightRange <= NOT_INITIALIZED
                || _savedFlashlightSpotAngle <= NOT_INITIALIZED
            )
            {
                ResetState();
                return;
            }

            _cachedWorldPlayer.flashLightRange = _savedFlashlightRange;
            _cachedWorldPlayer.flashLightSpotAngle = _savedFlashlightSpotAngle;

            ResetState();
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to revert flashlight state", exception: ex);
            ResetState();
        }
    }

    private bool TryFindWorldPlayer()
    {
        if (!UnityHelpers.IsNullOrDestroyed(_cachedWorldPlayer))
        {
            return true;
        }

        _cachedWorldPlayer = GameObject.Find("World")?.GetComponent<WorldPlayer>();
        return !UnityHelpers.IsNullOrDestroyed(_cachedWorldPlayer);
    }

    private void ResetState()
    {
        _cachedWorldPlayer = null;
        _isFlashlightEnabled = false;
        _savedFlashlightRange = NOT_INITIALIZED;
        _savedFlashlightSpotAngle = NOT_INITIALIZED;
    }
}
