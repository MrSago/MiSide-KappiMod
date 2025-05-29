#if BIE

using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using KappiMod.Config;
using KappiMod.Properties;
using UnityEngine;

namespace KappiMod.Loader.BepInEx;

[BepInPlugin(BuildInfo.PACKAGE, BuildInfo.NAME, BuildInfo.VERSION)]
public class KappiBepInExPlugin : BasePlugin, IKappiModLoader
{
    public static KappiBepInExPlugin Instance = null!;

    private static readonly Harmony _harmony = new(BuildInfo.GUID);

    public string KappiModDirectoryDestination { get; } =
        Path.Combine(Paths.PluginPath, KappiCore.MOD_DIRECTORY_NAME);
    public string UnhollowedModulesDirectory { get; } =
        Path.Combine(Paths.BepInExRootPath, "interop");

    public ConfigHandler ConfigHandler { get; private set; } = null!;

    public Action<object> OnLogMessage => Log.LogMessage;
    public Action<object> OnLogWarning => Log.LogWarning;
    public Action<object> OnLogError => Log.LogError;

    public event Action? Update;
    public event Action<int, string>? SceneWasLoaded;
    public event Action<int, string>? SceneWasInitialized;

    public void OnUpdate() => Update?.Invoke();

    public void OnSceneWasLoaded(int buildIndex, string sceneName) =>
        SceneWasLoaded?.Invoke(buildIndex, sceneName);

    public void OnSceneWasInitialized(int buildIndex, string sceneName) =>
        SceneWasInitialized?.Invoke(buildIndex, sceneName);

    public override void Load()
    {
        Instance = this;
        ConfigHandler = new BepInExConfigHandler();
        _harmony.PatchAll(typeof(BepInExPatches));
        IL2CPPChainloader.AddUnityComponent(typeof(KappiModBepInExEventProxy));
        KappiCore.Init(this);
    }

    // Need to use unique class name to avoid conflicts with other mods
    private class KappiModBepInExEventProxy : MonoBehaviour
    {
        private void Update() => Instance?.OnUpdate();
    }
}

#endif // BIE
