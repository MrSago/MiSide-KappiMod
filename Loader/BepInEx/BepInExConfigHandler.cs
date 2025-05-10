#if BIE

using BepInEx.Configuration;
using KappiMod.Config;
using KappiMod.Properties;
using KappiMod.Logging;

namespace KappiMod.Loader.BepInEx;

public class BepInExConfigHandler : ConfigHandler
{
    internal const string CFG_NAME = BuildInfo.NAME;

    internal ConfigFile Config => KappiModBepInExPlugin.Instance.Config;

    public override void Init() { }

    public override void LoadConfig()
    {
        foreach (KeyValuePair<string, IConfigElement> entry in ConfigManager.ConfigElements)
        {
            string key = entry.Key;
            ConfigDefinition def = new(CFG_NAME, key);
            if (Config.ContainsKey(def) && Config[def] is ConfigEntryBase configEntry)
            {
                IConfigElement config = entry.Value;
                config.BoxedValue = configEntry.BoxedValue;
            }
        }
    }

    public override void SaveConfig() => Config.Save();

    public override void RegisterConfigElement<T>(ConfigElement<T> config)
    {
        ConfigEntry<T> entry = Config.Bind(CFG_NAME, config.Name, config.Value, config.Description);
        entry.SettingChanged += (object? o, EventArgs e) =>
        {
            config.Value = entry.Value;
        };
    }

    public override T GetConfigValue<T>(ConfigElement<T> element)
    {
        if (Config.TryGetEntry(CFG_NAME, element.Name, out ConfigEntry<T> configEntry))
        {
            return configEntry.Value;
        }

        throw new Exception("Could not get config entry '" + element.Name + "'");
    }

    public override void SetConfigValue<T>(ConfigElement<T> element, T value)
    {
        if (Config.TryGetEntry(CFG_NAME, element.Name, out ConfigEntry<T> configEntry))
        {
            configEntry.Value = value;
            return;
        }

        KappiLogger.LogWarning("Could not get config entry '" + element.Name + "'");
    }
}

#endif // BIE
