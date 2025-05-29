using KappiMod.Logging;

namespace KappiMod.Utils;

public static class SceneTracker
{
    private static bool _isInitialized = false;
    private static string _currentSceneName = string.Empty;
    private static int _currentSceneBuildIndex = -1;

    public static string Name { get; private set; } = string.Empty;

    public static int BuildIndex { get; private set; } = -1;

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiLogger.LogError($"{nameof(SceneTracker)} is already initialized");
            return;
        }

        KappiCore.Loader.SceneWasLoaded += OnSceneWasLoaded;

        _isInitialized = true;
        KappiLogger.Log("Initialized");
    }

    private static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Name = _currentSceneName;
        BuildIndex = _currentSceneBuildIndex;

        _currentSceneName = sceneName;
        _currentSceneBuildIndex = buildIndex;
    }
}
