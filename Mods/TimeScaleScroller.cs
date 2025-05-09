using KappiMod.Config;
using UnityEngine;
using UnityEngine.UIElements;

namespace KappiMod.Mods;

public static class TimeScaleScroller
{
    private static bool _isInitialized = false;

    public static bool Enabled
    {
        get => _isInitialized && ConfigManager.TimeScaleScroller.Value;
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
                if (!Mathf.Approximately(Time.timeScale, 1.0f))
                {
                    SetTimeScale(1.0f);
                }
            }

            KappiModCore.Log(value ? "Enabled" : "Disabled");
            ConfigManager.TimeScaleScroller.Value = value;
        }
    }

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiModCore.LogError($"{nameof(TimeScaleScroller)} is already initialized");
            return;
        }

        _isInitialized = true;

        if (Enabled)
        {
            KappiModCore.Loader.Update += OnUpdate;
        }

        KappiModCore.Log("Initialized");
    }

    public static void SetTimeScale(float timeScale)
    {
        try
        {
            Time.timeScale = Mathf.Max(0.0f, timeScale);
            KappiModCore.Log($"TimeScale: {Time.timeScale}");
        }
        catch (Exception ex)
        {
            KappiModCore.LogError(ex.Message);
        }
    }

    private static void OnUpdate()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            return;
        }

        if (Input.mouseScrollDelta.y > 0.0f)
        {
            SetTimeScale(Time.timeScale + 0.1f);
        }
        else if (Input.mouseScrollDelta.y < 0.0f)
        {
            SetTimeScale(Time.timeScale - 0.1f);
        }
        else if (Input.GetMouseButtonDown((int)MouseButton.MiddleMouse))
        {
            SetTimeScale(Mathf.Approximately(Time.timeScale, 1.0f) ? 0.0f : 1.0f);
        }
    }
}
