#if UNITY_EDITOR
//using ToolsFilesPatcher.Scripts;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Build;

namespace ToolsFilesPatcher.Window
{
    [InitializeOnLoad]
    public class ToolsFilesPatcherWindow : EditorWindow
    {
        static ToolsFilesPatcherWindow()
        {
#if !_iMMOTOOLS
            EditorApplication.update += RunOnce;
#endif
        }

        private readonly string directoryPath = "Assets/uMMORPG/Scripts/Addons/_Tools/Editor/ToolsFilesPatcher/Core";
        private readonly string corePath = "Assets/uMMORPG/Scripts";
        private ScriptsList files; // Déplacer l'initialisation de la liste ici
        private List<string> filesToEdit = new(); // Liste des fichiers à éditer

        // Initialisation de la liste une seule fois lors de la création de la fenêtre
        void OnEnable()
        {
            files = new ScriptsList();
            GetFiles(directoryPath, files); // Remplir la liste de fichiers lors de l'initialisation

            if (File.Exists("Assets/uMMORPG/Scripts/MovementSystems/PlayerNavMeshMovement2D.cs"))
            {
                Debug.Log("2D Version");
                if (!ArrayContains(GetListDefine(), "_iMMO2D"))
                    AddScriptingDefine("_iMMO2D");
            }
            else
            {
                Debug.Log("3D version");
            }
        }

        [MenuItem("MMO-Indie/Scripts Patcher", false, 350)]
        static void RunOnce()
        {
            ToolsFilesPatcherWindow window = GetWindow<ToolsFilesPatcherWindow>(true, "Script Patcher", true);
            window.minSize = new Vector2(605, 370);
            window.maxSize = new Vector2(605, 370);
            window.Show();
#if !_iMMOTOOLS
            EditorApplication.update -= RunOnce;
#endif
        }

