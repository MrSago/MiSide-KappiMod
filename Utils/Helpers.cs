using KappiMod.Constants;
using KappiMod.Properties;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
#if BIE
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
#endif

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

    public static class Delay
    {
        private static DelayedActionRunner? _runner;

        public static void ExecuteAfter(UnityAction action, float delay)
        {
            if (_runner == null)
            {
                var gameObject = new GameObject($"{BuildInfo.NAME}_{nameof(DelayedActionRunner)}");
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
#if BIE
                _runner = (DelayedActionRunner)
                    IL2CPPChainloader.AddUnityComponent(typeof(DelayedActionRunner));
#else
                _runner = gameObject.AddComponent<DelayedActionRunner>();
#endif
            }

            _runner.ExecuteDelayedAction(action, delay);
        }

        private class DelayedActionRunner : MonoBehaviour
        {
            private UnityAction? _actionToExecute;

            public void ExecuteDelayedAction(UnityAction action, float delay)
            {
                if (_actionToExecute != null)
                {
                    CancelInvoke(nameof(ExecuteAction));
                    ExecuteAction();
                }

                _actionToExecute = action;
                Invoke(nameof(ExecuteAction), delay);
            }

            private void ExecuteAction()
            {
                _actionToExecute?.Invoke();
                _actionToExecute = null;
            }
        }
    }
}
