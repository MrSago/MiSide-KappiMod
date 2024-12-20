using UnityEngine;
using UnityEngine.UIElements;

namespace KappiMod.Mods;

public static class TimeScaleScroller
{
    private static bool _enabled = true;
    public static bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            if (!_enabled && !Mathf.Approximately(Time.timeScale, 1.0f))
            {
                ResetTimeScale();
            }

            KappiModCore.Log(
                $"[{nameof(TimeScaleScroller)}] " + (_enabled ? "Enabled" : "Disabled")
            );
        }
    }

    private static bool _shiftPressed = false;

    public static void Init()
    {
        KappiModCore.Loader.Update += OnUpdate;

        KappiModCore.Log($"[{nameof(TimeScaleScroller)}] Initialized");
    }

    private static void OnUpdate(object sender)
    {
        if (!_enabled)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _shiftPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _shiftPressed = false;
        }

        if (_shiftPressed && Input.mouseScrollDelta.y > 0.0f)
        {
            SetTimeScale(Time.timeScale + 0.1f);
        }
        else if (_shiftPressed && Input.mouseScrollDelta.y < 0.0f)
        {
            SetTimeScale(Time.timeScale - 0.1f);
        }
        else if (_shiftPressed && Input.GetMouseButtonDown((int)MouseButton.MiddleMouse))
        {
            SetTimeScale(Mathf.Approximately(Time.timeScale, 1.0f) ? 0.0f : 1.0f);
        }
    }

    private static void SetTimeScale(float timeScale)
    {
        Time.timeScale = Mathf.Max(0.0f, timeScale);
        KappiModCore.Log($"[{nameof(TimeScaleScroller)}] TimeScale: {Time.timeScale}");
    }

    private static void ResetTimeScale()
    {
        SetTimeScale(1.0f);
    }
}
