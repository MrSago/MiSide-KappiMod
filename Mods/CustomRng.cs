using KappiMod.Mods.Core;
using KappiMod.Patches.Core;
using KappiMod.Patches.Rng;
using KappiMod.Properties;

namespace KappiMod.Mods;

[ModInfo(
    name: "Custom Rng Mod",
    description: "A mod that allows you to customize the RNG behavior in the game",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public sealed class CustomRng : BaseMod
{
    private readonly PatchManager _patchManager = new();

    protected override void OnEnable()
    {
        OnDisable();
        RegisterPatches();
    }

    protected override void OnDisable()
    {
        _patchManager.Dispose();
    }

    private void RegisterPatches()
    {
        _patchManager.RegisterPatch(new DeterministicRandomPatch(disabledRandom: true));
    }
}
