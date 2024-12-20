#if ML
using MelonLoader;
using MelonLoader.Utils;
using KappiMod.Config;

[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
[assembly: MelonInfo(
    typeof(KappiMod.Loader.MelonLoader.KappiModeMelonMod),
    KappiMod.Properties.BuildInfo.NAME,
    KappiMod.Properties.BuildInfo.VERSION,
    KappiMod.Properties.BuildInfo.AUTHOR,
    KappiMod.Properties.BuildInfo.DOWNLOADLINK
)]
[assembly: MelonOptionalDependencies("UniverseLib")]
[assembly: MelonGame("AIHASTO", "MiSideFull")]
[assembly: MelonColor(255, 196, 21, 169)]
[assembly: MelonAuthorColor(255, 33, 164, 176)]

namespace KappiMod.Loader.MelonLoader;

public class KappiModeMelonMod : MelonMod, IKappiModLoader
{
    public string KappiModFolderDestination => MelonEnvironment.ModsDirectory;
    public string UnhollowedModulesFolder =>
        Path.Combine(
            Path.GetDirectoryName(KappiModFolderDestination) ?? "",
            Path.Combine("MelonLoader", "Il2CppAssemblies")
        );

    private MelonLoaderConfigHandler _configHandler = null!;
    public ConfigHandler ConfigHandler => _configHandler;

    public event Action? Update;
    public event Action<int, string>? SceneWasLoaded;
    public event Action<int, string>? SceneWasInitialized;

    public Action<object> OnLogMessage => MelonLogger.Msg;
    public Action<object> OnLogWarning => MelonLogger.Warning;
    public Action<object> OnLogError => MelonLogger.Error;

    public override void OnUpdate() => Update?.Invoke();

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) =>
        SceneWasLoaded?.Invoke(buildIndex, sceneName);

    public override void OnSceneWasInitialized(int buildIndex, string sceneName) =>
        SceneWasInitialized?.Invoke(buildIndex, sceneName);

    public override void OnLateInitializeMelon()
    {
        _configHandler = new MelonLoaderConfigHandler();
        KappiModCore.Init(this);
    }
}

#endif
