using KappiMod.Config;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods;

public static class FlashlightIncreaser
{
    private const float NOT_INITIALIZED = -1.0f;
    private const float FLASHLIGHT_RANGE = 1000.0f;
    private const float FLASHLIGHT_SPOT_ANGLE = 70.0f;

    private static bool _isInitialized = false;
    private static bool _isFlashlightEnabled = false;
    private static float _savedFlashlightRange = NOT_INITIALIZED;
    private static float _savedFlashlightSpotAngle = NOT_INITIALIZED;
    private static WorldPlayer? _cachedWorldPlayer;

    public static bool Enabled
    {
        get => _isInitialized && ConfigManager.FlashlightIncreaser.Value;
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
                if (_isFlashlightEnabled)
                {
                    Toggle();
                }
            }

            KappiModCore.Log(value ? "Enabled" : "Disabled");

            ConfigManager.FlashlightIncreaser.Value = value;
        }
    }

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiModCore.LogError($"{nameof(FlashlightIncreaser)} is already initialized");
            return;
        }

        _isInitialized = true;

        if (Enabled)
        {
            KappiModCore.Loader.Update += OnUpdate;
        }

        KappiModCore.Log("Initialized");
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

        KappiModCore.Log($"Flashlight {(_isFlashlightEnabled ? "increased" : "restored")}");

        return _isFlashlightEnabled;
    }

    private static void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Toggle();
        }
    }

    private static void ActivateFlashlightFeatures()
    {
        try
        {
            TryFindWorldPlayer();
            if (_cachedWorldPlayer == null)
            {
                KappiModCore.LogError($"Object {nameof(WorldPlayer)} not found!");

                ResetState();
                return;
            }

            _savedFlashlightRange = _cachedWorldPlayer.flashLightRange;
            _savedFlashlightSpotAngle = _cachedWorldPlayer.flashLightSpotAngle;

            _cachedWorldPlayer.flashLightRange = FLASHLIGHT_RANGE;
            _cachedWorldPlayer.flashLightSpotAngle = FLASHLIGHT_SPOT_ANGLE;
        }
        catch (Exception e)
        {
            KappiModCore.LogError(e.Message);

            ResetState();
        }
    }

    private static void RevertFlashlightState()
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
        catch (Exception e)
        {
            KappiModCore.LogError(e.Message);

            ResetState();
        }
    }

    private static void TryFindWorldPlayer()
    {
        if (!IsWoldPlayerValid())
        {
            _cachedWorldPlayer = GameObject.Find("World")?.GetComponent<WorldPlayer>();
        }
    }

    private static bool IsWoldPlayerValid()
    {
        return _cachedWorldPlayer != null && _cachedWorldPlayer.gameObject != null;
    }

    private static void ResetState()
    {
        _cachedWorldPlayer = null;
        _isFlashlightEnabled = false;
        _savedFlashlightRange = NOT_INITIALIZED;
        _savedFlashlightSpotAngle = NOT_INITIALIZED;
    }
}
