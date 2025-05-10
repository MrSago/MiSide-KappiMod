using System.Reflection;
using UnityEngine;

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
            KappiModCore.Log($"[{Id}] Mod is already initialized", LogType.Warning);
            return;
        }

        try
        {
            InitializeAttribute();
            OnInitialize();
            IsInitialized = true;
            KappiModCore.Log($"[{Id}] Mod initialized successfully", LogType.Log);
        }
        catch (Exception ex)
        {
            KappiModCore.Log($"[{Id}] Failed to initialize: {ex.Message}", LogType.Error);
        }
    }

    public virtual void Enable()
    {
        if (!IsInitialized)
        {
            KappiModCore.Log($"[{Id}] Cannot enable: not initialized", LogType.Error);
            return;
        }

        if (IsEnabled)
        {
            KappiModCore.Log($"[{Id}] is already enabled", LogType.Warning);
            return;
        }

        try
        {
            OnEnable();
            IsEnabled = true;
            KappiModCore.Log($"[{Id}] Enabled successfully", LogType.Log);
        }
        catch (Exception ex)
        {
            KappiModCore.Log($"[{Id}] Failed to enable: {ex.Message}", LogType.Error);
        }
    }

    public virtual void Disable()
    {
        if (!IsInitialized)
        {
            KappiModCore.Log($"[{Id}] Cannot disable: not initialized", LogType.Error);
            return;
        }

        if (!IsEnabled)
        {
            KappiModCore.Log($"[{Id}] is already disabled", LogType.Warning);
            return;
        }

        try
        {
            OnDisable();
            IsEnabled = false;
            KappiModCore.Log($"[{Id}] Disabled successfully", LogType.Log);
        }
        catch (Exception ex)
        {
            KappiModCore.Log($"[{Id}] Failed to disable: {ex.Message}", LogType.Error);
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
