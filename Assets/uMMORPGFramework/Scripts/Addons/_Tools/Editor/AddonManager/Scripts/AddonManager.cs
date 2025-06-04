using UnityEditor;
using System.Collections.Generic;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    using System.Linq;
    using UnityEngine.Networking;
    using System;
    using System.Threading.Tasks;
    
    #if UNITY_EDITOR
    using System.IO;
    using UnityEngine;
    
    [InitializeOnLoad]
    public class AddonManager : EditorWindow
    {
        private string uriListServer = "https://mmo-indie.com/addons_version.php";
        private AddonListAPI addonsList;
        TemplateDefines target;
        static string[] listDefine;
        public string listAddonJson = "";
    
        [MenuItem("MMO-Indie/Addons Manager", false, 50)]
        static void RunOnce()
        {
            AddonManager window = GetWindow<AddonManager>(true, "Addons Manager v2", true);
    
            window.target = (TemplateDefines)AssetDatabase.LoadAllAssetsAtPath("Assets/uMMORPG/Scripts/Addons/_Tools/Editor/Config/Defines.asset")[0];
            window.minSize = new Vector2(800, 500);
            window.maxSize = new Vector2(800, 500);
            window.Show();
    
            RefreshDefine();
        }
        private void OnEnable()
        {
    
            RefreshDefine();
    #pragma warning disable
            GetRequest(uriListServer);
    #pragma
        }
        Vector2 scrollPosition;
        void OnGUI()
        {
    
            RefreshDefine();
            var labelTitle = new GUIStyle(GUI.skin.label);
            labelTitle.richText = true;
            labelTitle.fontSize = 22;
    
            var labelLink = new GUIStyle(GUI.skin.label);
            labelLink.richText = true;
            labelLink.fontSize = 13;
            
    
            var activeBuild = new GUIStyle(GUI.skin.button);
            activeBuild.normal.textColor = Color.green;
            activeBuild.fontStyle = FontStyle.Bold;
            activeBuild.fontSize = 15;
    
            var inactiveBuild = new GUIStyle(GUI.skin.button);
            inactiveBuild.normal.textColor = Color.red;
            inactiveBuild.fontStyle = FontStyle.Bold;
            inactiveBuild.fontSize = 15;
            GUILayout.Label("<b><color=red>Addon Manager : </color></b> ", labelTitle);
            GUILayout.Space(10);
            GUILayout.Label("This tool allows you to quickly know the addons available for your version of ummorpg, to know if");
            GUILayout.Label(" you have installed them, and if they are up to date or not.");
            GUILayout.Label("one click is enough to open the download page in your default browser");
            GUILayout.Space(10);
            if (GUILayout.Button("<b>Visit Website :</b> <a>https://mmo-indie.com</a>", labelLink)) { Application.OpenURL("https://mmo-indie.com"); }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.Space(10);
            if(GUILayout.Button("<b>Visit Discord :</b> <a>https://discord.gg/aRCBPGMr7A</a>", labelLink)){ Application.OpenURL("https://discord.gg/aRCBPGMr7A"); }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.Space(30);
            /*Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/uMMORPG/Plugins/Navigation2D/logo.png", typeof(Texture));
            GUILayout.Box(banner);*/
            GUILayout.Label("<b>Addons :</b>", labelLink);
    
            var listHeaderRowTextField = new GUIStyle(GUI.skin.button);
            listHeaderRowTextField.normal.textColor = Color.white;
            listHeaderRowTextField.fontStyle = FontStyle.Bold;
            listHeaderRowTextField.fontSize = 13;
    
            GUILayout.BeginHorizontal(); //side by side columns
    
            GUILayout.BeginVertical(GUILayout.Width(289)); //Layout objects vertically in each column
            GUI.enabled = false;
            GUILayout.TextField("Addon Name", listHeaderRowTextField);
            GUI.enabled = true;
            GUILayout.EndVertical();
    
            GUILayout.BeginVertical(GUILayout.Width(120));
            GUI.enabled = false;
            GUILayout.TextField("Release Date", listHeaderRowTextField);
            GUI.enabled = true;
            GUILayout.EndVertical();
    
            GUILayout.BeginVertical(GUILayout.Width(120));
            GUI.enabled = false;
            GUILayout.TextField("Status", listHeaderRowTextField);
            GUI.enabled = true;
            GUILayout.EndVertical();
    
            GUILayout.BeginVertical(GUILayout.Width(120));
            GUI.enabled = false;
            GUILayout.TextField("Version", listHeaderRowTextField);
            GUI.enabled = true;
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(GUILayout.Width(120));
            GUI.enabled = false;
            GUILayout.TextField("Enable/Disable", listHeaderRowTextField);
            GUI.enabled = true;
            GUILayout.EndVertical();
    
            GUILayout.EndHorizontal();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(800), GUILayout.Height(245));
                GUILayout.BeginHorizontal(); //side by side columns
    
            AddonList();
    
            GUILayout.EndHorizontal();
    
             GUILayout.EndScrollView();       //GUILayout.EndArea();
    
            /**
             * Button edit script
             */
    #region EditButton
            if (GUILayout.Button("Check Addon Version", activeBuild))
            {
                RefreshAddonVersion();
            }
    #endregion
        }
    
        private void AddonList()
        {
            GUILayout.BeginVertical(GUILayout.Width(289)); //Layout objects vertically in each column
            RefeshUIAddonName();
            GUILayout.EndVertical();
    
            GUILayout.BeginVertical(GUILayout.Width(120));
            RefeshUIAddonRelease();
            GUILayout.EndVertical();
    
            GUILayout.BeginVertical(GUILayout.Width(120));
            RefeshUIAddonUpToDate();
            GUILayout.EndVertical();
    
            GUILayout.BeginVertical(GUILayout.Width(120));
            RefeshUIAddonVersion();
            GUILayout.EndVertical();
    
            GUILayout.BeginVertical(GUILayout.Width(120));
            RefeshUIAddonEnableDisable();
            GUILayout.EndVertical();
        }
    
        private bool CheckFileExist(string path)
        {
            return File.Exists(path);
        }
    
        private string AddonVersion(string path)
        {
            string version = "Version file not found";
    
            if (CheckFileExist(path))
            {
                version = File.ReadLines(path).First();
            }
    
            return version;
        }
    
        private void RefeshUIAddonName()
        {
            var btnSkin = new GUIStyle(GUI.skin.button);
            btnSkin.fontStyle = FontStyle.Bold;
            btnSkin.fontSize = 13;
            
            if (addonsList != null)
            {
                if (addonsList.addons.Count > 0)
                {
                    foreach (var item in addonsList.addons)
                    {
                        btnSkin.normal.textColor = Color.white;
                        GUI.enabled = false;
                        GUILayout.TextField(item.name, btnSkin);
                        GUI.enabled = true;
                    }
                }
                else
                {
                    btnSkin.normal.textColor = Color.red;
                    GUI.enabled = false;
                    GUILayout.TextField("No item found", btnSkin);
                    GUI.enabled = true;
                }
            }
        }
    
        private void RefeshUIAddonRelease()
        {
            var btnSkin = new GUIStyle(GUI.skin.button);
            btnSkin.fontStyle = FontStyle.Bold;
            btnSkin.fontSize = 13;
    
            if (addonsList != null)
            {
                if (addonsList.addons.Count > 0)
                {
                    foreach (var item in addonsList.addons)
                    {
                        btnSkin.normal.textColor = Color.green;
                        GUI.enabled = false;
                        GUILayout.TextField(item.updateDate, btnSkin);
                        GUI.enabled = true;
                    }
                }
                else
                {
                    btnSkin.normal.textColor = Color.red;
                    GUI.enabled = false;
                    GUILayout.TextField("No item found", btnSkin);
                    GUI.enabled = true;
                }
            }
        }
     
        private void RefeshUIAddonUpToDate()
        {
            var skinButton = new GUIStyle(GUI.skin.button);
            skinButton.fontStyle = FontStyle.Bold;
            skinButton.fontSize = 13;
            skinButton.richText = true;
            if (addonsList != null)
            {
                if (addonsList.addons.Count > 0)
                {
                    foreach (var item in addonsList.addons)
                    {
                        string path = "Assets/uMMORPG/Scripts/Addons/" + item.nameSlug;
                        if (Directory.Exists(path))
                        {
    
    
                            // le fichier version existe pas ( ne devrais pas être placé ici
                            if (!CheckFileExist(path + "/version.txt"))
                            {
                                skinButton.normal.textColor = Color.yellow;
                                if (GUILayout.Button("Version missing", skinButton))
                                    Application.OpenURL("https://mmo-indie.com/download/" + item.nameSlug);
                            }
                            else
                            {
                                // Online version (on ne regarde pas la version de unity)
                                string output = item.filename.Substring(item.filename.IndexOf('.') + 1);
                                string[] partsOnline = output.Split('-');
                                string ummoVersionOnline = (partsOnline.Length >1) ? partsOnline[1].Split('.')[0] : "0"; // Récupère "39"
                                string addonVersionOnline = (partsOnline.Length > 1) ? partsOnline[1].Split('.')[1] : "0"; // Récupère "1"
    
                                // Locale version (on ne regarde pas la version de unity)
                                string localeVersion = AddonVersion(path + "/version.txt");
                                string[] partsLocale = localeVersion.Split('-');
                                string ummoVersionLocale = (partsLocale.Length > 1) ? partsLocale[1].Split('.')[0] : "0"; // Récupère "39"
                                string addonVersionLocale = (partsLocale.Length > 1) ? partsLocale[1].Split('.')[1] : "0"; // Récupère "1"
    
                                // La version est à jour
                                if (ummoVersionOnline == ummoVersionLocale && addonVersionOnline == addonVersionLocale)
                                {
                                    GUI.enabled = false;
                                    skinButton.normal.textColor = Color.green;
                                    GUILayout.TextField("Up To Date", skinButton);
                                    GUI.enabled = true;
                                }
    
                                // pas à jour
                                else
                                {
                                    // if faut mettre à jour le site car notre version locale est supérieure à la version en ligne
                                    if (ummoVersionLocale.ToInt() > ummoVersionOnline.ToInt() || ummoVersionLocale.ToInt() == ummoVersionOnline.ToInt() && addonVersionLocale.ToInt() > addonVersionOnline.ToInt())
                                    {
                                        //Debug.Log(ummoVersionOnline + "." + addonVersionOnline + " <> " + ummoVersionLocale + "." + addonVersionLocale);
                                        GUI.enabled = false;
                                        skinButton.normal.textColor = Color.cyan;
                                        if (GUILayout.Button("Update Now    ", skinButton))
                                            Application.OpenURL("https://mmo-indie.com/download/" + item.nameSlug);
                                    }
                                    else
                                    {
                                        //Debug.Log(ummoVersionOnline + "." + addonVersionOnline + " <> " + ummoVersionLocale + "." + addonVersionLocale);
                                        GUI.enabled = false;
                                        skinButton.normal.textColor = Color.cyan;
                                        if (GUILayout.Button("Outdated", skinButton))
                                            Application.OpenURL("https://mmo-indie.com/download/" + item.nameSlug);
                                    }
                                }
                            }
                        }
    
                        // Pas installer
                        else
                        {
                            skinButton.normal.textColor = Color.red;
                            if (GUILayout.Button("Download", skinButton))
                                Application.OpenURL("https://mmo-indie.com/download/" + item.nameSlug);
                        }
    
                    }
                }
                else
                {
                    skinButton.normal.textColor = Color.red;
                    GUI.enabled = false;
                    GUILayout.TextField("No item found", skinButton);
                    GUI.enabled = true;
                }
            }
        }
    
        private void RefeshUIAddonVersion()
        {
            var skinButton = new GUIStyle(GUI.skin.button);
            skinButton.fontStyle = FontStyle.Bold;
            skinButton.fontSize = 13;
            skinButton.richText = true;
            if (addonsList != null)
            {
                if (addonsList.addons.Count > 0)
                {
                    foreach (var item in addonsList.addons)
                    {
                        string path = "Assets/uMMORPG/Scripts/Addons/" + item.nameSlug;
                        if (Directory.Exists(path))
                        {
    
    
                            // le fichier version existe pas ( ne devrais pas être placé ici
                            if (!CheckFileExist(path + "/version.txt"))
                            {
                                skinButton.normal.textColor = Color.yellow;
                                if (GUILayout.Button("Version missing", skinButton))
                                    Application.OpenURL("https://mmo-indie.com/download/" + item.nameSlug);
                            }
                            else
                            {
                                // Online version (on ne regarde pas la version de unity)
                                string output = item.filename.Substring(item.filename.IndexOf('.') + 1);
                                string[] partsOnline = output.Split('-');
                                string ummoVersionOnline = (partsOnline.Length > 1) ? partsOnline[1].Split('.')[0] : "0"; // Récupère "39"
                                string addonVersionOnline = (partsOnline.Length > 1) ? partsOnline[1].Split('.')[1] : "0"; // Récupère "1"
    
                                // Locale version (on ne regarde pas la version de unity)
                                string localeVersion = AddonVersion(path + "/version.txt");
                                string[] partsLocale = localeVersion.Split('-');
                                string ummoVersionLocale = (partsLocale.Length > 1) ? partsLocale[1].Split('.')[0] : "0"; // Récupère "39"
                                string addonVersionLocale = (partsLocale.Length > 1) ? partsLocale[1].Split('.')[1] : "0"; // Récupère "1"
    
                                // La version est à jour
                                if (ummoVersionOnline == ummoVersionLocale && addonVersionOnline == addonVersionLocale)
                                {
                                    GUI.enabled = false;
                                    skinButton.normal.textColor = Color.green;
                                    GUILayout.TextField("V " + ummoVersionLocale + "." + addonVersionLocale, skinButton);
                                    GUI.enabled = true;
                                }
    
                                // pas à jour
                                else
                                {
                                    // if faut mettre à jour le site car notre version locale est supérieure à la version en ligne
                                    if (ummoVersionLocale.ToInt() > ummoVersionOnline.ToInt())
                                    {
                                        //Debug.Log(ummoVersionOnline + "." + addonVersionOnline + " <> " + ummoVersionLocale + "." + addonVersionLocale);
                                        GUI.enabled = false;
                                        skinButton.normal.textColor = Color.blue;
                                        GUILayout.TextField("V " + ummoVersionLocale + "." + addonVersionLocale, skinButton);
                                        GUI.enabled = true;
                                    }
                                    else
                                    {
                                        // la version de ummo <= à celle en ligne est  ou la version de l'addon > différente
                                        if (ummoVersionLocale.ToInt() == ummoVersionOnline.ToInt() && addonVersionLocale.ToInt() > addonVersionOnline.ToInt())
                                        {
                                            //Debug.Log(ummoVersionOnline + "." + addonVersionOnline + " <> " + ummoVersionLocale + "." + addonVersionLocale);
                                            GUI.enabled = false;
                                            skinButton.normal.textColor = Color.red;
                                            GUILayout.TextField("V " + ummoVersionLocale + "." + addonVersionLocale, skinButton);
                                            GUI.enabled = true;
                                        }
                                        else
                                        {
                                            //Debug.Log(ummoVersionOnline + "." + addonVersionOnline + " <> " + ummoVersionLocale + "." + addonVersionLocale);
                                            GUI.enabled = false;
                                            skinButton.normal.textColor = Color.yellow;
                                            GUILayout.TextField("V " + ummoVersionLocale + "." + addonVersionLocale, skinButton);
                                            GUI.enabled = true;
                                        }
                                    }
    
    
                                }
                            }
                        }
    
    
                        // Pas installer
                        else
                        {
    
                            GUI.enabled = false;
                            skinButton.normal.textColor = Color.white;
                            GUILayout.TextField("---", skinButton);
                            GUI.enabled = true;
                        }
    
                    }
                }
                else
                {
                    GUI.enabled = false;
                    skinButton.normal.textColor = Color.white;
                    GUILayout.TextField("---", skinButton);
                    GUI.enabled = true;
    
                }
            }
        }
        private void RefeshUIAddonEnableDisable()
        {
            //Debug.Log(listDefine.Length + " <<");
            /*foreach (var item in target.addons)
            {
                Debug.Log(item.name);
            }*/
            var skinButton = new GUIStyle(GUI.skin.button);
            skinButton.fontStyle = FontStyle.Bold;
            skinButton.fontSize = 13;
            skinButton.richText = true;
            if (addonsList != null)
            {
                if (addonsList.addons.Count > 0)
                {
                    foreach (var item in addonsList.addons)
                    {
                        string path = "Assets/uMMORPG/Scripts/Addons/" + item.nameSlug;
    
                        if (Directory.Exists(path))
                        {
                            AddOn matchingAddons = target.addons.Find(addon => (addon.name == item.nameSlug || addon.name == item.name));
                            if (matchingAddons != null)
                            {
                                //Debug.Log(item.nameSlug);
                                ShowAddon(matchingAddons);
                            }
                            else
                            {
                                GUI.enabled = false;
                                skinButton.normal.textColor = Color.white;
                                GUILayout.TextField("Enabled", skinButton);
                                GUI.enabled = true;
                            }
                        }
    
    
                        // Pas installer
                        else
                        {
    
                            GUI.enabled = false;
                            skinButton.normal.textColor = Color.white;
                            GUILayout.TextField("---", skinButton);
                            GUI.enabled = true;
                        }
    
                    }
                }
                else
                {
                    GUI.enabled = false;
                    skinButton.normal.textColor = Color.white;
                    GUILayout.TextField("---", skinButton);
                    GUI.enabled = true;
    
                }
            }
        }
    
    
        private void RefreshAddonVersion()
        {
            GetRequest(uriListServer);
        }
    
        public void CreateFromJSON()
        {
            addonsList = JsonUtility.FromJson<AddonListAPI>("{\"addons\":" + listAddonJson + "}");
        }
    
        internal async Task GetRequest(string uri)
        {
            UnityWebRequest request = UnityWebRequest.Get(uri);
            request.timeout = 15;
            request.method = UnityWebRequest.kHttpVerbGET;
    
            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
    
            // Catch the event as it's own method
            asyncOperation.completed += DoStuff;
    
            // Catch the event as lambda
            asyncOperation.completed += (obj) =>
            {
                Debug.Log(uri + " return :" + request.downloadHandler.text);
                listAddonJson = request.downloadHandler.text;
                CreateFromJSON();
            };
    
            // Or just block unitl done
            while (!asyncOperation.isDone)
            {
                await Task.Delay(100);
            }
        }
    
        private static void DoStuff(AsyncOperation obj)
        {
            obj.completed -= DoStuff;
    
            // Cast it back to a request
            UnityWebRequestAsyncOperation asyncRequestObj = (UnityWebRequestAsyncOperation)obj;
            UnityWebRequest request = asyncRequestObj.webRequest;
        }
    
        [Serializable]
        public class AddonsAPI
        {
            public string name;
            public string nameSlug;
            public string filename;
            public string updateDate;
            public string addon_version;
        }
        [Serializable]
        public class AddonListAPI
        {
            public List<AddonsAPI> addons;
        }
    
        private void EnableDisableAddon(AddOn addon)
        {
            bool isActive = (UMMO_Tools.ArrayContains(listDefine, addon.define));
    
            if (isActive)
                DefineSymbols.RemoveScriptingDefine(addon.define);
            else
                DefineSymbols.AddScriptingDefine(addon.define);
    
        }
    
        private static void RefreshDefine()
        {
            listDefine = DefineSymbols.GetListDefine();
        }
    
        private void ShowAddon(AddOn addon)
        {
            var btnSkin = new GUIStyle(GUI.skin.button);
            btnSkin.fontStyle = FontStyle.Bold;
            btnSkin.fontSize = 13;
    
            bool isActive = (UMMO_Tools.ArrayContains(listDefine, addon.define));
    
            var skinButton = new GUIStyle(GUI.skin.button);
            skinButton.fontStyle = FontStyle.Bold;
            skinButton.fontSize = 13;
            skinButton.richText = true;
    
    
            skinButton.normal.textColor = isActive ? Color.green : Color.red;
            if (GUILayout.Button(isActive ? "Enabled" : "Disabled", skinButton))
                EnableDisableAddon(addon);
            /*skinButton.normal.textColor = Color.white;
            GUILayout.TextField("bouton", skinButton);
            GUI.enabled = true;*/
        }
    }
    
    #endif
    
}
#endif
