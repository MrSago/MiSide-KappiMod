namespace KappiMod.Utils;

public static class Helpers
{
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
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        return new string(
            Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()
        );
    }
}
