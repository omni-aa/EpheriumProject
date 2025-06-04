#if _iMMOTOOLS

namespace uMMORPG
{
    public partial struct Buff
    {
        public bool cannotRemove { get { return data.cannotRemove; } }
        public bool blockNerfs { get { return data.blockNerfs; } }
        public bool blockBuffs { get { return data.blockBuffs; } }
    
    #if _iMMOBUFFBLOCKHEALTHRECOVERY
        public bool blockHealthRecovery { get { return data.blockHealthRecovery; } }
    #endif
    #if _iMMOBUFFBLOCKMANARECOVERY
        public bool blockManaRecovery { get { return data.blockManaRecovery; } }
    #endif
    
    #if _iMMOSTAMINA
        //public bool     blockStaminaRecovery            => data.blockStaminaRecovery;
        //public int      bonusStaminaMax                 => data.bonusStaminaMax.Get(level);
        //public float    bonusStaminaPercentPerSecond    => data.bonusStaminaPercentPerSecond.Get(level);
        public int staminaMaxBonus => data.staminaMaxBonus.Get(level);
        public float staminaPercentPerSecondBonus => data.staminaPercentPerSecondBonus.Get(level);
    #endif
    
    #if _iMMOBUFFENDURE
        public bool endure { get { return data.endure; } }
    #endif
    #if _iMMOBUFFEXPERIENCE
        public float boostExperience { get { return data.boostExperience; } }
    #endif
        public float boostGold { get { return data.boostGold; } }
    
        public bool invincibility { get { return data.invincibility; } }
    
    #if _iMMOBUFFHEALPERPOINT
        public float healthPointsPerSecondBonus => data.healthPointsPerSecondBonus.Get(level); //TODO a finir heal par point et pas par pourcentage
    #endif
        // -----------------------------------------------------------------------------------
        // CheckBuffType
        // -----------------------------------------------------------------------------------
        public bool CheckBuffType(BuffType buffType)
        {
            if (buffType == BuffType.Both) return true;
    
            return ((buffType == BuffType.Buff && !data.disadvantageous) || (buffType == BuffType.Nerf && data.disadvantageous));
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
