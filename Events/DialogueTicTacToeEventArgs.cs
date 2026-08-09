using UnityEngine;
using UnityEngine.SceneManagement;
using UniverseLib.Utility;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Events;

public sealed class DialogueTicTacToeEventArgs : DialogueEventArgs
{
    private readonly Location18_TicTacToe TicTacToe;

    private DialogueTicTacToeEventArgs(
        string objectName,
        string sceneName,
        int indexString,
        string text,
        GameObject? speaker,
        DialoguePatchType patchType,
        Location18_TicTacToe ticTacToe
    )
        : base(objectName, sceneName, indexString, text, speaker, patchType)
    {
        TicTacToe = ticTacToe;
    }

    public static DialogueTicTacToeEventArgs Create(
        Location18_TicTacToe ticTacToe,
        DialoguePatchType patchType
    )
    {
        Location18_Novella? novella = ticTacToe.main;
        string objectName =
            novella?.dialoguePlay is not null && !novella.dialoguePlay.IsNullOrDestroyed()
                ? novella.dialoguePlay.name
                : ticTacToe.name;

        return new(
            objectName,
            ticTacToe.gameObject.scene.name,
            novella?.indexStringDialogue ?? 0,
            novella?.textNeed ?? string.Empty,
            speaker: null,
            patchType,
            ticTacToe
        );
    }

    public override void Skip()
    {
        if (TicTacToe.IsNullOrDestroyed())
        {
            return;
        }

        if (TicTacToe.timeDialogueNext > 0f && !TicTacToe.main.IsNullOrDestroyed())
        {
            TicTacToe.timeDialogueNext = 0f;
            TicTacToe.main.NextDialogue();
            return;
        }

        if (!TicTacToe.waitClick)
        {
            return;
        }

        TicTacToe.waitClick = false;
        TicTacToe.CanClick(true);
    }
}
