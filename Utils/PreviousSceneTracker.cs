namespace KappiMod.Utils;

public static class PreviousSceneTracker
{
    private static string _previousSceneName = string.Empty;
    public static string Name
    {
        get => _previousSceneName;
        private set => _previousSceneName = value;
    }

    private static int _previousSceneBuildIndex = -1;
    public static int BuildIndex
    {
        get => _previousSceneBuildIndex;
        private set => _previousSceneBuildIndex = value;
    }

    private static string _currentSceneName = string.Empty;
    private static int _currentSceneBuildIndex = -1;

    public static void Init()
    {
        KappiModCore.Loader.SceneWasLoaded += OnSceneWasLoaded;
    }

    private static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        _previousSceneName = _currentSceneName;
        _previousSceneBuildIndex = _currentSceneBuildIndex;
        _currentSceneName = sceneName;
        _currentSceneBuildIndex = buildIndex;
    }
}
