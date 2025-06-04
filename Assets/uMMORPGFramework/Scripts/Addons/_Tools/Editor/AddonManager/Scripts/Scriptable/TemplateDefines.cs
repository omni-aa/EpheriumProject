using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

#if _iMMOTOOLS && UNITY_EDITOR

namespace uMMORPG
{
    //using UnityEditor.Build;
    using UnityEditor;
    
    // TemplateDefines
    
    [CreateAssetMenu(menuName = "MMO-Indie/Tools/Other/Defines", fileName = "Defines", order = 999)]
    public partial class TemplateDefines : ScriptableObject
    {
    
        static TemplateDefines _instance;
    #if UNITY_EDITOR
        [SerializeField]
        [Header("(Change list size to force refresh)")]
        public List<AddOn> addons = new List<AddOn>();
    #endif
        // -----------------------------------------------------------------------------------
        // OnValidate
        // -----------------------------------------------------------------------------------
        void OnValidate()
        {
    #if UNITY_EDITOR
            if (DefinesManager.addons.Count() > 0 && addons.Count() != DefinesManager.addons.Count() - 1)
            {
                addons.Clear();
    
                for (int i = 0; i < DefinesManager.addons.Count(); ++i)
                {
    
                    AddOn addon = new AddOn();
                    addon.Copy(DefinesManager.addons[i]);
    
                    if (addon.define != "_iMMOTOOLS")
                    {
                        //string currentDefinesStandalone = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone);
                        //string[] definesStandalone = currentDefinesStandalone.Split(';');
                        if (!UMMO_Tools.ArrayContains(DefineSymbols.GetListDefine(), addon.define))
                        {
                            addon.active = false;
                        }
                        addons.Add(addon);
                    }
                    else
                        DefineSymbols.AddScriptingDefine(addon.define);
                }
    
            }
    #endif
        }
    
        // -----------------------------------------------------------------------------------
        // UpdateDefines
        // -----------------------------------------------------------------------------------
        public void UpdateDefines()
        {
    #if UNITY_EDITOR
            for (int i = 0; i < addons.Count(); ++i)
            {
                if (addons[i].define == "_iMMOTOOLS") continue;
    
                if (!addons[i].active)
                    DefineSymbols.RemoveScriptingDefine(addons[i].define);
                else
                    DefineSymbols.AddScriptingDefine(addons[i].define);
            }
    #endif
        }
    
        // -----------------------------------------------------------------------------------
        // Singleton
        // -----------------------------------------------------------------------------------
        public static TemplateDefines singleton
        {
            get 
            {
                if (!_instance)
                    //_instance = SGResources.FindObjectsOfTypeAll<TemplateDefines>().FirstOrDefault();
                    _instance = Resources.FindObjectsOfTypeAll<TemplateDefines>().FirstOrDefault();
                return _instance;
            }
        }
    
        // -----------------------------------------------------------------------------------
    
    }
    #if UNITY_EDITOR
    
    [CustomEditor(typeof(TemplateDefines))]
    public class TestOnInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (TemplateDefines)target;
    
            if (GUILayout.Button("Edit Addons list", GUILayout.Height(20)))
            {
                script.UpdateDefines();
            }
    
        }
    }
    #endif
    
}
#endif
