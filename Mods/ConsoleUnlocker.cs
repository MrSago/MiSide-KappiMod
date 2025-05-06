#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods;

public static class ConsoleUnlocker
{
    private static bool _isInitialized = false;

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiModCore.LogError($"{nameof(ConsoleUnlocker)} is already initialized");
            return;
        }

        KappiModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;

        _isInitialized = true;
        KappiModCore.Log("Initialized");
    }

    public static void UnlockConsole()
    {
        try
        {
            if (ConsoleMain.liteVersion)
            {
                ConsoleMain.liteVersion = false;

                KappiModCore.Log("Console successfully unlocked!");
            }
            else
            {
                KappiModCore.Log("Console is already unlocked!");
            }
        }
        catch (Exception e)
        {
            KappiModCore.LogError(e.Message);
        }
    }

    private static void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is not ObjectNames.MAIN_MENU_SCENE)
        {
            return;
        }

        UnlockConsole();
    }
}
