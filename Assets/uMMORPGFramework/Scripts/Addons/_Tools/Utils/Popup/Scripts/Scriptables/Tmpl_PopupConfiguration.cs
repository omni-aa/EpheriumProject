using UnityEngine;

namespace uMMORPG
{
    
    [CreateAssetMenu(menuName = "MMO-Indie/Tools/Popup Configuration", order = 0)]
    public class Tmpl_PopupConfiguration : ScriptableObject
    {
        [Header("[-=-[ Icons available [-=-]")]
        public Sprite[] availableIcons;
    
        [Header("[-=-[ Sounds available [-=-]")]
        public AudioClip[] availableSounds;
    
    #if UNITY_EDITOR
        private const int MaxSize = 256;
        private void OnValidate()
        {
            if (availableIcons != null && availableIcons.Length > MaxSize)
            {
                Debug.LogWarning($"'availableIcons' exceeds the maximum size of {MaxSize}. Truncating the array.");
                System.Array.Resize(ref availableIcons, MaxSize);
            }
    
            if (availableSounds != null && availableSounds.Length > MaxSize)
            {
                Debug.LogWarning($"'availableSounds' exceeds the maximum size of {MaxSize}. Truncating the array.");
                System.Array.Resize(ref availableSounds, MaxSize);
            }
        }
    #endif
    }
    
}
