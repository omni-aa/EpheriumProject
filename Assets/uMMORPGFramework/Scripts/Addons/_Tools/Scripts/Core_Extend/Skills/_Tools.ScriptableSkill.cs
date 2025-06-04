using UnityEngine;

namespace uMMORPG
{
    
    public abstract partial class ScriptableSkill
    {
    #if _iMMOSTAMINA
        [Tooltip("Stamina Special Rule: Can be cast with insufficient Stamina as well!")]
        public LinearInt staminaCosts;
    #endif
    
        [Tooltip("This skill cannot be learned via the Skill Window, only via other means")]
        public bool unlearnable;
    
        [Tooltip("Checked = negative skill, Unchecked = positive skill. Certain skills can debuff disadvantageous skills only")]
        public bool disadvantageous;
    
    }
    
}
