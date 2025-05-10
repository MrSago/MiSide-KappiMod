using KappiMod.Config;
using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace KappiMod.Mods;

[ModInfo(
    name: "Time Scale Scroller",
    description: "Allows you to adjust the time scale using the mouse scroll wheel.",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public sealed class TimeScaleScroller : BaseMod
{
    public override bool IsEnabled
    {
        get => base.IsEnabled && ConfigManager.TimeScaleScroller.Value;
        protected set
        {
            base.IsEnabled = value;
            ConfigManager.TimeScaleScroller.Value = value;
        }
    }

    protected override void OnInitialize()
    {
        if (ConfigManager.TimeScaleScroller.Value)
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
        if (!Mathf.Approximately(Time.timeScale, 1.0f))
        {
            SetTimeScale(1.0f);
        }
    }

    public static void SetTimeScale(float timeScale)
    {
        try
        {
            Time.timeScale = Mathf.Max(0.0f, timeScale);
            KappiLogger.Log($"TimeScale: {Time.timeScale}");
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to set time scale", exception: ex);
        }
    }

    private void OnUpdate()
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
