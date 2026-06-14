using System.Diagnostics;
using UnityEngine;

namespace SRPG.Debugging
{
    public static class DevLogger
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}
