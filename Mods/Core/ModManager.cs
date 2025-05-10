using System.Reflection;

namespace KappiMod.Mods.Core;

public static class ModManager
{
    private static readonly Dictionary<string, BaseMod> _registeredMods = new();
    private static readonly List<Type> _modTypes = new();

    public static IReadOnlyDictionary<string, BaseMod> RegisteredMods => _registeredMods;

    public static void Initialize()
    {
        KappiModCore.Log("Initializing ModManager...");
        DiscoverMods();
        InstantiateMods();
        InitializeMods();
        KappiModCore.Log($"ModManager initialized with {_registeredMods.Count} mods");
    }

    private static void DiscoverMods()
    {
        try
        {
            KappiModCore.Log("Discovering mods...");
            Assembly? assembly = Assembly.GetExecutingAssembly();

            var modTypes = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && t.IsClass && typeof(BaseMod).IsAssignableFrom(t))
                .ToList();

            _modTypes.AddRange(modTypes);
            KappiModCore.Log($"Discovered {modTypes.Count} mod types");
        }
        catch (Exception ex)
        {
            KappiModCore.LogError($"Error discovering mods: {ex.Message}");
        }
    }

    private static void InstantiateMods()
    {
        KappiModCore.Log("Instantiating mods...");

        foreach (var modType in _modTypes)
        {
            try
            {
                object? modInstance = Activator.CreateInstance(modType);
                if (modInstance is not BaseMod mod)
                {
                    KappiModCore.LogError(
                        $"Failed to create an instance of {modType.Name}."
                            + $" The instance is null or not of type {nameof(BaseMod)}."
                    );
                    continue;
                }

                if (_registeredMods.ContainsKey(modType.Name))
                {
                    KappiModCore.LogError(
                        $"Mod with ID {modType.Name} is already registered. Skipping registration."
                    );
                    continue;
                }

                _registeredMods[modType.Name] = mod;
                KappiModCore.Log($"Registered mod: {modType.Name}");
            }
            catch (Exception ex)
            {
                KappiModCore.LogError($"Failed to instantiate mod {modType.Name}: {ex.Message}");
            }
        }
    }

    private static void InitializeMods()
    {
        KappiModCore.Log("Initializing mods...");

        foreach (var mod in _registeredMods.Values)
        {
            try
            {
                mod.Initialize();
            }
            catch (Exception ex)
            {
                KappiModCore.LogError($"Failed to initialize mod {mod.Name}: {ex.Message}");
            }
        }
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
}
