#if ML

using MelonLoader;
using KappiMod.Config;
using System.Reflection;

namespace KappiMod.Loader.MelonLoader;

public class MelonLoaderConfigHandler : ConfigHandler
{
    internal const string CFG_NAME = Properties.BuildInfo.NAME;

    internal MelonPreferences_Category PrefCategory = null!;

    public override void Init()
    {
        PrefCategory = MelonPreferences.CreateCategory(CFG_NAME, $"{CFG_NAME} Configuration");
    }

    public override void LoadConfig()
    {
        foreach (var element in ConfigManager.ConfigElements)
        {
            string? key = element.Key;
            if (PrefCategory.GetEntry(key) is not null)
            {
                IConfigElement? config = element.Value;
                config.BoxedValue = config.GetLoaderConfigValue();
            }
        }
    }

    public override void SaveConfig() => MelonPreferences.Save();

    public override void OnAnyConfigChanged() => MelonPreferences.Save();

    public override void RegisterConfigElement<T>(ConfigElement<T> element)
    {
        MelonPreferences_Entry<T> entry = PrefCategory.CreateEntry(
            element.Name,
            element.Value,
            null,
            element.Description,
            false,
            false
        );
        _ = new EntryDelegateWrapper<T>(entry, element);
    }

    public override T GetConfigValue<T>(ConfigElement<T> element)
    {
        if (PrefCategory.GetEntry(element.Name) is MelonPreferences_Entry<T> entry)
        {
            return entry.Value;
        }
        return default!;
    }

    public override void SetConfigValue<T>(ConfigElement<T> element, T value)
    {
        if (PrefCategory.GetEntry(element.Name) is MelonPreferences_Entry<T> entry)
        {
            entry.Value = value;
        }
    }

    private class EntryDelegateWrapper<T>
    {
        public MelonPreferences_Entry<T> entry;
        public ConfigElement<T> config;

        public EntryDelegateWrapper(MelonPreferences_Entry<T> entry, ConfigElement<T> config)
        {
            this.entry = entry;
            this.config = config;

            EventInfo? evt = entry.GetType().GetEvent("OnValueChangedUntyped");
            MethodInfo? thisMethod = GetType().GetMethod("OnChanged");
            if (evt is null or { EventHandlerType: null } || thisMethod is null)
            {
                return;
            }

            evt.AddEventHandler(
                entry,
                Delegate.CreateDelegate(evt.EventHandlerType, this, thisMethod)
            );
        }

        public void OnChanged()
        {
            if (
                entry is { Value: null }
                || config is { Value: null }
                || config.Value.Equals(entry.Value)
            )
            {
                return;
            }
            config.Value = entry.Value;
        }
    }
}

#endif // ML
