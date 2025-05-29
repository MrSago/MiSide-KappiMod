using UnityEngine;
using UnityEngine.SceneManagement;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Events;

public enum DialoguePatchType
{
    Postfix,
    Prefix,
}

public class DialogueEventArgs : EventArgs
{
    public Dialogue_3DText DialogueInstance { get; private init; }
    public string ObjectName { get; private init; }
    public string SceneName { get; private init; }
    public int IndexString { get; private init; }
    public string Text { get; private init; }
    public GameObject? Speaker { get; private init; }
    public DialoguePatchType PatchType { get; private init; }

    public DialogueEventArgs(
        Dialogue_3DText dialogueInstance,
        string objectName,
        string sceneName,
        int indexString,
        string text,
        GameObject? speaker,
        DialoguePatchType patchType
    )
    {
        DialogueInstance = dialogueInstance;
        ObjectName = objectName;
        SceneName = sceneName;
        IndexString = indexString;
        Text = text;
        Speaker = speaker;
        PatchType = patchType;
    }

    public static DialogueEventArgs Create(Dialogue_3DText instance, DialoguePatchType patchType) =>
        new(
            instance,
            instance.name,
            SceneManager.GetActiveScene().name,
            instance.indexString,
            instance.textPrint,
            instance.speaker,
            patchType
        );
}
