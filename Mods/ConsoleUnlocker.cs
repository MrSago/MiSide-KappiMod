using KappiMod.Constants;
using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Properties;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace KappiMod.Mods;

[ModInfo(
    name: "Console Unlocker",
    description: "Unlocks the developer console in the game",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public sealed class ConsoleUnlocker : BaseMod
{
    public override bool IsEnabled => true;

    protected override void OnInitialize()
    {
        KappiCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
    }

    public static void UnlockConsole()
    {
        try
        {
            if (ConsoleMain.liteVersion)
            {
                ConsoleMain.liteVersion = false;
                KappiLogger.Log("Console successfully unlocked!");
            }
            else
            {
                KappiLogger.Log("Console is already unlocked!");
            }
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to unlock console", exception: ex);
        }
    }

    private void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is not SceneName.MAIN_MENU)
        {
            return;
        }

        UnlockConsole();
    }
}
