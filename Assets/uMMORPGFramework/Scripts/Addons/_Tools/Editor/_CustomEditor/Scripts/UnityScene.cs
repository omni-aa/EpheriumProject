using UnityEngine;

namespace uMMORPG
{
    
    [System.Serializable]
    public class UnityScene
    {
    #pragma warning disable CS0169
        [SerializeField]
        private Object sceneAsset;
    #pragma warning  restore CS0169
        [SerializeField]
        private string sceneName = string.Empty;
    
        public string SceneName
        {
            get { return sceneName; }
            set { sceneName = value; }
        }
    
        public static implicit operator string(UnityScene unityScene)
        {
            return unityScene.SceneName;
        }
    
        public bool IsSet()
        {
            return !string.IsNullOrEmpty(sceneName);
        }
    }
    
}
