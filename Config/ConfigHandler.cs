namespace KappiMod.Config;

public abstract class ConfigHandler
{
    public abstract void Init();
    public abstract void LoadConfig();
    public abstract void SaveConfig();

    public virtual void OnAnyConfigChanged() { }

    public abstract void RegisterConfigElement<T>(ConfigElement<T> element);
    public abstract T GetConfigValue<T>(ConfigElement<T> element);
    public abstract void SetConfigValue<T>(ConfigElement<T> element, T value);
}
