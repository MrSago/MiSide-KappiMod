using UnityEngine;

namespace KappiMod.UI.Internal.EventDisplay;

/// <summary>
/// Original source code was taken from: https://github.com/SliceCraft/MiSideSpeedrunMod
/// </summary>
internal class ModEvent
{
    internal string EventString { get; }
    internal GameObject? HintObject = null;
    internal float TimeUntilHide = 9f;
    internal float TimeUntilDestroy = 10f;

    internal ModEvent(string eventString)
    {
        EventString = eventString;
    }
}
