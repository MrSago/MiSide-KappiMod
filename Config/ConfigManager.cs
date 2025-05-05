namespace KappiMod.Config;

public static class ConfigManager
{
    internal static readonly Dictionary<string, IConfigElement> ConfigElements = new();

    public static ConfigHandler Handler { get; private set; } = null!;

    // Internal config elements
    public static ConfigElement<float> StartupDelayTime { get; private set; } = null!;
    public static ConfigElement<bool> DisableEventSystemOverride { get; private set; } = null!;
    public static ConfigElement<bool> ForceUnlockMouse { get; private set; } = null!;

    // Mod config elements
    public static ConfigElement<bool> DialogueSkipper { get; private set; } = null!;
    public static ConfigElement<bool> FlashlightIncreaser { get; private set; } = null!;
    public static ConfigElement<bool> SitUnlocker { get; private set; } = null!;
    public static ConfigElement<bool> SprintUnlocker { get; private set; } = null!;
    public static ConfigElement<bool> TimeScaleScroller { get; private set; } = null!;

    // Patch config elements
    public static ConfigElement<bool> IntroSkipper { get; private set; } = null!;

    // Mod settings elements
    public static ConfigElement<int> FpsLimit { get; private set; } = null!;

    public static void Init(ConfigHandler handler)
    {
        Handler = handler;
        Handler.Init();

        CreateConfigElements();

        Handler.LoadConfig();
    }

    internal static void RegisterConfigElement<T>(ConfigElement<T> element)
    {
        Handler.RegisterConfigElement(element);
        ConfigElements.Add(element.Name, element);
    }

    private static void CreateConfigElements()
    {
        StartupDelayTime = new("StartupDelayTime", "Startup delay time", 0.0f);
        DisableEventSystemOverride = new(
            "DisableEventSystemOverride",
            "Disable event system override",
            false
        );
        ForceUnlockMouse = new("ForceUnlockMouse", "Force unlock mouse", true);

        DialogueSkipper = new("DialogueSkipper", "Dialogue skipper", false);
        FlashlightIncreaser = new("FlashlightIncreaser", "Flashlight increaser", true);
        SitUnlocker = new("SitUnlocker", "Sit unlocker", true);
        SprintUnlocker = new("SprintUnlocker", "Sprint unlocker", true);
        TimeScaleScroller = new("TimeScaleScroller", "Time scale scroller", true);

        IntroSkipper = new("IntroSkipper", "Intro skipper", false);

        FpsLimit = new("FpsLimit", "Fps limit", -1);
    }
}
