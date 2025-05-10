namespace KappiMod.Mods.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ModInfoAttribute : Attribute
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Version { get; private set; }
    public string Author { get; private set; }

    public ModInfoAttribute(
        string name,
        string? description = null,
        string? version = null,
        string? author = null
    )
    {
        Name = name;
        Description = description ?? "No description";
        Version = version ?? "1.0.0";
        Author = author ?? "Unknown";
    }
}
