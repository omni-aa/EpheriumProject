#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;


namespace uMMORPG
{
    
    public partial class FindMissing : EditorWindow
    {
        static int go_count = 0, components_count = 0, missing_count = 0;
    
        [MenuItem("MMO-Indie/Other/Find Missing")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(FindMissing));
        }
    
        public void OnGUI()
        {
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
            {
                FindInSelected();
            }
            if (GUILayout.Button("Find Missing Scripts in Scene"))
            {
                FindInScene();
            }
            if (GUILayout.Button("Find Missing References in Scene"))
            {
                FindMissingReferencesInScene();
            }
            if (GUILayout.Button("Find Missing Scripts in Project"))
            {
                FindInProject();
            }
            if (GUILayout.Button("Find Missing References in Project"))
            {
                FindMissingReferencesInProject();
            }
            if (GUILayout.Button("Find Missing Scripts in ScriptableObjects"))
            {
                FindMissingScriptsInScriptableObjects();
            }
        }
    
        private static void FindInScene()
        {
            //GameObject[] go = GameObject.FindObjectsOfType<GameObject>();
            GameObject[] go = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
    
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }
    
        private static void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }
        }
    
        public static void FindMissingReferencesInScene()
        {
    
            GameObject[] objects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (var go in objects)
            {
                CheckMissingReferencesInGO(go);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing references", go_count, components_count, missing_count));
        }
    
        private static void FindInProject()
        {
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            go_count = 0;
            components_count = 0;
            missing_count = 0;
    
            foreach (string path in allAssetPaths)
            {
                if (path.EndsWith(".prefab") || path.EndsWith(".unity"))
                {
                    GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (asset != null)
                    {
                        FindInGO(asset);
                    }
                }
            }
            Debug.Log(string.Format("Searched {0} Prefabs/Scenes, found {1} missing", go_count, missing_count));
        }
    
        private static void FindMissingReferencesInProject()
        {
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            go_count = 0;
            components_count = 0;
            missing_count = 0;
    
            foreach (string path in allAssetPaths)
            {
                if (path.EndsWith(".prefab") || path.EndsWith(".unity"))
                {
                    GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (asset != null)
                    {
                        CheckMissingReferencesInGO(asset);
                    }
                }
            }
            Debug.Log(string.Format("Searched {0} Prefabs/Scenes, found {1} missing references", go_count, components_count, missing_count));
        }
    
        private static void FindMissingScriptsInScriptableObjects()
        {
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            go_count = 0;
            components_count = 0;
            missing_count = 0;
    
            foreach (string path in allAssetPaths)
            {
                if (path.EndsWith(".asset"))
                {
                    ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                    if (asset != null)
                    {
                        CheckMissingReferencesInScriptableObject(asset, path);
                    }
                }
            }
            Debug.Log(string.Format("Searched {0} ScriptableObjects, found {1} missing scripts", go_count, missing_count));
        }
    
        private static void CheckMissingReferencesInGO(GameObject go)
        {
            go_count++;
            var components = go.GetComponents<Component>();
    
            foreach (var c in components)
            {
                components_count++;
                if (c == null)
                {
                    Debug.LogError("Missing script found on: " + FullObjectPath(go), go);
                }
                else
                {
                    SerializedObject so = new SerializedObject(c);
                    var sp = so.GetIterator();
    
                    while (sp.NextVisible(true))
                    {
                        if (sp.propertyType != SerializedPropertyType.ObjectReference)
                        {
                            continue;
                        }
    
                        if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                        {
                            ShowErrorWithReferenceDetails(go, so.targetObject.ToString(), sp);
                        }
                    }
                }
            }
            // Recurse through each child GO (if there are any):
            foreach (Transform childT in go.transform)
            {
                CheckMissingReferencesInGO(childT.gameObject);
            }
        }
    
        private static void CheckMissingReferencesInScriptableObject(ScriptableObject so, string assetPath)
        {
            go_count++;
            SerializedObject serializedObject = new SerializedObject(so);
            SerializedProperty sp = serializedObject.GetIterator();
    
            while (sp.NextVisible(true))
            {
                if (sp.propertyType != SerializedPropertyType.ObjectReference)
                {
                    continue;
                }
    
                if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                {
                    missing_count++;
                    Debug.LogError($"Missing reference in ScriptableObject at path: {assetPath}, Property: {sp.name}, at PropertyPath: {sp.propertyPath}", so);
                }
            }
        }
    
        private static void ShowErrorWithReferenceDetails(GameObject go, string scriptName, SerializedProperty sp)
        {
            missing_count++;
            string objectPath = FullObjectPath(go);
            Debug.LogError($"Missing reference found in: {objectPath}, Script: {scriptName}, Property: {sp.name}, at Path: {sp.propertyPath}", go);
        }
    
        private static string FullObjectPath(GameObject go)
        {
            return go.transform.parent == null ? go.name : FullObjectPath(go.transform.parent.gameObject) + "/" + go.name;
        }
    
        private static void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string objectPath = FullObjectPath(g);
                    Debug.LogError(objectPath + " has a missing script at position: " + i, g);
                }
            }
            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in g.transform)
            {
                FindInGO(childT.gameObject);
            }
        }
    }
    
}
#endif
