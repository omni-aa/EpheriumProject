#if UNITY_EDITOR

namespace uMMORPG
{
    
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEngine.SceneManagement;
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;
    using System.Linq;
    
    public class ServerBuildSettings : IActiveBuildTargetChanged
    {
        public int callbackOrder => 0;
    
        // Liste pour sauvegarder les terrains d�sactiv�s
        private static List<Terrain> savedTerrains = new List<Terrain>();
    
        // Lorsque la plateforme de build change, ce callback est appel�
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            CheckServerBuildMode();
        }
    
        // M�thode qui v�rifie si le mode "Server" est activ� dans les Build Settings
        public static void CheckServerBuildMode()
        {
            if (EditorUserBuildSettings.standaloneBuildSubtarget == StandaloneBuildSubtarget.Server)
            {
                DisableTerrainsInAllScenes();
            }
            else
            {
                RestoreTerrainsInAllScenes();
            }
        }
    
        // D�sactive tous les terrains dans chaque sc�ne active du Build Settings
        private static void DisableTerrainsInAllScenes()
        {
            foreach (var scenePath in GetActiveScenePaths())
            {
                // Charge la sc�ne en mode Additive sans la rendre active
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
    
                // D�sactive les terrains dans cette sc�ne
                Terrain[] terrainsInScene = scene.GetRootGameObjects()
                    .SelectMany(go => go.GetComponentsInChildren<Terrain>(true))
                    .ToArray();
    
                foreach (Terrain terrain in terrainsInScene)
                {
                    if (terrain != null)
                    {
                        terrain.enabled = false;
                        savedTerrains.Add(terrain);
                    }
                }
    
                // Sauvegarde la sc�ne pour que les changements soient pris en compte lors du build
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"Terrains d�sactiv�s et sc�ne sauvegard�e : {scenePath}");
    
                // D�charge la sc�ne pour �viter de la garder en m�moire
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    
        // R�active tous les terrains dans chaque sc�ne active du Build Settings
        private static void RestoreTerrainsInAllScenes()
        {
            foreach (var scenePath in GetActiveScenePaths())
            {
                // Charge la sc�ne en mode Additive sans la rendre active
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
    
                // R�active les terrains dans cette sc�ne
                Terrain[] terrainsInScene = scene.GetRootGameObjects()
                    .SelectMany(go => go.GetComponentsInChildren<Terrain>(true))
                    .ToArray();
    
                foreach (Terrain terrain in terrainsInScene)
                {
                    if (terrain != null)
                    {
                        terrain.enabled = true;
                    }
                }
    
                // Sauvegarde la sc�ne pour que les changements soient pris en compte
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"Terrains r�activ�s et sc�ne sauvegard�e : {scenePath}");
    
                // D�charge la sc�ne pour �viter de la garder en m�moire
                EditorSceneManager.CloseScene(scene, true);
            }
    
            savedTerrains.Clear();
        }
    
        // M�thode pour obtenir tous les chemins des sc�nes actives dans le Build Settings
        private static List<string> GetActiveScenePaths()
        {
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled)  // On ne prend que les sc�nes actives dans le Build Settings
                .Select(scene => scene.path)    // On r�cup�re le chemin des sc�nes
                .ToList();
        }
    }
    
}
#endif
