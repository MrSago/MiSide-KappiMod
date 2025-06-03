using HarmonyLib;
using KappiMod.Patches.Core;
using UnityEngine;

namespace KappiMod.Patches.Rng;

[HarmonyPatch]
public sealed class DeterministicRandomPatch : IPatch
{
    public string Id => "com.kappimod.deterministicrandom";
    public string Name => "Deterministic Random";
    public string Description => "Forces Unity's Random class to return deterministic values";

    private static int _forcedSeed = 12345;
    public static int ForcedSeed
    {
        get => _forcedSeed;
        set
        {
            _forcedSeed = value;
            _random = new System.Random(value);
            UnityEngine.Random.InitState(value);
        }
    }

    private static bool _disabledRandom = false;
    public static bool DisabledRandom
    {
        get => _disabledRandom;
        set => _disabledRandom = value;
    }

    private static System.Random _random = new(_forcedSeed);

    private readonly HarmonyLib.Harmony _harmony;

    public DeterministicRandomPatch(int forcedSeed = 12345, bool disabledRandom = false)
    {
        _forcedSeed = forcedSeed;
        _disabledRandom = disabledRandom;

        _random = new System.Random(_forcedSeed);
        UnityEngine.Random.InitState(_forcedSeed);

        _harmony = new(Id);
        _harmony.PatchAll(typeof(DeterministicRandomPatch));
    }

    public void Dispose()
    {
        _harmony.UnpatchSelf();
    }

    #region Getters Patches

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.value), MethodType.Getter)]
    private static bool Value(ref float __result)
    {
        if (_disabledRandom)
        {
            __result = 0.0f;
            return false;
        }

        __result = (float)_random.NextDouble();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.insideUnitSphere),
        MethodType.Getter
    )]
    private static bool InsideUnitSphere(ref Vector3 __result)
    {
        if (_disabledRandom)
        {
            __result = Vector3.zero;
            return false;
        }

        float phi = 2.0f * Mathf.PI * (float)_random.NextDouble();
        float cosTheta = 2.0f * (float)_random.NextDouble() - 1.0f;
        float radius = Mathf.Pow((float)_random.NextDouble(), 1.0f / 3.0f);

        float sinTheta = Mathf.Sqrt(1.0f - cosTheta * cosTheta);

        float x = radius * sinTheta * Mathf.Cos(phi);
        float y = radius * sinTheta * Mathf.Sin(phi);
        float z = radius * cosTheta;

        __result = new Vector3(x, y, z);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.insideUnitCircle),
        MethodType.Getter
    )]
    private static bool InsideUnitCircle(ref Vector2 __result)
    {
        if (_disabledRandom)
        {
            __result = Vector2.zero;
            return false;
        }

        float angle = 2.0f * Mathf.PI * (float)_random.NextDouble();
        float radius = Mathf.Sqrt((float)_random.NextDouble());

        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        __result = new Vector2(x, y);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.onUnitSphere),
        MethodType.Getter
    )]
    private static bool OnUnitSphere(ref Vector3 __result)
    {
        if (_disabledRandom)
        {
            __result = Vector3.zero;
            return false;
        }

        float u1 = (float)_random.NextDouble();
        float u2 = (float)_random.NextDouble();
        float sqrtU1 = Mathf.Sqrt(u1);
        float phi = 2.0f * Mathf.PI * u2;

        float x = sqrtU1 * Mathf.Cos(phi);
        float y = sqrtU1 * Mathf.Sin(phi);
        float z = Mathf.Sqrt(1.0f - u1);

        __result = new Vector3(x, y, z);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.rotation),
        MethodType.Getter
    )]
    private static bool Rotation(ref Quaternion __result)
    {
        if (_disabledRandom)
        {
            __result = Quaternion.identity;
            return false;
        }

        float u1 = (float)_random.NextDouble();
        float u2 = (float)_random.NextDouble();
        float u3 = (float)_random.NextDouble();

        float sqrt1MinusU1 = Mathf.Sqrt(1.0f - u1);
        float sqrtU1 = Mathf.Sqrt(u1);

        float twoPI_U2 = 2.0f * Mathf.PI * u2;
        float twoPI_U3 = 2.0f * Mathf.PI * u3;

        float x = sqrt1MinusU1 * Mathf.Sin(twoPI_U2);
        float y = sqrt1MinusU1 * Mathf.Cos(twoPI_U2);
        float z = sqrtU1 * Mathf.Sin(twoPI_U3);
        float w = sqrtU1 * Mathf.Cos(twoPI_U3);

        __result = new Quaternion(x, y, z, w);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.rotationUniform),
        MethodType.Getter
    )]
    private static bool RotationUniform(ref Quaternion __result)
    {
        return Rotation(ref __result);
    }

    #endregion Getters Patches

    #region Methods Patches

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.Range),
        new[] { typeof(float), typeof(float) }
    )]
    private static bool RangeFloat(float minInclusive, float maxInclusive, ref float __result)
    {
        if (_disabledRandom)
        {
            __result = minInclusive;
            return false;
        }

        __result = minInclusive + (float)_random.NextDouble() * (maxInclusive - minInclusive);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(
        typeof(UnityEngine.Random),
        nameof(UnityEngine.Random.Range),
        new[] { typeof(int), typeof(int) }
    )]
    private static bool RangeInt(int minInclusive, int maxExclusive, ref int __result)
    {
        if (_disabledRandom)
        {
            __result = minInclusive;
            return false;
        }

        __result = _random.Next(minInclusive, maxExclusive);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(UnityEngine.Random), nameof(UnityEngine.Random.GetRandomUnitCircle))]
    private static void GetRandomUnitCircle(out Vector2 output)
    {
        if (_disabledRandom)
        {
            output = Vector2.zero;
            return;
        }

        float angle = 2.0f * Mathf.PI * (float)_random.NextDouble();
        float radius = Mathf.Sqrt((float)_random.NextDouble());

        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        output = new Vector2(x, y);
    }

    #endregion Methods Patches
}
