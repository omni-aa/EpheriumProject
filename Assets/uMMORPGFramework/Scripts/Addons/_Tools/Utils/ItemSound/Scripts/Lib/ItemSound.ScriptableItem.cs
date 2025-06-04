using UnityEngine;

namespace uMMORPG
{
    
    public partial class ScriptableItem
    {
        [Header("[-=-[ Drop Item sound ]-=-]")]
        [Tooltip("Are playing item pickup")]
        public AudioClip dropSound;
    }
    #if _iMMOTOOLS
    public partial class UsableItem
    {
        [Header("[-=-[ Usable Item sound ]-=-]")]
        [Tooltip("Are played when using the item")]
        public AudioClip usableSound;
    }
    #endif
    
}
