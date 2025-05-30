using KappiMod.Logging;
using KappiMod.Properties;

namespace KappiMod.Utils;

internal static class VersionChecker
{
    private const string GitHubApiUrl =
        "https://api.github.com/repos/MrSago/MiSide-KappiMod/releases/latest";

    internal static bool IsCheckingVersion { get; private set; } = false;
    internal static bool UpdateAvailable { get; private set; } = false;
    internal static string LatestVersion { get; private set; } = string.Empty;
    internal static string CurrentVersion => BuildInfo.VERSION;
    internal static string DownloadUrl => BuildInfo.DOWNLOADLINK + "/releases/latest";

    internal static async void CheckForUpdatesAsync()
    {
        if (IsCheckingVersion || !string.IsNullOrEmpty(LatestVersion))
        {
            return;
        }

        IsCheckingVersion = true;

        try
        {
            string latestVersion = await GetLatestVersionAsync();
            if (string.IsNullOrEmpty(latestVersion))
            {
                return;
            }

            LatestVersion = latestVersion.TrimStart('v');
            UpdateAvailable = IsNewerVersion(LatestVersion, CurrentVersion);

            KappiLogger.Log($"Current version: {CurrentVersion}");
            KappiLogger.Log($"Latest version: {LatestVersion}");

            if (UpdateAvailable)
            {
                KappiLogger.Log($"Update available! Download it here: {DownloadUrl}");
            }
            else
            {
                KappiLogger.Log("You are using the latest version");
            }
        }
        catch (Exception ex)
        {
            KappiLogger.LogException("Failed to check for updates", exception: ex);
        }
        finally
        {
            IsCheckingVersion = false;
        }
    }

    private static async Task<string> GetLatestVersionAsync()
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("User-Agent", $"{BuildInfo.NAME}/{BuildInfo.VERSION}");

        string response = await client.GetStringAsync(GitHubApiUrl);

        int tagNameStart = response.IndexOf("\"tag_name\":\"") + "\"tag_name\":\"".Length;
        if (tagNameStart <= 0)
        {
            return string.Empty;
        }

        int tagNameEnd = response.IndexOf("\"", tagNameStart);
        if (tagNameEnd <= tagNameStart)
        {
            return string.Empty;
        }

        return response[tagNameStart..tagNameEnd];
    }

    private static bool IsNewerVersion(string latestVersion, string currentVersion)
    {
        if (string.IsNullOrEmpty(latestVersion) || string.IsNullOrEmpty(currentVersion))
        {
            return false;
        }

        try
        {
            string[] latest = latestVersion.Split('.');
            string[] current = currentVersion.Split('.');

            // Compare major version
            int latestMajor = latest.Length > 0 ? int.Parse(latest[0]) : 0;
            int currentMajor = current.Length > 0 ? int.Parse(current[0]) : 0;
            if (latestMajor > currentMajor)
                return true;
            if (latestMajor < currentMajor)
                return false;

            // Compare minor version
            int latestMinor = latest.Length > 1 ? int.Parse(latest[1]) : 0;
            int currentMinor = current.Length > 1 ? int.Parse(current[1]) : 0;
            if (latestMinor > currentMinor)
                return true;
            if (latestMinor < currentMinor)
                return false;

            // Compare patch version
            int latestPatch = latest.Length > 2 ? int.Parse(latest[2]) : 0;
            int currentPatch = current.Length > 2 ? int.Parse(current[2]) : 0;
            if (latestPatch > currentPatch)
                return true;

            return false;
        }
        catch (Exception ex)
        {
            KappiLogger.LogException(
                $"Error comparing versions: {latestVersion} and {currentVersion}",
                exception: ex
            );
            return false;
        }
    }
}
