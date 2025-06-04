using UnityEngine;
using UnityEditor;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    #if UNITY_EDITOR
    //[CustomEditor(typeof(BuildSwitchreManager))]
    public class BuildSwitchreManager : EditorWindow
    {
        [MenuItem("MMO-Indie/Switch Build Mode", false, 500)]
        static void RunOnce()
    //    static void OpenTagsEditorWindow()
        {
            BuildSwitchreManager window = GetWindow<BuildSwitchreManager>(true, "Switch Build Mode v1.1", true);
            window.minSize = new Vector2(440, 275);
            window.maxSize = new Vector2(440, 275);
            window.Show();
            //BuildSwitchreManager window = GetWindow<BuildSwitchreManager>("Switch Build Mode v1.1");
    
        }
    
        public void OnGUI()
        {
            var labelTitle = new GUIStyle(GUI.skin.label);
            labelTitle.richText = true;
            labelTitle.fontSize = 22;
    
            var labelLink = new GUIStyle(GUI.skin.label);
            labelLink.richText = true;
            labelLink.fontSize = 13;
            GUILayout.Label("<b><color=red> Switch Build Mode </color></b> ", labelTitle);
            GUILayout.Space(10);
            GUILayout.Label("this little tool lets you switch between build modes to include only the");
            GUILayout.Label("necessary code, for example not to include server code in a client build.");
            GUILayout.Space(10);
            if (GUILayout.Button("<b>Visit Website :</b> <a>https://mmo-indie.com</a>", labelLink)) { Application.OpenURL("https://mmo-indie.com"); }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.Space(10);
            if (GUILayout.Button("<b>Visit Discord :</b> <a>https://discord.gg/aRCBPGMr7A</a>", labelLink)) { Application.OpenURL("https://discord.gg/aRCBPGMr7A"); }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.Space(30);
    
            GUILayout.Label("<b>Switch Option :</b>", labelLink);
    
            var activeBuild = new GUIStyle(GUI.skin.button);
            activeBuild.normal.textColor = Color.green;
            activeBuild.fontStyle = FontStyle.Bold;
            activeBuild.fontSize = 15;
    
            var inactiveBuild = new GUIStyle(GUI.skin.button);
            inactiveBuild.normal.textColor = Color.red;
            inactiveBuild.fontStyle = FontStyle.Bold;
            inactiveBuild.fontSize = 15;
            string IS_SERVER = "_SERVER";
            string IS_CLIENT = "_CLIENT";
            
            // Switch to Client Build
            bool isClient = UMMO_Tools.ArrayContains(DefineSymbols.GetListDefine(), IS_CLIENT);
            bool isServer = UMMO_Tools.ArrayContains(DefineSymbols.GetListDefine(), IS_SERVER);
    
            if (isClient && !isServer) GUI.enabled = false;
            if (GUILayout.Button("Switch To Client Only", ((isClient && !isServer) ? activeBuild : inactiveBuild)))
            {
                DefineSymbols.RemoveScriptingDefine(IS_SERVER);
                DefineSymbols.AddScriptingDefine(IS_CLIENT);
            }
            if (isClient && !isServer) GUI.enabled = true;
    
            // Switch to Server Build
            if (!isClient && isServer) GUI.enabled = false;
            if (GUILayout.Button("Switch To Server Only", ((!isClient && isServer) ? activeBuild : inactiveBuild)))
            {
                DefineSymbols.AddScriptingDefine(IS_SERVER);
                DefineSymbols.RemoveScriptingDefine(IS_CLIENT);
            }
            if (!isClient && isServer) GUI.enabled = true;
    
            // Switch to Client/Server Build
            if (isClient && isServer) GUI.enabled = false;
            if (GUILayout.Button("Switch To Client/Server", ((isClient && isServer) ? activeBuild : inactiveBuild)))
            {
                DefineSymbols.AddScriptingDefine(IS_SERVER);
                DefineSymbols.AddScriptingDefine(IS_CLIENT);
            }
            if (isClient && isServer) GUI.enabled = true;
        }
    }
    
    #endif
    
}
    #endif
