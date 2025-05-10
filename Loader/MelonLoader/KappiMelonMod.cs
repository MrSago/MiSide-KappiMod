#if ML

using MelonLoader;
using MelonLoader.Utils;
using KappiMod.Config;

[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
[assembly: MelonInfo(
    typeof(KappiMod.Loader.MelonLoader.KappiMelonMod),
    KappiMod.Properties.BuildInfo.NAME,
    KappiMod.Properties.BuildInfo.VERSION,
    KappiMod.Properties.BuildInfo.AUTHOR,
    KappiMod.Properties.BuildInfo.DOWNLOADLINK
)]
[assembly: MelonOptionalDependencies("UniverseLib")]
[assembly: MelonGame("AIHASTO", "MiSideFull")]
[assembly: MelonColor(255, 196, 22, 169)]
[assembly: MelonAuthorColor(255, 120, 60, 190)]

namespace KappiMod.Loader.MelonLoader;

public class KappiMelonMod : MelonMod, IKappiModLoader
{
    public string KappiModDirectoryDestination => MelonEnvironment.ModsDirectory;
    public string UnhollowedModulesDirectory =>
        Path.Combine(
            Path.GetDirectoryName(KappiModDirectoryDestination) ?? "",
            Path.Combine("MelonLoader", "Il2CppAssemblies")
        );

    private MelonLoaderConfigHandler _configHandler = null!;
    public ConfigHandler ConfigHandler => _configHandler;

    public Action<object> OnLogMessage => MelonLogger.Msg;
    public Action<object> OnLogWarning => MelonLogger.Warning;
    public Action<object> OnLogError => MelonLogger.Error;

    public event Action? Update;
    public event Action<int, string>? SceneWasLoaded;
    public event Action<int, string>? SceneWasInitialized;

    public override void OnUpdate() => Update?.Invoke();

    public override void OnSceneWasLoaded(int buildIndex, string sceneName) =>
        SceneWasLoaded?.Invoke(buildIndex, sceneName);

    public override void OnSceneWasInitialized(int buildIndex, string sceneName) =>
        SceneWasInitialized?.Invoke(buildIndex, sceneName);

    public override void OnLateInitializeMelon()
    {
        _configHandler = new MelonLoaderConfigHandler();
        KappiCore.Init(this);
    }
}

#endif // ML
