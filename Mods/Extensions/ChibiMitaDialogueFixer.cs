using Il2CppInterop.Runtime;
using KappiMod.Events;
using KappiMod.Utils;
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
    private static bool _isInitialized = false;
    private static Mob_ChibiMita? _cachedChibiMita;

    internal static void Init()
    {
        if (_isInitialized)
        {
            KappiModCore.LogError($"{nameof(ChibiMitaDialogueFixer)} is already initialized");
            return;
        }

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

        _isInitialized = true;
        KappiModCore.Log("Initialized");
    }

    internal static void CleanUp()
    {
        if (!_isInitialized)
        {
            KappiModCore.LogError($"{nameof(ChibiMitaDialogueFixer)} is not initialized");
            return;
        }

        _cachedChibiMita = null;

        KappiModCore.Loader.SceneWasInitialized -= OnSceneWasInitialized;
        DialogueEventSystem.OnPostfixDialogueStart -= HandleDialogue;

        _isInitialized = false;
        KappiModCore.Log("Cleaned up");
    }

    private static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is ObjectNames.CHIBIMITA_SCENE)
        {
            _cachedChibiMita = null;
            TryFindChibiMita();
        }
        else if (_cachedChibiMita != null)
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

        if (!TryFindChibiMita() || _cachedChibiMita == null)
        {
            return;
        }

        _cachedChibiMita.AnimationStop();
        KappiModCore.Log("ChibiMita animation stopped");
    }

    private static bool TryFindChibiMita()
    {
        if (UnityHelpers.IsValid(_cachedChibiMita))
        {
            return true;
        }

        _cachedChibiMita = Resources
            .FindObjectsOfTypeAll(Il2CppType.Of<Mob_ChibiMita>())
            ?.FirstOrDefault(x => x.name == "ChibiMita")
            ?.Cast<Mob_ChibiMita>();

        bool isFound = UnityHelpers.IsValid(_cachedChibiMita);
        KappiModCore.Log($"ChibiMita {(isFound ? "found" : "not found")}");
        return isFound;
    }
}
