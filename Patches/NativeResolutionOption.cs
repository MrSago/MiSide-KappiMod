using HarmonyLib;
using Il2CppInterop.Runtime;
using UnityEngine;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Patches;

public static class NativeResolutionOption
{
    private static HarmonyLib.Harmony _harmony = null!;

    public static void Init()
    {
        _harmony = new("com.miside.nativeresolutionoption");
        _harmony.PatchAll(typeof(Patch));

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
            if (menuCaseOption == null)
            {
                KappiModCore.LogError("MenuCaseOption not found");
                return;
            }

            Resolution resolution = GetNativeResolution();
            string buttonText = "Native Resolution";

            KappiModCore.Log(
                $"Native resolution: {resolution.width}x{resolution.height}@{resolution.refreshRate}Hz"
            );

            foreach (var buttonInfo in menuCaseOption.scrIccb)
            {
                if (buttonInfo.buttonText == buttonText)
                {
                    KappiModCore.Log("Option is already exists");
                    return;
                }
            }

            int index = menuCaseOption.resolutions.IndexOf(resolution);
            index = index >= 0 ? index : menuCaseOption.resolutions.Count - 1;

            Interface_ChangeScreenButton_Class_ButtonInfo newButtonInfo = new()
            {
                buttonText = buttonText,
                value_int = index,
            };
            menuCaseOption.scrIccb.Add(newButtonInfo);

            KappiModCore.Log("Option successfully added");
        }

        private static Resolution GetNativeResolution()
        {
            Display primaryDisplay = Display.main;
            int nativeWidth = primaryDisplay.systemWidth;
            int nativeHeight = primaryDisplay.systemHeight;

            int maxRefreshRate = Screen
                .resolutions.Where(r => r.width == nativeWidth && r.height == nativeHeight)
                .Max(r => r.refreshRate);

            return new Resolution
            {
                width = nativeWidth,
                height = nativeHeight,
                refreshRate = maxRefreshRate,
            };
        }
    }
}
