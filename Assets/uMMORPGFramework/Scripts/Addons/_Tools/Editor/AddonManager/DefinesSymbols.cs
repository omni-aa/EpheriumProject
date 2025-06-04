#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;


namespace uMMORPG
{
    
    [InitializeOnLoad]
    public static partial class DefineSymbols
    {
    
        public static string[] GetListDefine()
        {
            NamedBuildTarget currentNameBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string definesList = PlayerSettings.GetScriptingDefineSymbols(currentNameBuildTarget);
            string[] defines = definesList.Split(';');
            return defines;
        }
    
        // -------------------------------------------------------------------------------
        // AddScriptingDefine
        // -------------------------------------------------------------------------------
        public static void AddScriptingDefine(string define)
        {
            if (ArrayContains(GetListDefine(), define)) return;
            string definesList = string.Join(";", GetListDefine());
            EditDefineForAll((definesList + ";" + define));
    
        }
    
        // -------------------------------------------------------------------------------
        // RemoveScriptingDefine
        // -------------------------------------------------------------------------------
        public static void RemoveScriptingDefine(string define)
        {
            string[] defines = RemoveFromArray(GetListDefine(), define);
            string definesList = string.Join(";", defines);
            EditDefineForAll(definesList);
    
        }
    
        private static void EditDefineForAll(string definesList)
        {
            List<NamedBuildTarget> list = new()
            {
                NamedBuildTarget.Android,
                NamedBuildTarget.EmbeddedLinux,
                NamedBuildTarget.iOS,
                NamedBuildTarget.LinuxHeadlessSimulation,
                //NamedBuildTarget.NintendoSwitch,
                //NamedBuildTarget.PS4,
                NamedBuildTarget.Server,
                //NamedBuildTarget.Stadia,
                NamedBuildTarget.Standalone,
                //NamedBuildTarget.tvOS,
                //NamedBuildTarget.Unknown,
                //NamedBuildTarget.WindowsStoreApps,
                //NamedBuildTarget.XboxOne,
                NamedBuildTarget.WebGL
            };
    
            foreach (NamedBuildTarget item in list)
            {
                try
                {
                    PlayerSettings.SetScriptingDefineSymbols(item, (definesList));
                }
                catch (Exception e)
                {
                    Debug.LogError("Sorry " + e + "is not valid");
                }
            }
        }
    
        public static bool ArrayContains(string[] defines, string define)
        {
            foreach (string def in defines)
            {
                if (def == define)
                    return true;
            }
            return false;
        }
    
        // -------------------------------------------------------------------------------
        // RemoveFromArray
        // -------------------------------------------------------------------------------
        public static string[] RemoveFromArray(string[] defines, string define)
        {
            return defines.Where(x => x != define).ToArray();
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
#endif
