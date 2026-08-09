using UnityEngine;
using UniverseLib.Utility;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Events;

public sealed class DialogueTamagotchiEventArgs : DialogueEventArgs
{
    private readonly Tamagotchi_Dialogue Dialogue;

    private DialogueTamagotchiEventArgs(
        string objectName,
        string sceneName,
        int indexString,
        string text,
        GameObject? speaker,
        DialoguePatchType patchType,
        Tamagotchi_Dialogue dialogue
    )
        : base(objectName, sceneName, indexString, text, speaker, patchType)
    {
        Dialogue = dialogue;
    }

    public static DialogueTamagotchiEventArgs Create(
        Tamagotchi_Dialogue dialogue,
        DialoguePatchType patchType
    )
    {
        Tamagotchi_Dialogue_Mob? mob = dialogue.dialogueRun;
        string objectName =
            mob is not null && !mob.IsNullOrDestroyed() ? mob.name : dialogue.name;

        int indexString = dialogue.dialogueIndex;
        if (
            mob is not null
            && !mob.IsNullOrDestroyed()
            && mob.dialogue is not null
            && indexString >= 0
            && indexString < mob.dialogue.Length
            && mob.dialogue[indexString] is not null
        )
        {
            indexString = mob.dialogue[indexString].indexString;
        }

        string text = dialogue.textDialogue is not null
            ? dialogue.textDialogue.text ?? string.Empty
            : dialogue.stringDialogueNeed ?? string.Empty;

        GameObject? speaker = !dialogue.mita.IsNullOrDestroyed()
            ? dialogue.mita.gameObject
            : null;

        return new(
            objectName,
            // Object scene — GetActiveScene() is wrong with additive chapter loads.
            dialogue.gameObject.scene.name,
            indexString,
            text,
            speaker,
            patchType,
            dialogue
        );
    }

    public override void Skip()
    {
        if (Dialogue.IsNullOrDestroyed() || !Dialogue.enableDialogue || Dialogue.play)
        {
            return;
        }

        Dialogue.KeyNextDialogue();
    }
}