        void OnGUI()
        {
            filesToEdit.Clear();
            GUILayout.Label("<b><color=red>Easy Install & Update scripts</color></b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 22 });
            GUILayout.Space(10);
            GUILayout.Label("This Installer edits Scripts and Prefabs for easy installation of Tools.");
            GUILayout.Space(10);

            if (GUILayout.Button("<b>Visit Website :</b> <a>https://mmo-indie.com</a>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 13 }))
                Application.OpenURL("https://mmo-indie.com");
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);

            GUILayout.Space(10);

            if (GUILayout.Button("<b>Visit Discord :</b> <a>https://discord.gg/aRCBPGMr7A</a>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 13 }))
                Application.OpenURL("https://discord.gg/aRCBPGMr7A");
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);

            GUILayout.Space(30);
            GUILayout.Label("<b>List of Edited Files:</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 13 });

            int totalCodeEdit = 0;
            int allScriptISOk = 0;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(600), GUILayout.Height(110));

            foreach (string file in files.script)
            {
                int totalEdit = 0;
                int editedCodeForThisFile = 0;
                string relativeFilePath = Path.ChangeExtension(Path.Combine(corePath, file), ".cs");
                if (File.Exists(relativeFilePath))
                {
                    ++totalCodeEdit;
                    ++totalEdit;
                    //ToolsFilesEditor fileEditor = new();
                    string checkPath = Path.ChangeExtension(file, ".cs");
                    //bool checkedEdit = fileEditor.CompareFileContentsV2(checkPath);
                    bool checkedEdit = CompareFileContentsV2(checkPath);
                    if (checkedEdit)
                    {
                        ++allScriptISOk;
                        ++editedCodeForThisFile;
                    }
                    else
                    {
                        filesToEdit.Add(file);
                    }
                    ShowFile(file, checkedEdit);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.Space(10);

            GUILayout.Label("<b>Patch script:</b>", new GUIStyle(GUI.skin.label) { richText = true, fontSize = 13 });

            GUILayout.Space(10);
            GUI.enabled = (totalCodeEdit != allScriptISOk);
            ButtonEditScript(totalCodeEdit == allScriptISOk);
            GUI.enabled = true;
            if (GUILayout.Button("Search Update"))
            {
                OpenOtherWindow(); // Appel de la méthode pour ouvrir la nouvelle fenêtre
            }
            // 
        }
        private void OpenOtherWindow()
        {
            ScriptUpdateEditorWindow otherWindow = GetWindow<ScriptUpdateEditorWindow>("Script Patcher");
            // Réglez la taille de la nouvelle fenêtre si nécessaire
            otherWindow.minSize = new Vector2(200, 200);
            otherWindow.maxSize = new Vector2(400, 400);
            // Affichez la nouvelle fenêtre
            otherWindow.Show();
        }
        private Vector2 scrollPosition;

        private void ButtonEditScript(bool allScriptSwitched)
        {
            var label = allScriptSwitched ? "All Scripts are already patched" : "Patch all Scripts";
            var style = new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = allScriptSwitched ? Color.red : Color.green },
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };

            if (GUILayout.Button(label, style))
            {
                ListFilesEditByType(filesToEdit);
                AssetDatabase.Refresh();
            }
        }

        private void ListFilesEditByType(List<string> files)
        {
            Debug.Log("Count :" + files.Count);
            foreach (string filename in files)
            {
                Debug.Log("filename :" + filename);
                //FileContentSwapper fileContentSwapper = new();
                //fileContentSwapper.FindAndSwapFileContentsV2(filename);
                FindAndSwapFileContentsV2(filename);
            }

            if (!ArrayContains(GetListDefine(), "_iMMOTOOLS"))
            {
                AddScriptingDefine(";_iMMOTOOLS;_SQLITE;_SERVER;_CLIENT");
            }
        }
        private void ShowFile(string fileName, bool isOk)
        {
            var style = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 22,
                normal = { textColor = isOk ? Color.green : Color.red },
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };
            var inactiveTextField = new GUIStyle(GUI.skin.textField)
            {
                fixedHeight = 22,
                normal = { textColor = isOk ? Color.green : Color.red },
                fontSize = 15
            };
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            GUILayout.TextField(fileName, inactiveTextField);
            GUI.enabled = true;
            GUI.enabled = !isOk;
            if (GUILayout.Button(isOk ? "File OK" : "Show Diff", style, GUILayout.Width(100)))
            {
                //ToolsFileEditor.ShowDiff(fileName);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private void GetFiles(string directoryPath, ScriptsList _files)
        {
            foreach (string file in Directory.GetFiles(directoryPath, "*.csharp", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(directoryPath, file);
                _files.script.Add(relativePath);
            }
        }

        [Serializable]
        public class ScriptsList
        {
            public List<string> script = new List<string>();
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

        private string coreScriptPath = "Assets/uMMORPG/Scripts/Addons/_Tools/Editor/ToolsFilesPatcher/Core/";
        private string originalScriptPath = "Assets/uMMORPG/Scripts/";
        public bool CompareFileContentsV2(string scriptPath)
        {
            string checkCorePath = Path.Combine(coreScriptPath, scriptPath + "harp");
            string checkOriginalPath = Path.Combine(originalScriptPath, scriptPath);

            string[] InRevisionNotInOriginal = LineDiff(checkCorePath, checkOriginalPath).ToArray();
            string[] InOriginalNotInRevision = LineDiff(checkOriginalPath, checkCorePath).ToArray();

            return InRevisionNotInOriginal.Length == 0 && InOriginalNotInRevision.Length == 0;
        }

        public static IEnumerable<string> LineDiff(string originalPath, string revisionPath)
        {
            if (File.Exists(originalPath) && File.Exists(revisionPath))
            {
                var originalLines = File.ReadAllLines(originalPath).Where(line => !string.IsNullOrWhiteSpace(line)); // Exclure les lignes vides
                var revisionLines = File.ReadAllLines(revisionPath).Where(line => !string.IsNullOrWhiteSpace(line)); // Exclure les lignes vides

                return revisionLines.Except(originalLines); // Comparer les lignes et exclure les retours à la ligne
            }
            else
            {
                if (!File.Exists(originalPath))
                    throw new FileNotFoundException("Bad File Path : " + originalPath);

                else if (!File.Exists(revisionPath))
                    throw new FileNotFoundException("Bad File Path : " + revisionPath);

                else
                    throw new FileNotFoundException("Booh");
            }
        }

        public void FindAndSwapFileContentsV2(string originalFilePath)
        {
            // Extraire la partie spécifique du chemin d'accès du fichier original
            string sourcePath = Path.Combine(coreScriptPath, originalFilePath);

            // Construire le chemin d'accès du fichier cible
            string targetFilePath = Path.ChangeExtension(Path.Combine(originalScriptPath, originalFilePath), ".cs");

            // Vérifier si le fichier cible existe
            if (File.Exists(sourcePath))
            {
                // Copier le contenu du fichier Core
                string targetFileContent = File.ReadAllText(sourcePath);
                // Ajout dans le fichier final
                File.WriteAllText(targetFilePath, targetFileContent);
            }
            else
            {
                Debug.LogWarning("Aucun fichier trouvé correspondant au chemin spécifié. " + targetFilePath + " <-> " + sourcePath);
            }
        }



        // Define Symbol Management
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


    }
}
#endif
