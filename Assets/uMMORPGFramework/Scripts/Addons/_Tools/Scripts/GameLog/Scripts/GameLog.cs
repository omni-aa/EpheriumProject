using UnityEngine;

namespace uMMORPG
{
    
    public static class GameLog
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogMessage(string _message, GameObject _object = null)
        {
            Debug.Log("(Show in editor only) "+ _message, _object);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(string _message, GameObject _object = null)
        {
            Debug.LogError("(Show in editor only) " + _message, _object);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(string _message, GameObject _object = null)
        {
            Debug.LogWarning("(Show in editor only) " + _message, _object);
        }
    }
    
}
