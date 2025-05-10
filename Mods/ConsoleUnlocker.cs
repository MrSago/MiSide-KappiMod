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
        KappiModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
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
        catch (Exception ex)
        {
            KappiModCore.LogError(ex.Message);
        }
    }

    private void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName is not ObjectNames.MAIN_MENU_SCENE)
        {
            return;
        }

        UnlockConsole();
    }
}
