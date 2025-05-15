namespace KappiMod.Patches.Core;

public interface IPatch : IDisposable
{
    string Id { get; }

    string Name { get; }

    string Description { get; }
}
