#if BIE

using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KappiMod.Loader.BepInEx;

[HarmonyPatch]
public static class BepInExPatches
{
    [HarmonyPatch(typeof(SceneManager), nameof(SceneManager.Internal_SceneLoaded))]
    [HarmonyPostfix]
    private static void SceneLoadedPostfix(Scene scene)
    {
        KappiBepInExPlugin.Instance?.OnSceneWasLoaded(scene.buildIndex, scene.name);
        KappiBepInExPlugin.Instance?.OnSceneWasInitialized(scene.buildIndex, scene.name);
    }
}

#endif // BIE
