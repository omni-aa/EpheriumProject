namespace uMMORPG
{
    //#if _iMMOTOOLS
    #if UNITY_EDITOR
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    
    // DEFINES MANAGER
    [InitializeOnLoad]
    public partial class DefinesManager
    {
    
        public static List<AddOn> addons = new();
    
        // -----------------------------------------------------------------------------------
        // DefinesManager
        // -----------------------------------------------------------------------------------
        static DefinesManager()
        {
            InvokeMany(typeof(DefinesManager), addons, "Constructor"); ;
        }
    
        // invoke multiple functions by prefix via reflection.
        // -> works for static classes too if object = null
        // -> cache it so it's fast enough for Update calls
        static Dictionary<KeyValuePair<Type, string>, MethodInfo[]> lookup = new();
        public static MethodInfo[] GetMethodsByPrefix(Type type, string methodPrefix)
        {
            KeyValuePair<Type, string> key = new(type, methodPrefix);
            if (!lookup.ContainsKey(key))
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).Where(m => m.Name.StartsWith(methodPrefix)).ToArray();
                lookup[key] = methods;
            }
            return lookup[key];
        }
    
        public static void InvokeMany(Type type, object onObject, string methodPrefix, params object[] args)
        {
            foreach (MethodInfo method in GetMethodsByPrefix(type, methodPrefix))
                method.Invoke(onObject, args);
        }
    
        // -----------------------------------------------------------------------------------
    
    }
    #endif
    //#endif
    
}
