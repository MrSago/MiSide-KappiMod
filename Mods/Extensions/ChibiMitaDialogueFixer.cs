using Il2CppInterop.Runtime;
using KappiMod.Events;
using KappiMod.Logging;
using KappiMod.Patches;
using KappiMod.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods.Extensions;

internal class ChibiMitaDialogueFixer
{
    private const string BROKEN_DIALOGUE = "3D TextFactory 5";

    private readonly DialogueStartPatch _dialoguePatch;

    private bool _isInitialized = false;
    private Mob_ChibiMita? _cachedChibiMita;

    internal ChibiMitaDialogueFixer(DialogueStartPatch dialoguePatch)
    {
        _dialoguePatch = dialoguePatch;
    }

    internal void Init()
    {
        if (_isInitialized)
        {
            KappiLogger.LogError($"{nameof(ChibiMitaDialogueFixer)} is already initialized");
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

        KappiCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
        _dialoguePatch.OnPostfixDialogueStart += HandleDialogue;

        _isInitialized = true;
        KappiLogger.Log("Initialized");
    }

    internal void CleanUp()
    {
        if (!_isInitialized)
        {
            KappiLogger.LogError($"{nameof(ChibiMitaDialogueFixer)} is not initialized");
            return;
        }

        _cachedChibiMita = null;

        KappiCore.Loader.SceneWasInitialized -= OnSceneWasInitialized;
        _dialoguePatch.OnPostfixDialogueStart -= HandleDialogue;

        _isInitialized = false;
        KappiLogger.Log("Cleaned up");
    }

    private void OnSceneWasInitialized(int buildIndex, string sceneName)
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

    private void HandleDialogue(object? sender, DialogueEventArgs args)
    {
        if (args.ObjectName is not BROKEN_DIALOGUE)
        {
            return;
        }

        if (!TryFindChibiMita() || _cachedChibiMita == null)
        {
            return;
        }

        _cachedChibiMita.AnimationStop();
        KappiLogger.Log("ChibiMita animation stopped");
    }

    private bool TryFindChibiMita()
    {
        if (Helpers.IsValid(_cachedChibiMita))
        {
            return true;
        }

        _cachedChibiMita = Resources
            .FindObjectsOfTypeAll(Il2CppType.Of<Mob_ChibiMita>())
            ?.FirstOrDefault(x => x.name == "ChibiMita")
            ?.Cast<Mob_ChibiMita>();

        bool isFound = Helpers.IsValid(_cachedChibiMita);
        KappiLogger.Log($"ChibiMita {(isFound ? "found" : "not found")}");
        return isFound;
    }
}
