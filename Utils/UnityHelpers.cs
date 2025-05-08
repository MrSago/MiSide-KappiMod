using UnityEngine;

namespace KappiMod.Utils;

public static class UnityHelpers
{
    public static bool IsValid(MonoBehaviour? obj) => obj != null && obj.gameObject != null;
}
