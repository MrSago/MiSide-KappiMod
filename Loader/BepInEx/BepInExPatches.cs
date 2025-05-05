#if BIE

using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KappiMod.Loader.BepInEx;

[HarmonyPatch]
public static class BepInExPatches
{
    [HarmonyPatch(typeof(SceneManager), "Internal_SceneLoaded")]
    [HarmonyPostfix]
    private static void SceneLoadedPostfix(Scene scene)
    {
        KappiModBepInExPlugin.Instance?.OnSceneWasLoaded(scene.buildIndex, scene.name);
        KappiModBepInExPlugin.Instance?.OnSceneWasInitialized(scene.buildIndex, scene.name);
    }
}

#endif // BIE
