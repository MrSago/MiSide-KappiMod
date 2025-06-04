using KappiMod.Logging;

namespace KappiMod.Patches.Core;

public sealed class PatchManager : IDisposable
{
    private readonly List<IPatch> _patches = new();

    public void RegisterPatch<T>(T patch)
        where T : IPatch
    {
        if (patch is null)
        {
            KappiLogger.LogWarning("Attempted to register null patch");
            return;
        }

        _patches.Add(patch);
        KappiLogger.Log($"Registered patch: {patch.GetType().Name}");
    }

    public void RegisterPatch<T>()
        where T : IPatch, new()
    {
        try
        {
            var patch = new T();
            RegisterPatch(patch);
        }
        catch (Exception ex)
        {
            KappiLogger.LogException(
                $"Failed to create patch of type {typeof(T).Name}",
                exception: ex
            );
        }
    }

    public void Dispose()
    {
        foreach (var patch in _patches)
        {
            try
            {
                patch.Dispose();
                KappiLogger.Log($"Disposed patch: {patch.GetType().Name}");
            }
            catch (Exception ex)
            {
                KappiLogger.LogException(
                    $"Failed to dispose patch {patch.GetType().Name}",
                    exception: ex
                );
            }
        }

        _patches.Clear();
    }
}
