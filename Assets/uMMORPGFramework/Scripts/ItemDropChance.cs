// Defines the drop chance of an item for monster loot generation.
using System;
using UnityEngine;

namespace uMMORPG
{
    [Serializable]
    public class ItemDropChance
    {
        public ScriptableItem item;
        [Range(0,1)] public float probability;
    }
}