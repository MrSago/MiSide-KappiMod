namespace KappiMod.Utils;

public static class PreviousSceneTracker
{
    private static bool _isInitialized = false;
    private static string _currentSceneName = string.Empty;
    private static int _currentSceneBuildIndex = -1;

    private static string _previousSceneName = string.Empty;
    public static string Name => _previousSceneName;

    private static int _previousSceneBuildIndex = -1;
    public static int BuildIndex => _previousSceneBuildIndex;

    public static void Init()
    {
        if (_isInitialized)
        {
            KappiModCore.LogError($"{nameof(PreviousSceneTracker)} is already initialized");
            return;
        }

        KappiModCore.Loader.SceneWasLoaded += OnSceneWasLoaded;

        _isInitialized = true;
        KappiModCore.Log("Initialized");
    }

    private static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        _previousSceneName = _currentSceneName;
        _previousSceneBuildIndex = _currentSceneBuildIndex;

        _currentSceneName = sceneName;
        _currentSceneBuildIndex = buildIndex;
    }
}
