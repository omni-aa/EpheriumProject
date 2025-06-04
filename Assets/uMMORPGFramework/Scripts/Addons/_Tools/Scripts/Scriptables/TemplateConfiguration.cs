using System;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEditor;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // TemplateConfiguration
    
    [CreateAssetMenu(menuName = "MMO-Indie/Tools/Other/Configuration", fileName = "Configuration")]
    public partial class TemplateConfiguration : ScriptableObject
    {
    
        [Header("Scriptable Object Folders")]
        public ScripableObjectEntry[] scriptableObjects;
    
        static TemplateConfiguration _instance;
    
        // -----------------------------------------------------------------------------------
        // singleton
        // -----------------------------------------------------------------------------------
        public static TemplateConfiguration singleton
        {
            get 
            {
                if (!_instance)
                    _instance = Resources.FindObjectsOfTypeAll<TemplateConfiguration>().FirstOrDefault();
                return _instance;
            }
        }
    
        // -----------------------------------------------------------------------------------
        // GetEntry
        // -----------------------------------------------------------------------------------
        public ScripableObjectEntry GetEntry(Type type)
        {
            foreach (ScripableObjectEntry entry in scriptableObjects)
            {
                if (entry.scriptableObject.GetType() == type)
                    return entry;
            }
    
            return null;
        }
    
        // -----------------------------------------------------------------------------------
        // GetTemplatePath
        // -----------------------------------------------------------------------------------
        public string GetTemplatePath(Type type)
        {
            foreach (ScripableObjectEntry entry in scriptableObjects)
            {
                if (entry.scriptableObject.GetType() == type)
                    return entry.folderName;
            }
    
            return "";
        }
    
        // -----------------------------------------------------------------------------------
        // GetBundlePath
        // -----------------------------------------------------------------------------------
        public string GetBundlePath(Type type)
        {
    #if _iMMOASSETBUNDLEMANAGER
            foreach (ScripableObjectEntry entry in scriptableObjects)
            {
                if (entry.scriptableObject.GetType() == type)
                    return entry.bundleName;
            }
    #endif
            return "";
        }
    
        // -----------------------------------------------------------------------------------
        // GetLoadFromBundle
        // -----------------------------------------------------------------------------------
        public bool GetLoadFromBundle(Type type)
        {
    #if _iMMOASSETBUNDLEMANAGER
            foreach (ScripableObjectEntry entry in scriptableObjects)
            {
                if (entry.scriptableObject.GetType() == type)
                    return entry.loadFromAssetBundle;
            }
    #endif
            return false;
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
