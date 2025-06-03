using HarmonyLib;
using Il2CppInterop.Runtime;
using KappiMod.Logging;
using UnityEngine;
using UniverseLib.Utility;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches;

public static class NativeResolutionOption
{
    private static bool _isInitialized = false;
    private static HarmonyLib.Harmony _harmony = null!;

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiLogger.LogError($"{nameof(NativeResolutionOption)} is already initialized");
            return;
        }

        _harmony = new("com.kappimod.nativeresolutionoption");
        _harmony.PatchAll(typeof(Patch));

        _isInitialized = true;
        KappiLogger.Log("Initialized");
    }

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ButtonMouseClick), nameof(ButtonMouseClick.OnPointerDown))]
        private static void AddResolutionOption(ButtonMouseClick __instance)
        {
            if (
                __instance.name != "Button Option Graphics"
                && __instance.name != "Button Options Graphics"
            )
            {
                return;
            }

            try
            {
                AddNativeResolutionOption();
            }
            catch (Exception ex)
            {
                KappiLogger.LogException("Failed to add native resolution option", exception: ex);
            }
        }

        private static void AddNativeResolutionOption()
        {
            MenuCaseOption? menuCaseOption = Resources
                .FindObjectsOfTypeAll(Il2CppType.Of<MenuCaseOption>())
                ?.FirstOrDefault(x => x.name == "Button Resolution")
                ?.Cast<MenuCaseOption>();
            if (UnityHelpers.IsNullOrDestroyed(menuCaseOption) || menuCaseOption == null)
            {
                KappiLogger.LogError("MenuCaseOption not found");
                return;
            }

            Resolution resolution = GetNativeResolution();
            KappiLogger.Log(
                $"Native resolution: {resolution.width}x{resolution.height}@{resolution.refreshRate}Hz"
            );

            const string buttonText = "Native Resolution";
            foreach (var buttonInfo in menuCaseOption.scrIccb)
            {
                if (buttonInfo.buttonText is buttonText)
                {
                    KappiLogger.Log("Option is already exists");
                    return;
                }
            }

            int index = menuCaseOption.resolutions.IndexOf(resolution);
            index = index >= 0 ? index : menuCaseOption.resolutions.Count - 1;

            menuCaseOption.scrIccb.Add(new() { buttonText = buttonText, value_int = index });
            KappiLogger.Log("Option successfully added");
        }

        private static Resolution GetNativeResolution()
        {
            Display primaryDisplay = Display.main;
            int nativeWidth = primaryDisplay.systemWidth;
            int nativeHeight = primaryDisplay.systemHeight;

            int maxRefreshRate =
                Screen
                    .resolutions.Where(r => r.width == nativeWidth && r.height == nativeHeight)
                    ?.Max(r => r.refreshRate) ?? Screen.resolutions.Max(r => r.refreshRate);

            return new Resolution
            {
                width = nativeWidth,
                height = nativeHeight,
                refreshRate = maxRefreshRate,
            };
        }
    }
}
