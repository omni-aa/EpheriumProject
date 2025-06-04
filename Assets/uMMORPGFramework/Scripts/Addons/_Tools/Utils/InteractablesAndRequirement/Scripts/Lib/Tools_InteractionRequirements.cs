using Mirror;
using System.Linq;
using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // INTERACTION REQUIREMENTS CLASS
    // THIS CLASS IS PRIMARILY FOR OBJECTS THE PLAYER CAN CHOOSE TO INTERACT WITH
    
    [System.Serializable]
    public partial class Tools_InteractionRequirements : Tools_Requirements
    {
        [Header("[-=-[ COSTS (Removed after checking Requirements) ]-=-]")]
        public Tools_Cost requierementCost;
    
        [Header("[-=-[ REWARDS (awarded after checks & costs (repetitive)) ]-=-]")]
        public Tools_Reward requierementReward;
        public int expRewardMin                     = 0;
        public int expRewardMax                     = 0;
        public int skillExpRewardMin                = 0;
        public int skillExpRewardMax                = 0;
    
        [Header("[-=-[ Chat Messages (automatic activation only) ]-=-]")]
    #pragma warning disable
        [SerializeField] private bool editChatMessage = false;
    #pragma warning restore
        [BoolShowConditional("editChatMessage", true)]
        public string labelMinLevel                 = " Minimum Level: ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelMaxLevel                 = " Maximum Level: ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelMinHealth                = " Min. Health Percent: ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelMinMana = " Min. Mana Percent: ";
    #if _iMMOSTAMINA
        [BoolShowConditional("editChatMessage", true)]
        public string labelMinStamina = " Min. Stamina Percent: ";
    #endif
        [BoolShowConditional("editChatMessage", true)]
        public string labelDayStart                 = " Start Day: ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelDayEnd                   = " End Day: ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelActiveMonth              = " Active Month: ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelRequiredSkills           = " Skill(s): ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelLevel                    = "LV";
        [BoolShowConditional("editChatMessage", true)]
        public string labelAllowedClasses           = " Allowed Class(es): ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelRequiresGuild            = " Guild Membership";
        [BoolShowConditional("editChatMessage", true)]
        public string labelRequiresParty            = " Party Membership";
    #if _iMMOPRESTIGECLASSES
        [BoolShowConditional("editChatMessage", true)]
        public string labelAllowedPrestigeClasses   = " Allowed Prestige Class(es): ";
    #endif
    #if _iMMOPVP
        [BoolShowConditional("editChatMessage", true)]
        public string labelRequiresRealm            = " Limited to Specific Realm";
    #endif
        [BoolShowConditional("editChatMessage", true)]
        public string labelRequiresQuest            = " Quest: ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelInProgressQuest          = "[Must be in progress]";
    #if _iMMOFACTIONS
        [BoolShowConditional("editChatMessage", true)]
        public string labelFactionRequirements      = " Faction Ratings: ";
    #endif
        [BoolShowConditional("editChatMessage", true)]
        public string labelRequiredEquipment        = " Equipment: ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelRequiredItems            = " Item(s): ";
        [BoolShowConditional("editChatMessage", true)]
        public string labelDestroyItem              = "[Destroyed on use]";
    #if _iMMOHARVESTING
        [BoolShowConditional("editChatMessage", true)]
        public string requiredHarvestProfessions    = " Harvesting Profession(s): ";
    #endif
    #if _iMMOCRAFTING
        [BoolShowConditional("editChatMessage", true)]
        public string requiredCraftProfessions      = " Craft Profession(s): ";
    #endif
    #if _iMMOMOUNTS
        [BoolShowConditional("editChatMessage", true)]
        public string labelMountedOnly              = " Accessible only while mounted.";
        [BoolShowConditional("editChatMessage", true)]
        public string labelUnmountedOnly            = " Accessible only while unmounted.";
    #endif
    #if _iMMOTRAVEL
        [BoolShowConditional("editChatMessage", true)]
        public string labelTravelroute              = " Travelroute: ";
    #endif
    #if _iMMOWORLDEVENTS
        [BoolShowConditional("editChatMessage", true)]
        public string labelWorldEvent               = " World Event: ";
    #endif
    #if _iMMOGUILDUPGRADES
        [BoolShowConditional("editChatMessage", true)]
        public string labelGuildUpgrades            = " Guild Level: ";
    #endif
    #if _iMMOACCOUNTUNLOCKABLES
        [BoolShowConditional("editChatMessage", true)]
        public string labelAccountUnlockable        = " Account Unlockable: ";
    #endif
    #if _iMMOPATREON
        [BoolShowConditional("editChatMessage", true)]
        public string labelPatreonSubscription      = " - Requires active Patreon subscription.";
    #endif
        [Header("Error Chat message")]
        [Tooltip("You will need to enter the SlotChatMessage, if you are using CompleteChat make sure you have a Slot for this chat")]
        public ChannelInfo requires = new ChannelInfo("", "(Requires)", "(Requires)", null);
    
        // -----------------------------------------------------------------------------------
        // checkRequirements
        // -----------------------------------------------------------------------------------
        public override bool checkRequirements(Player player)
        {
            bool valid = base.checkRequirements(player);
            valid = requierementCost.CheckCost(player) && valid;
    
            return valid;
        }
    #if _SERVER
        // -----------------------------------------------------------------------------------
        // GrantRewards
        // -----------------------------------------------------------------------------------
        [Server]
        public void GrantRewards(Player player)
        {
            if (checkRequirements(player))
            {
                player.experience.current += Random.Range(expRewardMin, expRewardMax);
                ((PlayerSkills)player.skills).skillExperience += Random.Range(skillExpRewardMin, skillExpRewardMax);
            }
        }
    #endif
    
        // -----------------------------------------------------------------------------------
        // Update Required Chat (Auto Activation)
        // @Client
        // -----------------------------------------------------------------------------------
        public virtual void UpdateRequirementChat()
        {
            Player player = Player.localPlayer;
            if (!player) return;
    
            // -- Requirements
    
            if (minLevel > 0)
                SendMessageChat("", requires.identifierIn, labelMinLevel + minLevel.ToString(), "", requires.textPrefab);
    
            if (maxLevel > 0)
                SendMessageChat("", requires.identifierIn, labelMaxLevel + maxLevel.ToString(), "", requires.textPrefab);
    
            if (minHealth > 0)
                SendMessageChat("", requires.identifierIn, labelMinHealth + minHealth.ToString(), "", requires.textPrefab);
    
            if (minMana > 0)
                SendMessageChat("", requires.identifierIn, labelMinMana + minMana.ToString(), "", requires.textPrefab);
    #if _iMMOSTAMINA
            if (minStamina > 0)
                SendMessageChat("", requires.identifierIn, labelMinStamina + minStamina.ToString(), "", requires.textPrefab);
    #endif
            // TIME
            if (dayStart > 0)
                SendMessageChat("", requires.identifierIn, labelDayStart + dayStart.ToString(), "", requires.textPrefab);
    
            if (dayEnd > 0)
                SendMessageChat("", requires.identifierIn, labelDayEnd + dayEnd.ToString(), "", requires.textPrefab);
    
            if (activeMonth > 0)
                SendMessageChat("", requires.identifierIn, labelActiveMonth + activeMonth.ToString(), "", requires.textPrefab);
    
            if (requiredSkills.Length > 0)
            {
                foreach (Tools_SkillRequirement skill in requiredSkills)
                    SendMessageChat("", requires.identifierIn, labelRequiredSkills + skill.skill.name + labelLevel + skill.level.ToString(), "", requires.textPrefab);
            }
    
            // CLASS
    
            if (allowedClasses.Length > 0)
            {
                string temp_classes = "";
                foreach (Player classes in allowedClasses)
                    temp_classes += " " + classes.name;
                SendMessageChat("", requires.identifierIn, labelAllowedClasses + temp_classes, "", requires.textPrefab);
            }
    
            // PARTY & GUILD
    
            if (requiresParty)
                SendMessageChat("", requires.identifierIn, labelRequiresParty, "", requires.textPrefab);
    
            if (requiresGuild)
                SendMessageChat("", requires.identifierIn, labelRequiresGuild, "", requires.textPrefab);
    
    #if _iMMOPRESTIGECLASSES
            if (allowedPrestigeClasses.Length > 0)
            {
                string temp_classes = "";
                foreach (Tmpl_PrestigeClass classes in allowedPrestigeClasses)
                    temp_classes += " " + classes.name;
                SendMessageChat("", requires.identifierIn, labelAllowedPrestigeClasses + temp_classes, "", requires.textPrefab);
            }
    #endif
    
    #if _iMMOPVP
            if (requiredRealm != null && requiredAlly != null)
                SendMessageChat("", requires.identifierIn, labelRequiresRealm, "", requires.textPrefab);
    #endif
    
    #if _iMMOQUESTS
            if (requiredQuest != null)
            {
                if (!questMustBeInProgress)
                {
                    SendMessageChat("", requires.identifierIn, labelRequiresQuest + requiredQuest.name, "", requires.textPrefab);
                }
                else
                {
                    SendMessageChat("", requires.identifierIn, labelRequiresQuest + requiredQuest.name + labelInProgressQuest, "", requires.textPrefab);
                }
            }
    #else
            if (requiredQuest != null)
                SendMessageChat("", requires.identifierIn, labelRequiresQuest + requiredQuest.name, "", requires.textPrefab);
    #endif
    
    #if _iMMOFACTIONS
            if (factionRequirements.Length > 0)
            {
                foreach (FactionRequirement factionRequirement in factionRequirements)
                    SendMessageChat("", requires.identifierIn, labelFactionRequirements + factionRequirement.faction.name, "", requires.textPrefab);
            }
    #endif
    
            if (requiredEquipment.Length > 0)
            {
                foreach (EquipmentItem item in requiredEquipment)
                    SendMessageChat("", requires.identifierIn, labelRequiredEquipment + item.name, "", requires.textPrefab);
            }
    
            if (requiredItems.Length > 0)
            {
                foreach (Tools_ItemRequirement item in requiredItems)
                    SendMessageChat("", requires.identifierIn, labelRequiredItems + item.item.name, "", requires.textPrefab);
            }
    
    #if _iMMOHARVESTING
            if (harvestProfessionRequirements.Length > 0)
            {
                foreach (HarvestingProfessionRequirement prof in harvestProfessionRequirements)
                {
                    if (requires.textPrefab != null)
                        SendMessageChat("", requires.identifierIn, requiredHarvestProfessions + prof.template.name + " " + labelLevel + prof.level, "", requires.textPrefab);
                }
    
    
            }
    #endif
    
    #if _iMMOCRAFTING
            if (craftProfessionRequirements.Length > 0)
            {
                foreach (Tools_CraftingProfessionRequirement prof in craftProfessionRequirements)
                    SendMessageChat("", requires.identifierIn, requiredCraftProfessions + prof.template.name + " " + labelLevel + prof.level, "", requires.textPrefab);
            }
    #endif
    
    #if _iMMOMOUNTS
            if (mountType == Tools_Requirements.MountType.Mounted)
            {
                SendMessageChat("", requires.identifierIn, labelMountedOnly, "", requires.textPrefab);
            }
            else if (mountType == Tools_Requirements.MountType.Unmounted)
            {
                SendMessageChat("", requires.identifierIn, labelUnmountedOnly, "", requires.textPrefab);
            }
    #endif
    
    #if _iMMOTRAVEL
            if (!string.IsNullOrWhiteSpace(requiredTravelrouteName))
            {
                SendMessageChat("", requires.identifierIn, labelTravelroute + requiredTravelrouteName, "", requires.textPrefab);
            }
    #endif
    
    #if _iMMOWORLDEVENTS
            if (worldEvent != null)
            {
                if (player.playerAddonsConfigurator.CheckWorldEvent(worldEvent, minEventCount, maxEventCount))
                {
                    if (maxEventCount == 0)
                        SendMessageChat("", requires.identifierIn, labelWorldEvent + worldEvent.name + " (" + player.playerAddonsConfigurator.GetWorldEventCount(worldEvent) + "/" + minEventCount.ToString() + ")", "", requires.textPrefab);
                    else
                        SendMessageChat("", requires.identifierIn, labelWorldEvent + worldEvent.name + " (" + minEventCount.ToString() + "-" + maxEventCount.ToString() + ") [" + player.playerAddonsConfigurator.GetWorldEventCount(worldEvent) + "]", "", requires.textPrefab);
                }
                else
                {
                    if (maxEventCount == 0)
                        SendMessageChat("", requires.identifierIn, labelWorldEvent + worldEvent.name + " (" + player.playerAddonsConfigurator.GetWorldEventCount(worldEvent) + "/" + minEventCount.ToString() + ")", "", requires.textPrefab);
                    else
                        SendMessageChat("", requires.identifierIn, labelWorldEvent + worldEvent.name + " (" + minEventCount.ToString() + "-" + maxEventCount.ToString() + ") [" + player.playerAddonsConfigurator.GetWorldEventCount(worldEvent) + "]", "", requires.textPrefab);
                }
            }
    #endif
    
    #if _iMMOGUILDUPGRADES
            if (minGuildLevel > 0)
            {
                if (player.guild.InGuild())
                    SendMessageChat("", requires.identifierIn, labelGuildUpgrades + player.playerAddonsConfigurator.guildLevel.ToString() + "/" + minGuildLevel.ToString(), "", requires.textPrefab);
                else
                    SendMessageChat("", requires.identifierIn, labelGuildUpgrades + "0/" + minGuildLevel.ToString(), "", requires.textPrefab);
            }
    #endif
    
    #if _iMMOACCOUNTUNLOCKABLES
            if (accountUnlockable != null)
            {
                if (player.playerAddonsConfigurator.HasAccountUnlock(accountUnlockable.name))
                    SendMessageChat("", requires.identifierIn, labelAccountUnlockable + accountUnlockable, "", requires.textPrefab);
                else
                    SendMessageChat("", requires.identifierIn, labelAccountUnlockable + accountUnlockable, "", requires.textPrefab);
            }
    #endif
    
    #if _iMMOPATREON
            if (activeMinPatreon > 0)
            {
                if (player.playerPatreonManager.HasActivePatreonSubscription(activeMinPatreon))
                   SendMessageChat("", requires.identifierIn, labelPatreonSubscription, "", requires.textPrefab);
                else
                    SendMessageChat("", requires.identifierIn, labelPatreonSubscription, "", requires.textPrefab);
            }
    #endif
    
        }
    
        private void SendMessageChat(string sender, string identifier, string message, string replyPrefix, GameObject textPrefab)
        {
            if (requires.textPrefab != null)
            {
    #if _iMMOCOMPLETECHAT
                UICompleteChat.singleton.AddMessage(new ChatMessage(sender, identifier, message, replyPrefix, textPrefab));
    #else
                UIChat.singleton.AddMessage(new ChatMessage(sender, identifier, message, replyPrefix, textPrefab));
    #endif
            }
            else
            {
                GameLog.LogWarning("Warning" + this +" does not contain a slotmessage so it cannot be displayed on the chat!");
            }
        }
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
