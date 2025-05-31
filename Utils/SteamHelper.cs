using KappiMod.Logging;
using KappiMod.Mods.Core;
using KappiMod.Properties;
#if ML
using Il2CppSteamworks;
#elif BIE
using BepInEx.IL2CPP;
using Steamworks;
#endif

namespace KappiMod.Utils;

[ModInfo(
    name: "Steam Helper",
    description: "Provides access to Steam API functionalities",
    version: "1.0.0",
    author: BuildInfo.COMPANY
)]
public class SteamHelper : BaseMod
{
    public static SteamHelper? Instance { get; private set; }

    private const string STEAM_API_NOT_INITIALIZED =
        "Steam API is not initialized or Steam is not running";

    protected override void OnInitialize()
    {
        try
        {
            IsEnabled = SteamAPI.Init();
            if (!IsEnabled)
            {
                KappiLogger.LogError("Steam API failed to initialize");
                return;
            }

            Instance = this;
            KappiLogger.Log("Steam API initialized successfully");
        }
        catch (Exception ex)
        {
            KappiLogger.LogError($"Error initializing Steam API: {ex.Message}");
            IsEnabled = false;
        }
    }

    public ulong GetSteamID()
    {
        if (!IsEnabled || !SteamAPI.IsSteamRunning())
        {
            KappiLogger.LogWarning(STEAM_API_NOT_INITIALIZED);
            return 0;
        }

        return SteamUser.GetSteamID().m_SteamID;
    }

    public string GetPersonaName()
    {
        if (!IsEnabled || !SteamAPI.IsSteamRunning())
        {
            KappiLogger.LogWarning(STEAM_API_NOT_INITIALIZED);
            return "Unknown";
        }

        return SteamFriends.GetPersonaName();
    }

    public EPersonaState GetPersonaState()
    {
        if (!IsEnabled || !SteamAPI.IsSteamRunning())
        {
            KappiLogger.LogWarning(STEAM_API_NOT_INITIALIZED);
            return EPersonaState.k_EPersonaStateOffline;
        }

        return SteamFriends.GetPersonaState();
    }

    public bool IsUserLoggedIn()
    {
        if (!IsEnabled || !SteamAPI.IsSteamRunning())
        {
            KappiLogger.LogWarning(STEAM_API_NOT_INITIALIZED);
            return false;
        }

        return SteamUser.BLoggedOn();
    }
}
