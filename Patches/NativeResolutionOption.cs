using HarmonyLib;
using Il2CppInterop.Runtime;
using KappiMod.Utils;
using UnityEngine;
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
            KappiModCore.LogError($"{nameof(NativeResolutionOption)} is already initialized");
            return;
        }

        _harmony = new("com.miside.nativeresolutionoption");
        _harmony.PatchAll(typeof(Patch));

        _isInitialized = true;
        KappiModCore.Log("Initialized");
    }

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPatch(typeof(ButtonMouseClick), "OnPointerDown")]
        private static void Postfix(ButtonMouseClick __instance)
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
            catch (Exception e)
            {
                KappiModCore.LogError(e.Message);
            }
        }

        private static void AddNativeResolutionOption()
        {
            MenuCaseOption? menuCaseOption = Resources
                .FindObjectsOfTypeAll(Il2CppType.Of<MenuCaseOption>())
                ?.FirstOrDefault(x => x.name == "Button Resolution")
                ?.Cast<MenuCaseOption>();
            if (!UnityHelpers.IsValid(menuCaseOption) || menuCaseOption == null)
            {
                KappiModCore.LogError("MenuCaseOption not found");
                return;
            }

            Resolution resolution = GetNativeResolution();
            KappiModCore.Log(
                $"Native resolution: {resolution.width}x{resolution.height}@{resolution.refreshRate}Hz"
            );

            const string buttonText = "Native Resolution";
            foreach (var buttonInfo in menuCaseOption.scrIccb)
            {
                if (buttonInfo.buttonText is buttonText)
                {
                    KappiModCore.Log("Option is already exists");
                    return;
                }
            }

            int index = menuCaseOption.resolutions.IndexOf(resolution);
            index = index >= 0 ? index : menuCaseOption.resolutions.Count - 1;

            menuCaseOption.scrIccb.Add(new() { buttonText = buttonText, value_int = index });
            KappiModCore.Log("Option successfully added");
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
