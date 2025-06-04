using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR

namespace uMMORPG
{
    
    public class FolderCreatorWindow : EditorWindow
    {
        private string gameName = "MyGame";
        private static FolderCreatorWindow window;
        private Texture2D infoIcon;
    
        [MenuItem("MMO-Indie/New Project/Create Default Folder Structure")]
        static void RunOnce()
        {
            // V�rifie si la fen�tre est d�j� ouverte pour ne pas en cr�er une nouvelle
            if (window == null)
            {
                window = GetWindow<FolderCreatorWindow>(true, "Create Folder Project", true);
                window.minSize = new Vector2(400, 200);  // Taille minimale de la fen�tre
                window.maxSize = new Vector2(400, 200);  // Taille maximale pour �viter le redimensionnement
                window.Show();  // Affiche la fen�tre comme une fen�tre ind�pendante
            }
            else
            {
                window.Focus();  // Si la fen�tre est d�j� ouverte, on la met au premier plan
            }
        }
    
        private void OnEnable()
        {
            // Charger une ic�ne d'information (Unity propose certaines ic�nes par d�faut)
            infoIcon = EditorGUIUtility.IconContent("console.infoicon").image as Texture2D;
        }
    
        private void OnGUI()
        {
            // Espace pour a�rer l'interface
            GUILayout.Space(10);
    
            // Affiche l'ic�ne et un encadr� avec une description (sans l'ic�ne par d�faut de HelpBox)
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(infoIcon, GUILayout.Width(32), GUILayout.Height(32));  // Affiche l'ic�ne
            GUILayout.BeginVertical("box");  // Contour encadr�
            GUILayout.Label("This tool helps you quickly create a folder structure for organizing your Unity project. You can specify the game name, and the necessary directories will be automatically generated under the 'Assets/uMMORPG' folder.", EditorStyles.wordWrappedLabel);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
    
            GUILayout.Space(20);
    
            // Champ pour entrer le nom du jeu
            GUILayout.Label("Game Name", EditorStyles.boldLabel);
            gameName = EditorGUILayout.TextField("Game Name:", gameName);
    
            // Bouton pour cr�er la structure des dossiers
            if (GUILayout.Button("Create the folder structure"))
            {
                if (string.IsNullOrEmpty(gameName))
                {
                    EditorUtility.DisplayDialog("Error", "Game name cannot be empty.", "OK");
                    return;
                }
    
                CreateFolders(gameName);
            }
        }
    
        private void CreateFolders(string gameName)
        {
            string rootPath = $"Assets/uMMORPG/_{gameName}";
            string[] folders = new string[]
            {
                $"{rootPath}/Prefabs",
                $"{rootPath}/Materials",
                $"{rootPath}/Models",
                $"{rootPath}/Resources",
                $"{rootPath}/Scenes",
                $"{rootPath}/Shaders",
                $"{rootPath}/Terrains",
                $"{rootPath}/Textures",
                $"{rootPath}/Animations",
                $"{rootPath}/Sounds"
            };
    
            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
    
            AssetDatabase.Refresh();
        }
    }
    
}
#endif

