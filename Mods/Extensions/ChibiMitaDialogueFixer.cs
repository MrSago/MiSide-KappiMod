using Il2CppInterop.Runtime;
using KappiMod.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods.Extensions;

internal static class ChibiMitaDialogueFixer
{
    private static Mob_ChibiMita? _cachedChibiMita;

    internal static void Init()
    {
        if (SceneManager.GetActiveScene().name is ObjectNames.CHIBIMITA_SCENE)
        {
            TryFindChibiMita();
        }
        else
        {
            _cachedChibiMita = null;
        }

        KappiModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
        DialogueEventSystem.OnPostfixDialogueStart += HandleDialogue;

        KappiModCore.Log("Initialized");
    }

    internal static void CleanUp()
    {
        _cachedChibiMita = null;

        KappiModCore.Loader.SceneWasInitialized -= OnSceneWasInitialized;
        DialogueEventSystem.OnPostfixDialogueStart -= HandleDialogue;

        KappiModCore.Log("Cleaned up");
    }

    private static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is ObjectNames.CHIBIMITA_SCENE)
        {
            TryFindChibiMita();
        }
        else
        {
            _cachedChibiMita = null;
        }
    }

    private static void HandleDialogue(object? sender, DialogueEventArgs args)
    {
        if (args.ObjectName is not ObjectNames.CHIBIMITA_BROKEN_DIALOGUE)
        {
            return;
        }

        if (_cachedChibiMita == null)
        {
            TryFindChibiMita();
        }

        if (_cachedChibiMita != null)
        {
            _cachedChibiMita.AnimationStop();

            KappiModCore.Log("ChibiMita animation stopped");
        }
        else
        {
            KappiModCore.Log("ChibiMita not found");
        }
    }

    private static void TryFindChibiMita()
    {
        _cachedChibiMita = Resources
            .FindObjectsOfTypeAll(Il2CppType.Of<Mob_ChibiMita>())
            ?.FirstOrDefault(x => x.name == "ChibiMita")
            ?.Cast<Mob_ChibiMita>();

        KappiModCore.Log($"ChibiMita {(_cachedChibiMita == null ? "not found" : "found")}");
    }
}
