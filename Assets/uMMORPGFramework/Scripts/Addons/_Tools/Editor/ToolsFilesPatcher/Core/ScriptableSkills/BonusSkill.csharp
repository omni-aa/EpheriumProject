#if _iMMOTOOLS
// Base type for bonus skill templates.
// => can be used for passive skills, buffs, etc.
using System.Text;
using UnityEngine;

namespace uMMORPG
{
    public abstract partial class BonusSkill : ScriptableSkill
    {
        public LinearInt healthMaxBonus;
        public LinearInt manaMaxBonus;
        public LinearInt damageBonus;
        public LinearInt defenseBonus;
        public LinearFloat blockChanceBonus; // range [0,1]
        public LinearFloat criticalChanceBonus; // range [0,1]
        public LinearFloat healthPercentPerSecondBonus; // 0.1=10%; can be negative too
        public LinearFloat manaPercentPerSecondBonus; // 0.1=10%; can be negative too
        public LinearFloat speedBonus; // can be negative too

    #if _iMMOSTAMINA
        //public LinearInt bonusStaminaMax;
        public LinearInt staminaMaxBonus;
        //public LinearFloat bonusStaminaPercentPerSecond;
        public LinearFloat staminaPercentPerSecondBonus;
    #endif

        [Header("[-=-[ BUFF ]-=-]")]
        [Tooltip("Buff cannot be removed via debuffing, it must TimeLogout-out by itself instead.")]
        public bool cannotRemove;

        [Tooltip("Blocks negative status effects being applied, while buff is active")]
        public bool blockNerfs;

        [Tooltip("Blocks positive status effects being applied, while buff is active")]
        public bool blockBuffs;

        [Tooltip("Blocks health recovery (and only recovery), while buff is active")]
        public bool blockHealthRecovery;

        [Tooltip("Blocks mana recovery (and only recovery), while buff is active")]
        public bool blockManaRecovery;

    #if _iMMOSTAMINA
        [Tooltip("Blocks Stamina recovery (and only recovery), while buff is active")]
        public bool blockStaminaRecovery;
    #endif

    #if _iMMOBUFFENDURE
        [Tooltip("Prevents losing the final Healthpoint, while buff is active (= cannot die)")]
        public bool endure;
    #endif

    #if _iMMOBUFFEXPERIENCE
        [Tooltip("Increases the amount of experience gained by this factor, while buff is active (0.5=50%, 1.5=150% etc.)")]
        public float boostExperience;
    #endif

        [Tooltip("Increases the amount of gold gained by this factor, while buff is active (0.5=50%, 1.5=150% etc.)")]
        public float boostGold;

        [Tooltip("Completely invulnerable while buff is active.")]
        public bool invincibility;

    #if _iMMOBUFFHEALPERPOINT
        public LinearInt healthPointsPerSecondBonus; //TODO a finir heal par point et pas par pourcentage
    #endif

        // tooltip
        public override string ToolTip(int skillLevel, bool showRequirements = false)
        {
            StringBuilder tip = new StringBuilder(base.ToolTip(skillLevel, showRequirements));
            tip.Replace("{HEALTHMAXBONUS}", healthMaxBonus.Get(skillLevel).ToString());
            tip.Replace("{MANAMAXBONUS}", manaMaxBonus.Get(skillLevel).ToString());
            tip.Replace("{DAMAGEBONUS}", damageBonus.Get(skillLevel).ToString());
            tip.Replace("{DEFENSEBONUS}", defenseBonus.Get(skillLevel).ToString());
            tip.Replace("{BLOCKCHANCEBONUS}", Mathf.RoundToInt(blockChanceBonus.Get(skillLevel) * 100).ToString());
            tip.Replace("{CRITICALCHANCEBONUS}", Mathf.RoundToInt(criticalChanceBonus.Get(skillLevel) * 100).ToString());
            tip.Replace("{HEALTHPERCENTPERSECONDBONUS}", Mathf.RoundToInt(healthPercentPerSecondBonus.Get(skillLevel) * 100).ToString());
            tip.Replace("{MANAPERCENTPERSECONDBONUS}", Mathf.RoundToInt(manaPercentPerSecondBonus.Get(skillLevel) * 100).ToString());
            tip.Replace("{SPEEDBONUS}", speedBonus.Get(skillLevel).ToString("F2"));
            return tip.ToString();
        }
    }
}
#endif