using UnityEngine;
using UnityEngine.UI;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.UI.Internal.EventDisplay;

/// <summary>
/// Original source code was taken from: https://github.com/SliceCraft/MiSideSpeedrunMod
/// </summary>
internal static class EventManager
{
    private static GameObject? _hintScreenTemplate;
    private static GameObject? _interfaceObject;
    private static readonly List<ModEvent> EventObjects = new();
    private static readonly int Hide = Animator.StringToHash("Hide");

    public static void Init()
    {
        KappiCore.Loader.Update += Update;
    }

    public static void ShowEvent(ModEvent modEvent)
    {
        EnsureObjectsSelected();
        GameObject? hintScreenObject = UnityEngine.Object.Instantiate(
            _hintScreenTemplate,
            _interfaceObject?.gameObject.transform
        );

        Text? text = hintScreenObject?.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = modEvent.EventString;
        }

        modEvent.HintObject = hintScreenObject;
        EventObjects.Add(modEvent);
        UpdatePositions();

        hintScreenObject?.SetActive(true);
    }

    private static void UpdatePositions()
    {
        for (int i = EventObjects.Count - 1; i >= 0; --i)
        {
            GameObject? hintObject = EventObjects[i].HintObject;
            if (hintObject == null)
            {
                continue;
            }

            Vector3 hintObjectPosition = hintObject.transform.position;
            hintObjectPosition.y = i * 100 + 100;
            hintObject.transform.position = hintObjectPosition;
        }
    }

    internal static void Update()
    {
        List<ModEvent> objectsToBeRemoved = new();

        foreach (var modEvent in EventObjects)
        {
            if (modEvent.HintObject == null)
            {
                objectsToBeRemoved.Add(modEvent);
                continue;
            }

            modEvent.TimeUntilHide -= Time.deltaTime;
            modEvent.TimeUntilDestroy -= Time.deltaTime;

            if (modEvent is { TimeUntilHide: <= 0.0f })
            {
                modEvent.HintObject.GetComponent<Animator>()?.SetBool(Hide, true);
                // Prevent the hide animation from playing again
                modEvent.TimeUntilHide = 1e10f;
            }

            if (modEvent is { TimeUntilDestroy: <= 0.0f })
            {
                objectsToBeRemoved.Add(modEvent);
            }
        }

        foreach (var modEvent in objectsToBeRemoved)
        {
            EventObjects.Remove(modEvent);
            UnityEngine.Object.Destroy(modEvent.HintObject);
        }

        if (objectsToBeRemoved.Count > 0)
        {
            UpdatePositions();
        }
    }

    private static void EnsureObjectsSelected()
    {
        if (_hintScreenTemplate != null || _interfaceObject != null)
        {
            return;
        }

        GameController? gameController =
            UnityEngine.Object.FindObjectOfType<GameController>()
            ?? throw new NullReferenceException(
                "Tried finding hint screen but a GameController couldn't be found."
            );

        GameObject? interfaceObject = null;
        for (int i = 0; i < gameController.transform.childCount; ++i)
        {
            GameObject childGameObject = gameController.transform.GetChild(i).gameObject;
            if (childGameObject.name is "Interface")
            {
                interfaceObject = childGameObject;
            }
        }

        if (interfaceObject == null)
        {
            throw new NullReferenceException(
                "Tried finding hint screen but an interface couldn't be found in the GameController."
            );
        }

        _interfaceObject = interfaceObject;

        GameObject? hintScreenObject = null;
        for (int i = 0; i < interfaceObject.transform.childCount; ++i)
        {
            GameObject childGameObject = interfaceObject.transform.GetChild(i).gameObject;
            if (childGameObject.name is "HintScreen")
            {
                hintScreenObject = childGameObject;
            }
        }

        if (hintScreenObject == null)
        {
            throw new NullReferenceException(
                "Tried finding hint screen but a hint screen couldn't be found in the interface."
            );
        }

        _hintScreenTemplate = hintScreenObject;
    }
}
