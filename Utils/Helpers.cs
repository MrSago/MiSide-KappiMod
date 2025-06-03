using KappiMod.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KappiMod.Utils;

public static class Helpers
{
    public static Transform? GetRootTransform()
    {
        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name is SceneName.WORLD_ROOT)
            {
                return root.transform;
            }
        }

        return null;
    }

    public static string GenerateRandomString(int length = 8, int? seed = null)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than 0", nameof(length));
        }

        if (seed.HasValue && seed.Value < 0)
        {
            throw new ArgumentException("Seed must be a non-negative integer", nameof(seed));
        }

        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Random random = seed.HasValue ? new(seed.Value) : new();
        return new(
            Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()
        );
    }
}
