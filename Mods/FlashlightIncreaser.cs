using Il2Cpp;
using UnityEngine;

namespace ModSide.Mods;

public static class FlashlightIncreaser
{
    private const float NOT_INITIALIZED = -1.0f;
    private const float FLASHLIGHT_RANGE = 1000.0f;
    private const float FLASHLIGHT_SPOT_ANGLE = 70.0f;

    private static bool _enabled = true;
    public static bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            if (!_enabled && _isFlashlightEnabled)
            {
                Toggle();
            }

            ModSideCore.Log(
                $"[{nameof(FlashlightIncreaser)}] " + (_enabled ? "Enabled" : "Disabled")
            );
        }
    }

    private static bool _isFlashlightEnabled = false;

    private static WorldPlayer? _player;

    private static float _savedFlashlightRange = NOT_INITIALIZED;
    private static float _savedFlashlightSpotAngle = NOT_INITIALIZED;

    public static void Init()
    {
        ModSideCore.Loader.Update += OnUpdate;

        ModSideCore.Log($"[{nameof(FlashlightIncreaser)}] Initialized");
    }

    private static void OnUpdate(object sender)
    {
        if (!_enabled)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Toggle();
        }
    }

    public static bool Toggle()
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

        ModSideCore.Log(
            $"[{nameof(FlashlightIncreaser)}] Flashlight "
                + (_isFlashlightEnabled ? "increased" : "restored")
        );
        return _isFlashlightEnabled;
    }

    private static void ActivateFlashlightFeatures()
    {
        try
        {
            _player = UnityEngine.Object.FindObjectOfType<WorldPlayer>();
            if (_player is null)
            {
                ModSideCore.LogError(
                    $"[{nameof(FlashlightIncreaser)}] Object {nameof(WorldPlayer)} not found!"
                );

                _isFlashlightEnabled = false;
                return;
            }

            _savedFlashlightRange = _player.flashLightRange;
            _savedFlashlightSpotAngle = _player.flashLightSpotAngle;

            _player.flashLightRange = FLASHLIGHT_RANGE;
            _player.flashLightSpotAngle = FLASHLIGHT_SPOT_ANGLE;
        }
        catch (Exception e)
        {
            ModSideCore.LogError($"[{nameof(FlashlightIncreaser)}] {e.Message}");

            _isFlashlightEnabled = false;
            _player = null;
        }
    }

    private static void RevertFlashlightState()
    {
        try
        {
            if (_player is not null)
            {
                if (
                    _savedFlashlightRange <= NOT_INITIALIZED
                    || _savedFlashlightSpotAngle <= NOT_INITIALIZED
                )
                {
                    return;
                }

                _player.flashLightRange = _savedFlashlightRange;
                _player.flashLightSpotAngle = _savedFlashlightSpotAngle;
                _player = null;

                _savedFlashlightRange = NOT_INITIALIZED;
                _savedFlashlightSpotAngle = NOT_INITIALIZED;
            }
        }
        catch (Exception e)
        {
            ModSideCore.LogError($"[{nameof(FlashlightIncreaser)}] {e.Message}");

            _isFlashlightEnabled = false;
            _player = null;
        }
    }
}
