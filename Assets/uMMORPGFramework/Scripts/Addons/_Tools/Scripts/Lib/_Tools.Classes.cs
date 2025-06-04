using UnityEngine;

namespace uMMORPG
{
    
    
    // SCRIPTABLE OBJECT BY FOLDER
    
    [System.Serializable]
    public partial class ScripableObjectEntry
    {
        public ScriptableObject scriptableObject;
        public string folderName;
    #if _iMMOASSETBUNDLEMANAGER
        public string bundleName;
        public bool loadFromAssetBundle;
    #endif
    }
    
    // MUTABLE WRAPPER
    [System.Serializable]
    public sealed class MutableWrapper<T>
    {
        public T Value;
    
        public MutableWrapper(T value)
        {
            this.Value = value;
        }
    }
    
    // Tools POPUP - CLASS
    [System.Serializable]
    public partial class Tools_PopupClass
    {
        public string message = "Default Message";
        public string suffix = "";
        [Range(0, 255)] public byte iconId;
        [Range(0, 255)] public byte soundId;
    }
    
    // Tools WEIGHTED CHANCE - CLASS
    [System.Serializable]
    public partial class Tools_WeightedChance : MonoBehaviour
    {
        [Range(0, 1)] public float chance;
    }
    
    // Tools ITEM REWARD - CLASS
    [System.Serializable]
    public partial class Tools_ItemReward
    {
        [Range(0, 1)] public float probability;
        public ScriptableItem item;
        public int amount;
    }
    
    // Tools ITEM REQUIREMENT - CLASS
    [System.Serializable]
    public partial class Tools_ItemRequirement
    {
        public ScriptableItem item;
        public int amount = 1;
    }
    
    // Tools ITEM REQUIREMENT - CLASS
    [System.Serializable]
    public partial class Tools_ItemRandomRequirement
    {
        public ScriptableItem item;
        public int amount = 1;
    }
    
    // Tools ITEM MODIFIER - CLASS
    [System.Serializable]
    public partial class Tools_ItemModifier
    {
        public ScriptableItem item;
        public int amount = 1;
    }
    
    // Tools SKILL REQUIREMENT - CLASS
    [System.Serializable]
    public partial class Tools_SkillRequirement
    {
        public ScriptableSkill skill;
        public int level = 1;
    }
    
    // Tools PROFESSION REQUIREMENT - CLASS
    #if _iMMOHARVESTING
    [System.Serializable]
    public partial class HarvestingProfessionRequirement
    {
        public Tmpl_HarvestingProfession template;
    
        [Tooltip("Minimum required profession level?")]
        public int level = 1;
    }
    #endif
    
    // Tools CRAFT REQUIREMENT - CLASS
    #if _iMMOCRAFTING
    [System.Serializable]
    public partial class Tools_CraftingProfessionRequirement
    {
        public CraftingProfessionTemplate template;
        //public Tmpl_CraftingProfession template;
    
        [Tooltip("Minimum required craft level?")]
        public int level = 1;
    }
    #endif
    
    // ATTRIBUTE MODIFIER
    #if _iMMOATTRIBUTES
    [System.Serializable]
    public partial class Tools_AttributeModifier
    {
        public AttributeTemplate template;
    
        [Range(-1f, 1f)]
        public float percentBonus = 0f;
        public int flatBonus = 0;
    }
    #else
    public partial class Tools_AttributeModifier { }
    #endif
    
    // ATTRIBUTE REQUIREMENT
    #if _iMMOATTRIBUTES
    [System.Serializable]
    public partial class Tools_AttributeRequirement
    {
        public AttributeTemplate template;
        public int minValue = 0;
        public int maxValue = 0;
    }
    #else
    public partial class Tools_AttributeRequirement { }
    #endif
    
}
