#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods;

public static class ConsoleUnlocker
{
    public static void Init()
    {
        KappiModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;

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
