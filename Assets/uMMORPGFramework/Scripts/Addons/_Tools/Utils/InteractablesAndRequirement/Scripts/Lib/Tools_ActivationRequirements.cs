using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // ACTIVATION REQUIREMENTS CLASS
    // THIS CLASS IS FOR OBJECTS THAT ARE ACTIVATED AUTOMATICALLY IF CERTAIN CRITERIA ARE MET
    
    [System.Serializable]
    public partial class Tools_ActivationRequirements : Tools_Requirements
    {
    #if _iMMOBUILDSYSTEM
    
        [Header("[BUILD SYTEM REQUIREMENTS]")]
        [Tooltip("[Optional] Build System - only the owner character of the structure can access it?")]
        public bool ownerCharacterOnly;
    
        [Tooltip("[Optional] Build System - only the owner guild of the structure can access it?")]
        public bool ownerGuildOnly;
    
        [Tooltip("[Optional] Build System - will reverse both checks from above and only activate when non owner / non guild members access")]
        public bool reverseCheck;
    #endif
    
        protected GameObject parent;
    
        // -----------------------------------------------------------------------------------
        // setParent
        // -----------------------------------------------------------------------------------
        public void setParent(GameObject myParent)
        {
            parent = myParent;
        }
    
        // -----------------------------------------------------------------------------------
        // checkRequirements
        // -----------------------------------------------------------------------------------
        public override bool checkRequirements(Player player)
        {
            bool valid = true;
    
            valid = base.checkRequirements(player);
    
    #if _iMMOBUILDSYSTEM
            valid = (checkBuildSystem(player) == true) && valid;
    #endif
    
            return valid;
        }
    
        // -----------------------------------------------------------------------------------
        // checkBuildSystem
        // -----------------------------------------------------------------------------------
    #if _iMMOBUILDSYSTEM
    
        public bool checkBuildSystem(Player player)
        {
            if (!parent || !parent.GetComponentInParent<PlaceableObject>()) return true;
    
            PlaceableObject po = parent.GetComponentInParent<PlaceableObject>();
    
            if (po == null || (!ownerCharacterOnly && !ownerGuildOnly)) return true;
    
            return
                    (!ownerCharacterOnly || (!reverseCheck && ownerCharacterOnly && player.name == po.ownerCharacter || reverseCheck && ownerCharacterOnly && player.name != po.ownerCharacter)) &&
                    (!ownerGuildOnly || (!reverseCheck && ownerGuildOnly && player.guild.name == po.ownerGuild || reverseCheck && ownerGuildOnly && player.guild.name != po.ownerGuild));
        }
    
    #endif
    
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
