using KappiMod.Config;
using KappiMod.Mods.Core;
using KappiMod.Patches.Core;
using KappiMod.Patches.Rng;
using KappiMod.Properties;

namespace KappiMod.Mods;

[ModInfo(
    name: "BlessRng Mod",
    description: "Removes RNG from the game",
    version: "1.1.0",
    author: BuildInfo.COMPANY
)]
public sealed class BlessRng : BaseMod
{
    public override bool IsEnabled
    {
        get => base.IsEnabled && ConfigManager.BlessRngMod.Value;
        protected set
        {
            base.IsEnabled = value;
            ConfigManager.BlessRngMod.Value = value;
        }
    }

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
        _patchManager.RegisterPatch<ChibiDoorUnlockerPatch>();
        _patchManager.RegisterPatch<ChipMiniGamePatch>();
        _patchManager.RegisterPatch<FixedItemSpawnPatch>();
        _patchManager.RegisterPatch<PassableDummiesPatch>();
        _patchManager.RegisterPatch<RingInstantReadyPatch>();
    }
}
