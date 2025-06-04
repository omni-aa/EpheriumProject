using System;
using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // ITEM DROP CHANCE EXTEND
    [Serializable]
    public class ItemDropChanceExtend
    {
        [Range(1, 999)] public int minStack = 1;
        [Range(1, 999)] public int maxStack = 1;
        public ScriptableItem item;
        [Range(0, 1)] public float probability;
        public Tools_ActivationRequirements dropRequirements;
    }
    
}
    #endif
