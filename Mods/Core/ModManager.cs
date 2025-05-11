using System.Reflection;
using KappiMod.Logging;

namespace KappiMod.Mods.Core;

public static class ModManager
{
    private static readonly List<Type> _modTypes = new();

    private static readonly Dictionary<string, BaseMod> _registeredMods = new();
    public static IReadOnlyDictionary<string, BaseMod> RegisteredMods => _registeredMods;

    public static void Init()
    {
        KappiLogger.Log("Initializing ModManager...");
        DiscoverMods();
        InstantiateMods();
        InitializeMods();
        KappiLogger.Log($"ModManager initialized with {_registeredMods.Count} mods");
    }

    public static BaseMod? GetMod(string id)
    {
        if (_registeredMods.TryGetValue(id, out var mod))
        {
            return mod;
        }

        return null;
    }

    public static T? GetMod<T>()
        where T : BaseMod
    {
        return _registeredMods.Values.OfType<T>().FirstOrDefault();
    }

    public static void EnableAllMods()
    {
        var disabledMods = _registeredMods.Values.Where(m => !m.IsEnabled);
        foreach (var mod in disabledMods)
        {
            mod.Enable();
        }
    }

    public static void DisableAllMods()
    {
        var enabledMods = _registeredMods.Values.Where(m => m.IsEnabled);
        foreach (var mod in enabledMods)
        {
            mod.Disable();
        }
    }

    private static void DiscoverMods()
    {
        try
        {
            KappiLogger.Log("Discovering mods...");

            int initialCount = _modTypes.Count;

            var newModTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && t.IsClass && typeof(BaseMod).IsAssignableFrom(t))
                .Except(_modTypes);

            _modTypes.AddRange(newModTypes);
            int addedCount = _modTypes.Count - initialCount;
            KappiLogger.Log($"Discovered {addedCount} new mod types");
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Error discovering mods", exception: ex);
        }
    }

    private static void InstantiateMods()
    {
        KappiLogger.Log("Instantiating mods...");

        foreach (var modType in _modTypes)
        {
            try
            {
                if (_registeredMods.ContainsKey(modType.Name))
                {
                    KappiLogger.LogError(
                        $"Mod with ID {modType.Name} is already registered. Skipping registration."
                    );
                    continue;
                }

                object? modInstance = Activator.CreateInstance(modType);
                if (modInstance is not BaseMod mod)
                {
                    KappiLogger.LogError(
                        $"Failed to create an instance of {modType.Name}."
                            + $" The instance is null or not of type {nameof(BaseMod)}."
                    );
                    continue;
                }

                _registeredMods[modType.Name] = mod;
                KappiLogger.Log($"Registered mod: {modType.Name}");
            }
            catch (Exception ex)
            {
                KappiLogger.LogException(
                    $"Failed to instantiate mod: {modType.Name}",
                    exception: ex
                );
            }
        }
    }

    private static void InitializeMods()
    {
        KappiLogger.Log("Initializing mods...");

        var notInitializedMods = _registeredMods.Values.Where(m => !m.IsInitialized);
        foreach (var mod in notInitializedMods)
        {
            try
            {
                mod.Initialize();
            }
            catch (Exception ex)
            {
                KappiLogger.LogException($"Failed to initialize mod: {mod.Name}", exception: ex);
            }
        }
    }
}
