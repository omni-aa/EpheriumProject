using System;
using System.Linq;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // REQUIREMENTS CLASS
    // BASE CLASS FOR ANY KIND OF REQUIREMENTS - can be used for all kinds of requirement checks
    
    [System.Serializable]
    public partial class Tools_Requirements
    {
        [Header("[-=-[ REQUIREMENTS (Checked before interaction allowed) ]-=-]")]
    
        [Tooltip("[Optional] One click deactivation (ignores all requirements)")]
        public bool isActive = true;
    
        [Header("[STATUS & LEVEL]")]
        [Tooltip("[Optional] Player must be alive (health > 0)?")]
        public bool aliveOnly = true;
    
        public enum StateType { Any, Idle, Casting, Moving, IdleOrCasting, IdleOrMoving, IdleOrCastingOrMoving, Dead }
    
        [Tooltip("[Optional] Player state limit?")]
        public StateType stateType;
    
        [Tooltip("[Optional] Minimum player level to interact (0 to disable)")]
        public int minLevel = 0;
    
        [Tooltip("[Optional] Maximum player level to interact (0 to disable)")]
        public int maxLevel = 0;
    
        [Tooltip("[Optional] Health has to be equal or higher (0 to disable, 0.5=50%, 1.0=100%)")]
        [Range(0, 1)] public float minHealth;
    
        [Tooltip("[Optional] Mana has to be equal or higher (0 to disable, 0.5=50%, 1.0=100%)")]
        [Range(0, 1)] public float minMana;
#if _iMMOSTAMINA
        [Tooltip("[Optional] Stamina has to be equal or higher (0 to disable, 0.5=50%, 1.0=100%)")]
        [Range(0, 1)] public float minStamina;
#endif
        [Header("[TIME]")]
        [Tooltip("[Optional] To access, must be this day or later in the month (0 to disable)"), Range(0, 31)]
        public int dayStart = 0;
        [Tooltip("[Optional] To access, must be this day or earlier in the month (0 to disable)"), Range(0, 31)]
        public int dayEnd = 0;
        [Tooltip("[Optional]  To access, must be this month (0 to disable)"), Range(0, 12)]
        public int activeMonth = 0;
    
        [Header("[Combat]")]
        public bool cannotUseInCombat = false;
        public int waitSecondsAfterCombat = 10;
    
    
        [Header("[SKILLS & BUFFS]")]
        [Tooltip("[Optional] Required Skills")]
        public Tools_SkillRequirement[] requiredSkills;
    
        [Tooltip("[Optional] All Skills are required (otherwise any one of them is enough)")]
        public bool requiresAllSkills;
    
        [Tooltip("[Optional] Required active Buff (this buff must be active)")]
        public BuffSkill requiredBuff;
    
        [Tooltip("[Optional] Prohibited active Buff (this buff may not be active)")]
        public BuffSkill prohibitedBuff;
    
        [Header("[CLASSES, PARTY & GUILD]")]
        [Tooltip("[Optional] Allowed Classes (Player Prefab)")]
        public Player[] allowedClasses;
    
        [Tooltip("[Optional] Must be in a party (any) ?")]
        public bool requiresParty = false;
    
        [Tooltip("[Optional] Must be in a guild (any) ?")]
        public bool requiresGuild = false;
    
        [Header("[EQUIPMENT ITEMS]")]
        [Tooltip("[Optional] This item must be in the players equipment")]
        public EquipmentItem[] requiredEquipment;
    
        [Tooltip("[Optional] All Items are required (otherwise any one of them is enough)")]
        public bool requiresAllEquipment;
    
        [Header("[INVENTORY ITEMS]")]
        [Tooltip("[Optional] This item must be in the players inventory")]
        public Tools_ItemRequirement[] requiredItems;
    
        [Tooltip("[Optional] All Equipment items are required (otherwise any one of them is enough)")]
        public bool requiresAllItems;
    
    
    
#if _iMMOPRESTIGECLASSES
        [Header("[PRESTIGE CLASSES REQUIREMENTS]")]
        public Tmpl_PrestigeClass[] allowedPrestigeClasses;
#endif
    
#if _iMMOATTRIBUTES
        [Header("[ATTRIBUTES REQUIREMENTS]")]
        public Tools_AttributeRequirement[] requiredAttributes;
        public bool requiresAllAttributes;
#endif
    
#if _iMMOPVP
    
        public enum LootableBy { MyRealmAndAllies, NotMyRealmAndAllies }
    
        [Header("[PVP REQUIREMENTS]")]
        [Tooltip("[Optional] Allowed Realms")]
        public Tmpl_Realm requiredRealm;
        public Tmpl_Realm requiredAlly;
    
        [Tooltip("[Optional] Accessible by all, allied or enemy Realms?")]
        public LootableBy lootableBy;
#endif
    
        [Header("[QUEST REQUIREMENTS]")]
        [Tooltip("[Optional] This quest must be completed first")]
#if _iMMOQUESTS
        public Scriptable_Quest requiredQuest;
        public bool questMustBeInProgress;
#else
        public ScriptableQuest requiredQuest;
#endif
    
#if _iMMOFACTIONS
        [Header("[FACTIONS REQUIREMENTS]")]
        [Tooltip("[Optional] Faction Requirements")]
        public FactionRequirement[] factionRequirements;
        public bool requiresAllFactionRatings;
#endif
    
#if _iMMOHARVESTING
        [Header("[HARVESTING REQUIREMENTS]")]
        public HarvestingProfessionRequirement[] harvestProfessionRequirements;
        public bool requiresAllHarvestingProfessions;
#endif
    
#if _iMMOCRAFTING
        [Header("[CRAFTING REQUIREMENTS]")]
        public Tools_CraftingProfessionRequirement[] craftProfessionRequirements;
        public bool requiresAllCraftingProfessions;
#endif
    
#if _iMMOMOUNTS
    
        public enum MountType { Both, Unmounted, Mounted };
    
        [Header("[MOUNTS REQUIREMENTS]")]
        [Tooltip("[Optional] Mounts - interactable while mounted or not?")]
        public MountType mountType;
#endif
    
#if _iMMOTRAVEL
    	[Header("[TRAVELROUTE REQUIREMENTS]")]
    	public string requiredTravelrouteName;
#endif
    
#if _iMMOWORLDEVENTS
    	[Header("[WORLD EVENT REQUIREMENTS]")]
    	[Tooltip("[Optional] This world event will be checked")]
    	public WorldEventTemplate worldEvent;
    	[Tooltip("[Optional] Min count the world event has been completed (0 to disable)")]
    	public int minEventCount;
    	[Tooltip("[Optional] Max count the world event has been completed (0 to disable)")]
    	public int maxEventCount;
#endif
    
#if _iMMOGUILDUPGRADES
    	[Header("[GUILD UPGRADES]")]
    	[Tooltip("[Optional] Players guild has to be of this level (player has to be in a guild)")]
    	public int minGuildLevel;
#endif
    
#if _iMMOACCOUNTUNLOCKABLES
    	[Header("[ACCOUNT UNLOCKABLES]")]
    	[Tooltip("[Optional] This must be unlocked on the account (empty to disable)")]
    	public Item_AccountUnlockable accountUnlockable;
#endif
    
#if _iMMOPATREON
        [Header("[PATREON]")]
        [Tooltip("[Optional] Requires an active patreon subscription")]
        public int activeMinPatreon;
#endif
    
        // -----------------------------------------------------------------------------------
        // checkRequirements
        // Runs a full check to see if all interaction requirements are met by the player
        // -----------------------------------------------------------------------------------
        public virtual bool checkRequirements(Player player)
        {
            if (!isActive) return true;
            if (player == null) return false;
    
            if (!hasRequirements()) return true;
    
            bool valid = true;
    
            valid = (!aliveOnly || aliveOnly && player.isAlive) ? valid : false;
            valid = checkState(player) ? valid : false;
            valid = (minLevel == 0 || player.level.current >= minLevel) ? valid : false;
            //Debug.Log(">>" + valid);
            valid = (maxLevel == 0 || player.level.current <= maxLevel) ? valid : false;
            //Debug.Log("<"+ maxLevel + "><"+ player.level.current + ">" + valid);
            valid = (minHealth == 0 || player.health.Percent() >= minHealth) ? valid : false;
            valid = (minMana == 0 || player.mana.Percent() >= minMana) ? valid : false;
    
            //TIME
            valid = (dayStart == 0 || dayStart <= DateTime.UtcNow.Day) ? valid : false;
            valid = (dayEnd == 0 || dayEnd >= DateTime.UtcNow.Day) ? valid : false;
            valid = (activeMonth == 0 || activeMonth == DateTime.UtcNow.Month) ? valid : false;
    
            //Combat
            valid = (!cannotUseInCombat || cannotUseInCombat && LastCombatEnd(player) ) ? valid : false; 
            
            valid = (requiredSkills.Length == 0 || player.Tools_checkHasSkills(requiredSkills, requiresAllSkills)) ? valid : false;
            valid = (requiredBuff == null || player.Tools_checkHasBuff(requiredBuff)) ? valid : false;
            valid = (prohibitedBuff == null || !player.Tools_checkHasBuff(prohibitedBuff)) ? valid : false;
            valid = (allowedClasses.Length == 0 || player.Tools_checkHasClass(allowedClasses)) ? valid : false;
            valid = (!requiresParty || player.party.InParty()) ? valid : false;
            valid = (!requiresGuild || player.guild.InGuild()) ? valid : false;
            valid = (requiredEquipment.Length == 0 || player.Tools_checkHasEquipment(requiredEquipment)) ? valid : false;
            valid = (requiredItems.Length == 0 || player.Tools_checkHasItems(player, requiredItems)) ? valid : false;
#if _iMMOPRESTIGECLASSES
    
            if (player.playerAddonsConfigurator)
                valid = player.playerAddonsConfigurator.CheckPrestigeClass(allowedPrestigeClasses) ? valid : false;
#endif
#if _iMMOATTRIBUTES
            valid = checkAttributes(player) ? valid : false;
#endif
#if _iMMOPVP
            valid = (checkRealm(player)) ? valid : false;
#endif
#if _iMMOQUESTS
            if (!questMustBeInProgress)
            {
                valid = (requiredQuest == null || player.playerExtendedQuest.HasCompletedQuest(requiredQuest.name)) ? valid : false;
            }
            else
            {
                valid = ((requiredQuest == null || (player.playerExtendedQuest.HasActiveQuest(requiredQuest.name) && !player.playerExtendedQuest.HasCompletedQuest(requiredQuest.name))) ? valid : false);
            }
#else
            valid = (requiredQuest == null || player.quests.HasCompleted(requiredQuest.name)) ? valid : false;
#endif
#if _iMMOFACTIONS
            if(player.playerFactions)
                valid = player.playerFactions.CheckFactionRatings(factionRequirements, requiresAllFactionRatings) ? valid : false;
#endif
#if _iMMOHARVESTING
            if (player.playerAddonsConfigurator)
                valid = player.playerAddonsConfigurator.HasHarvestingProfessions(harvestProfessionRequirements, requiresAllHarvestingProfessions) ? valid : false;
#endif
#if _iMMOCRAFTING
            if(player.playerCraftingExtended)
                valid = player.playerCraftingExtended.HasCraftingProfessions(craftProfessionRequirements, requiresAllCraftingProfessions) ? valid : false;
#endif
#if _iMMOMOUNTS
            valid = mountType == MountType.Both || (mountType == MountType.Unmounted && !player.playerExtentedMounts.isMounted) || (mountType == MountType.Mounted && !player.playerExtentedMounts.isMounted) ? valid : false;
#endif
    
#if _iMMOTRAVEL
    		valid = string.IsNullOrWhiteSpace(requiredTravelrouteName) || (!string.IsNullOrWhiteSpace(requiredTravelrouteName) && player.playerAddonsConfigurator.travelroutes.Any(t => t.name == requiredTravelrouteName)) ? valid : false;
#endif
    
#if _iMMOWORLDEVENTS
            if(player.playerAddonsConfigurator)
        		valid = player.playerAddonsConfigurator.CheckWorldEvent(worldEvent, minEventCount, maxEventCount) ? valid : false;
#endif
    
#if _iMMOGUILDUPGRADES
    		if (minGuildLevel > 0)
    			valid = player.guild.InGuild() && player.playerAddonsConfigurator.guildLevel >= minGuildLevel ? valid : false;
#endif
    
#if _iMMOACCOUNTUNLOCKABLES
    		if (player.playerAddonsConfigurator && accountUnlockable != null && !string.IsNullOrWhiteSpace(accountUnlockable.name))
    			valid = player.playerAddonsConfigurator.HasAccountUnlock(accountUnlockable.name) ? valid : false;
#endif
    
#if _iMMOPATREON
            if (player.playerPatreonManager && activeMinPatreon > 0)
                valid = player.playerPatreonManager.HasActivePatreonSubscription(activeMinPatreon) ? valid : false;
#endif
    
            return valid;
        }
    
        // -----------------------------------------------------------------------------------
        // checkState
        // -----------------------------------------------------------------------------------
        // Initialisation globale des états valides pour chaque type
        private static readonly Dictionary<StateType, HashSet<string>> StateMappings = new()
        {
            { StateType.Any, new HashSet<string> { "IDLE", "MOVING", "CASTING", "DEAD" } },
            { StateType.Idle, new HashSet<string> { "IDLE" } },
            { StateType.Casting, new HashSet<string> { "CASTING" } },
            { StateType.Moving, new HashSet<string> { "MOVING" } },
            { StateType.IdleOrCasting, new HashSet<string> { "IDLE", "CASTING" } },
            { StateType.IdleOrMoving, new HashSet<string> { "IDLE", "MOVING" } },
            { StateType.IdleOrCastingOrMoving, new HashSet<string> { "IDLE", "CASTING", "MOVING" } },
            { StateType.Dead, new HashSet<string> { "DEAD" } }
        };
    
        // Vérification de l'état du joueur
        public bool checkState(Player player)
        {
            if (StateMappings.TryGetValue(stateType, out var validStates))
            {
                return validStates.Contains(player.state);
            }
            return false; // Si le type d'état est invalide
        }
    
    
        // -----------------------------------------------------------------------------------
        // checkAttributes
        // -----------------------------------------------------------------------------------
        protected bool checkAttributes(Player player)
        {
#if _iMMOATTRIBUTES
            if (requiredAttributes.Length <= 0) return true;
    
            bool success = false;
    
            foreach (Tools_AttributeRequirement requirement in requiredAttributes)
            {
                if (requirement.template != null &&
                    (requirement.minValue <= 0 || player.playerAttribute.Attributes.FirstOrDefault(x => x.template == requirement.template).points >= requirement.minValue) &&
                    (requirement.maxValue <= 0 || player.playerAttribute.Attributes.FirstOrDefault(x => x.template == requirement.template).points <= requirement.maxValue)
                    )
                {
                    if (!requiresAllAttributes) return true;
                    success = true;
                }
                else
                {
                    success = false;
                }
            }
    
            return success;
#else
            return true;
#endif
        }
    
        // -----------------------------------------------------------------------------------
        // checkRealm
        // -----------------------------------------------------------------------------------
        public bool checkRealm(Player player)
        {
#if _iMMOPVP
            if ((requiredRealm == null && requiredAlly == null) || (player.Realm == null && player.Ally == null))
                return true;
    
            bool bValid = false;
    
            if (requiredRealm == player.Realm || requiredAlly == player.Ally || requiredRealm == player.Ally || requiredAlly == player.Realm)
                bValid = true;
    
            if (lootableBy == LootableBy.MyRealmAndAllies)
                return bValid;
    
            if (lootableBy == LootableBy.NotMyRealmAndAllies)
                return !bValid;
    
            return bValid;
#else
            return true;
#endif
        }
    
        // -----------------------------------------------------------------------------------
        // hasRequirements
        // -----------------------------------------------------------------------------------
        public bool hasRequirements()
        {
            if (!isActive) return false;
    
            return
                    aliveOnly ||
                    stateType != StateType.Any ||
                    minLevel > 0 ||
                    maxLevel > 0 ||
                    minHealth > 0 ||
                    minMana > 0 ||
                    cannotUseInCombat && waitSecondsAfterCombat > 0 ||
                    dayStart > 0 ||
                    dayEnd > 0 ||
                    activeMonth > 0 ||
                    requiredSkills.Length > 0 ||
                    requiredBuff != null ||
                    prohibitedBuff != null ||
                    allowedClasses.Length > 0 ||
                    requiresParty ||
                    requiresGuild ||
                    requiredItems.Length > 0 ||
                    requiredEquipment.Length > 0
#if _iMMOATTRIBUTES
                    || requiredAttributes.Length > 0
#endif
#if _iMMOPVP
                    || requiredRealm != null ||
                    requiredAlly != null
#endif
#if _iMMOQUESTS
                    || requiredQuest != null
#endif
#if _iMMOFACTIONS
                    || factionRequirements.Length > 0
#endif
#if _iMMOTRAVEL
    				|| !string.IsNullOrWhiteSpace(requiredTravelrouteName)
#endif
#if _iMMOWORLDEVENTS
    				|| worldEvent != null && (minEventCount != 0 || maxEventCount != 0)
#endif
#if _iMMOGUILDUPGRADES
    				|| minGuildLevel > 0
#endif
#if _iMMOPATREON
                    || activeMinPatreon > 0
#endif
                    ;
        }
    
    
        // -----------------------------------------------------------------------------------
        // LastCombatEnd
        // -----------------------------------------------------------------------------------
        public bool LastCombatEnd(Player player)
        {
            bool valid = false;
    
            double allowedTeleportTime = player.lastCombatTime + waitSecondsAfterCombat;
            double remainingTeleportTime = Mirror.NetworkTime.time < allowedTeleportTime ? (allowedTeleportTime - Mirror.NetworkTime.time) : 0;
            if (remainingTeleportTime <= 0)
            {
                valid = true;
            }
            return valid;
        }
        // -----------------------------------------------------------------------------------
    }
    
}
#endif
