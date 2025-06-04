using System;
using UnityEngine;
using System.Linq;

namespace uMMORPG
{
    
    // TemplateGameRules
    
    [CreateAssetMenu(menuName = "MMO-Indie/Tools/Other/GameRules", fileName = "GameRules")]
    public partial class TemplateGameRules : ScriptableObject
    {
    
    #if _iMMOATTRIBUTES
        [Header("Damage Formula")]
        [Tooltip("[Optional] All damage dealt can vary randomly (0.25 = +/- 25%) (0 to disable)")]
        [Range(0, 1)] public float randomDamageDeviation = 0.25f;
    
        [Tooltip("[Optional] Check to use new relational damage formula, uncheck to use old (attack-defense=damage) formula.")]
        public bool relationalDamage = true;
    #else
        [Header("First : Install Attribute")]
        [Tooltip("[Optional] Game rule is based on varition damage, for this need install addon Attribute")]
        public bool read = false;
    #endif
    
        static TemplateGameRules _instance;
    
        // -----------------------------------------------------------------------------------
        // singleton
        // -----------------------------------------------------------------------------------
        public static TemplateGameRules singleton
        {
            get
            {
                if (!_instance)
                    //_instance = SGResources.FindObjectsOfTypeAll<TemplateGameRules>().FirstOrDefault();
                    _instance = Resources.FindObjectsOfTypeAll<TemplateGameRules>().FirstOrDefault();
                return _instance;
            }
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
