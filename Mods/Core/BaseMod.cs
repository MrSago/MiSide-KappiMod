using System.Reflection;
using KappiMod.Logging;

namespace KappiMod.Mods.Core;

public abstract class BaseMod
{
    public bool IsInitialized { get; private set; } = false;

    private string _id = string.Empty;
    public virtual string Id => _id;

    private string _name = string.Empty;
    public virtual string Name => _name;

    private string _description = string.Empty;
    public virtual string Description => _description;

    private string _version = string.Empty;
    public virtual string Version => _version;

    private string _author = string.Empty;
    public virtual string Author => _author;

    public virtual bool IsEnabled { get; protected set; } = false;

    public void Initialize()
    {
        if (IsInitialized)
        {
            KappiLogger.LogWarning("Mod is already initialized", Id);
            return;
        }

        try
        {
            InitializeAttribute();
            OnInitialize();
            IsInitialized = true;
            KappiLogger.Log("Mod initialized successfully", Id);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to initialize mod", Id, ex);
        }
    }

    public virtual void Enable()
    {
        if (!IsInitialized)
        {
            KappiLogger.LogWarning("Cannot enable: not initialized", Id);
            return;
        }

        if (IsEnabled)
        {
            KappiLogger.LogWarning("Is already enabled", Id);
            return;
        }

        try
        {
            OnEnable();
            IsEnabled = true;
            KappiLogger.Log("Enabled successfully", Id);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to enable mod", Id, ex);
        }
    }

    public virtual void Disable()
    {
        if (!IsInitialized)
        {
            KappiLogger.LogWarning("Cannot disable: not initialized", Id);
            return;
        }

        if (!IsEnabled)
        {
            KappiLogger.LogWarning("Is already disabled", Id);
            return;
        }

        try
        {
            OnDisable();
            IsEnabled = false;
            KappiLogger.Log("Disabled successfully", Id);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to disable mod", Id, ex);
        }
    }

    protected abstract void OnInitialize();

    protected virtual void OnEnable() { }

    protected virtual void OnDisable() { }

    private void InitializeAttribute()
    {
        var type = GetType();
        var modInfoAttribute = type.GetCustomAttribute<ModInfoAttribute>();
        if (modInfoAttribute is not null)
        {
            _id = type.DeclaringType?.Name ?? type.Name;
            _name = modInfoAttribute.Name;
            _description = modInfoAttribute.Description;
            _version = modInfoAttribute.Version;
            _author = modInfoAttribute.Author;
        }
    }
}
