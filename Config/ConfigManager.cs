namespace KappiMod.Config;

public static class ConfigManager
{
    internal static readonly Dictionary<string, IConfigElement> ConfigElements = new();

    public static ConfigHandler Handler { get; private set; } = null!;

    // Internal config elements
    public static ConfigElement<bool> DebugMode { get; private set; } = null!;
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
        if (Handler is not null)
        {
            throw new Exception($"{nameof(ConfigManager)} is already initialized");
        }

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
        DebugMode = new(nameof(DebugMode), "Debug mode", false);
        StartupDelayTime = new(nameof(StartupDelayTime), "Startup delay time", 0.0f);
        DisableEventSystemOverride = new(
            nameof(DisableEventSystemOverride),
            "Disable event system override",
            false
        );
        ForceUnlockMouse = new(nameof(ForceUnlockMouse), "Force unlock mouse", true);

        DialogueSkipper = new(nameof(DialogueSkipper), "Dialogue skipper", false);
        FlashlightIncreaser = new(nameof(FlashlightIncreaser), "Flashlight increaser", true);
        SitUnlocker = new(nameof(SitUnlocker), "Sit unlocker", true);
        SprintUnlocker = new(nameof(SprintUnlocker), "Sprint unlocker", true);
        TimeScaleScroller = new(nameof(TimeScaleScroller), "Time scale scroller", true);

        IntroSkipper = new(nameof(IntroSkipper), "Intro skipper", false);

        FpsLimit = new(nameof(FpsLimit), "Fps limit", -1);
    }
}
