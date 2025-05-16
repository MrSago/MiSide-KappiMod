using KappiMod.Config;
using KappiMod.Mods.Core;
using KappiMod.Patches.Core;
using KappiMod.Patches.RngRemovals;
using KappiMod.Properties;

namespace KappiMod.Mods;

[ModInfo(
    name: "BlessRng Mod",
    description: "Removes RNG from the game",
    version: "1.0.0",
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

    protected override void OnInitialize()
    {
        if (ConfigManager.BlessRngMod.Value)
        {
            OnEnable();
            base.IsEnabled = true;
        }
    }

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
        _patchManager.RegisterPatch<ChipMiniGamePatch>();
        _patchManager.RegisterPatch<FixedItemSpawnPatch>();
        _patchManager.RegisterPatch<NoChibiDoorUnlockerPatch>();
        _patchManager.RegisterPatch<RingInstantReadyPatch>();
        _patchManager.RegisterPatch<GoodManekenPatch>();
    }
}
