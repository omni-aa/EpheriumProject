using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // ===================================================================================
    // UI ACCESS REQUIREMENT
    // ===================================================================================
    public partial class Tools_UI_Requirement : MonoBehaviour
    {
        [Header("[-=-[ Required Assignments ]-=-]")]
        public GameObject panel;
        public Button interactButton;
        public Button cancelButton;
        public Transform content;
        public ScrollRect scrollRect;
        public GameObject textPrefab;
    
        [Header("[-=-[ Configureable Colors ]-=-]")]
        public Color headingColor;
        public Color textColor;
        public Color errorColor;
    
        [Space(20)]
    
        [Header("[-=-[ Configureable Labels ]-=-]")]
    #pragma warning disable
        [SerializeField] private bool editTextMessage = false;
    #pragma warning restore    
        [BoolShowConditional("editTextMessage", true)]
        public string labelHeading = "Interaction requirements:";
        [BoolShowConditional("editTextMessage", true)]
        public string labelMinLevel = " - Required minimum Level: ";
        [BoolShowConditional("editTextMessage", true)]
        public string labelMaxLevel = " - Required maximum level: ";
    
        [BoolShowConditional("editTextMessage", true)]
        public string labelMinHealth = " - Min. Health Percent: ";
        [BoolShowConditional("editTextMessage", true)]
        public string labelMinMana = " - Min. Mana Percent: ";
    
        [BoolShowConditional("editTextMessage", true)]
        public string labelDayStart = " - Start Day: ";
        [BoolShowConditional("editTextMessage", true)]
        public string labelDayEnd = " - End Day: ";
        [BoolShowConditional("editTextMessage", true)]
        public string labelActiveMonth = " - Active Month: ";
    
        [BoolShowConditional("editTextMessage", true)]
        public string labelRequiredSkills = " - Required Skill(s): ";
        [BoolShowConditional("editTextMessage", true)]
        public string labelLevel = "LV";
        [BoolShowConditional("editTextMessage", true)]
        public string labelAllowedClasses = " - Allowed Class(es): ";
        [BoolShowConditional("editTextMessage", true)]
        public string labelRequiresGuild = " - Requires guild membership.";
        [BoolShowConditional("editTextMessage", true)]
        public string labelRequiresParty = " - Requires party membership.";
    #if _iMMOPRESTIGECLASSES
        [BoolShowConditional("editTextMessage", true)]
        public string labelAllowedPrestigeClasses 		= " - Allowed Prestige Class(es): ";
    #endif
    #if _iMMOPVP
        [BoolShowConditional("editTextMessage", true)]
        public string labelRequiresRealm 				= " - Limited to specific Realm.";
    #endif
        [BoolShowConditional("editTextMessage", true)]
        public string labelRequiresQuest = " - Requires Quest: ";
        [BoolShowConditional("editTextMessage", true)]
        public string labelInProgressQuest = "[Must be in progress]";
    #if _iMMOFACTIONS
        [BoolShowConditional("editTextMessage", true)]
        public string labelFactionRequirements 			= " - Required faction ratings:";
    #endif
    
        [BoolShowConditional("editTextMessage", true)]
        public string labelRequiredEquipment = " - Required equipment: ";
        [BoolShowConditional("editTextMessage", true)]
        public string labelRequiredItems = " - Required item(s): ";
        [BoolShowConditional("editTextMessage", true)]
        public string labelDestroyItem = "[Destroyed on use]";
    #if _iMMOHARVESTING
        [BoolShowConditional("editTextMessage", true)]
        public string requiredHarvestProfessions 		= " - Requires Harvesting Profession(s):";
    #endif
    #if _iMMOCRAFTING
        [BoolShowConditional("editTextMessage", true)]
        public string requiredCraftProfessions 			= " - Requires Craft Profession(s):";
    #endif
    #if _iMMOMOUNTS
        [BoolShowConditional("editTextMessage", true)]
        public string labelMountedOnly 					= " - Accessible only while mounted.";
        [BoolShowConditional("editTextMessage", true)]
        public string labelUnmountedOnly 				= " - Accessible only while unmounted.";
    #endif
    #if _iMMOTRAVEL
        [BoolShowConditional("editTextMessage", true)]
        public string labelTravelroute					= " - Required Travelroute:";
    #endif
    #if _iMMOWORLDEVENTS
        [BoolShowConditional("editTextMessage", true)]
        public string labelWorldEvent 					= " - Required World Event:";
    #endif
    #if _iMMOGUILDUPGRADES
        [BoolShowConditional("editTextMessage", true)]
        public string labelGuildUpgrades 				= " - Required Guild Level:";
    #endif
    #if _iMMOACCOUNTUNLOCKABLES
        [BoolShowConditional("editTextMessage", true)]
        public string labelAccountUnlockable			= " - Required Account Unlockable:";
    #endif
    #if _iMMOPATREON
        [BoolShowConditional("editTextMessage", true)]
        public string labelPatreonSubscription          = " - Requires active Patreon subscription.";
    #endif
    
        protected Tools_Requirements requirements;
    
        // -----------------------------------------------------------------------------------
        // Show
        // -----------------------------------------------------------------------------------
        public virtual void Show(GameObject go)
        {
            Debug.Log("activation de la fenetre normalement..."+go.name);
            go.SetActive(true);
        }
    
        // -----------------------------------------------------------------------------------
        // UpdateTextbox
        // -----------------------------------------------------------------------------------
        protected virtual void UpdateTextbox()
        {
            Player player = Player.localPlayer;
            if (!player) return;
    
            AddMessage(labelHeading, headingColor);
    
            // -- Requirements
    
            if (requirements.minLevel > 0)
                AddMessage(labelMinLevel + requirements.minLevel.ToString(), player.level.current >= requirements.minLevel ? textColor : errorColor);
    
            if (requirements.maxLevel > 0)
                AddMessage(labelMaxLevel + requirements.maxLevel.ToString(), player.level.current <= requirements.maxLevel ? textColor : errorColor);
    
            if (requirements.minHealth > 0)
                AddMessage(labelMinHealth + requirements.minHealth.ToString(), player.health.Percent() >= requirements.minHealth ? textColor : errorColor);
    
            if (requirements.minMana > 0)
                AddMessage(labelMinMana + requirements.minMana.ToString(), player.mana.Percent() >= requirements.minMana ? textColor : errorColor);
    
            //TIME
            if (requirements.dayStart > 0)
                AddMessage(labelDayStart + requirements.dayStart.ToString(), requirements.dayStart <= DateTime.UtcNow.Day ? textColor : errorColor);
            if (requirements.dayEnd > 0)
                AddMessage(labelDayEnd + requirements.dayEnd.ToString(), requirements.dayEnd >= DateTime.UtcNow.Day ? textColor : errorColor);
            if (requirements.activeMonth > 0)
                AddMessage(labelActiveMonth + requirements.activeMonth.ToString(), requirements.activeMonth == DateTime.UtcNow.Month ? textColor : errorColor);
    
            if (requirements.requiredSkills.Length > 0)
            {
                AddMessage(labelRequiredSkills, textColor);
                foreach (Tools_SkillRequirement skill in requirements.requiredSkills)
                    AddMessage(skill.skill.name + labelLevel + skill.level.ToString(), player.Tools_checkHasSkill(skill.skill, skill.level) ? textColor : errorColor);
            }
    
            if (requirements.allowedClasses.Length > 0)
            {
                AddMessage(labelAllowedClasses, textColor);
                string temp_classes = "";
                foreach (Player classes in requirements.allowedClasses)
                    temp_classes += " " + classes.name;
                AddMessage(temp_classes, player.Tools_checkHasClass(requirements.allowedClasses) ? textColor : errorColor);
            }
    
            if (requirements.requiresParty)
                AddMessage(labelRequiresParty, player.party.InParty() ? textColor : errorColor);
    
            if (requirements.requiresGuild)
                AddMessage(labelRequiresGuild, player.guild.InGuild() ? textColor : errorColor);
    
    #if _iMMOPRESTIGECLASSES
            if (requirements.allowedPrestigeClasses.Length > 0)
            {
                AddMessage(labelAllowedPrestigeClasses, textColor);
                string temp_classes = "";
                foreach (Tmpl_PrestigeClass classes in requirements.allowedPrestigeClasses)
                    temp_classes += " " + classes.name;
                AddMessage(temp_classes, player.playerAddonsConfigurator.CheckPrestigeClass(requirements.allowedPrestigeClasses) ? textColor : errorColor);
            }
    #endif
    
    #if _iMMOPVP
            if (requirements.requiredRealm != null && requirements.requiredAlly != null)
                AddMessage(labelRequiresRealm, requirements.checkRealm(player) ? textColor : errorColor);
    #endif
    
    #if _iMMOQUESTS
            if (requirements.requiredQuest != null)
            {
                if (!requirements.questMustBeInProgress)
                {
                    //AddMessage(labelRequiresQuest + requirements.requiredQuest.name, player.HasCompletedQuest(requirements.requiredQuest.name) ? textColor : errorColor);
                    AddMessage(labelRequiresQuest + requirements.requiredQuest.name, player.playerExtendedQuest.HasCompletedQuest(requirements.requiredQuest.name) ? textColor : errorColor);
                }
                else
                {
                    //AddMessage(labelRequiresQuest + requirements.requiredQuest.name + labelInProgressQuest, player.HasActiveQuest(requirements.requiredQuest.name) ? textColor : errorColor);
                    AddMessage(labelRequiresQuest + requirements.requiredQuest.name + labelInProgressQuest, (player.playerExtendedQuest.HasActiveQuest(requirements.requiredQuest.name) && !player.playerExtendedQuest.HasCompletedQuest(requirements.requiredQuest.name)) ? textColor : errorColor);
                }
            }
    #else
            if (requirements.requiredQuest != null)
                AddMessage(labelRequiresQuest + requirements.requiredQuest.name, player.quests.HasCompleted(requirements.requiredQuest.name) ? textColor : errorColor);
    #endif
    
    #if _iMMOFACTIONS
            if (requirements.factionRequirements.Length > 0)
            {
                AddMessage(labelFactionRequirements, textColor);
                foreach (FactionRequirement factionRequirement in requirements.factionRequirements)
                    AddMessage(factionRequirement.faction.name +" require point > "+ factionRequirement.min + " and < " + factionRequirement.max +" !", player.playerFactions.CheckFactionRating(factionRequirement) ? textColor : errorColor);
            }
    #endif
    
            if (requirements.requiredEquipment.Length > 0)
            {
                AddMessage(labelRequiredEquipment, textColor);
    
                foreach (EquipmentItem item in requirements.requiredEquipment)
                {
                    AddMessage(item.name, player.Tools_checkHasEquipment(item) ? textColor : errorColor);
                }
            }
    
            if (requirements.requiredItems.Length > 0)
            {
                AddMessage(labelRequiredItems, textColor);
    
                foreach (Tools_ItemRequirement item in requirements.requiredItems)
                {
                    AddMessage(item.item.name + " x" + item.amount.ToString(), player.inventory.Count(new Item(item.item)) >= item.amount ? textColor : errorColor);
                }
            }
    
    #if _iMMOHARVESTING
            if (requirements.harvestProfessionRequirements.Length > 0)
            {
                AddMessage(requiredHarvestProfessions, textColor);
                foreach (HarvestingProfessionRequirement prof in requirements.harvestProfessionRequirements)
                {
                    AddMessage(prof.template.name + " " + labelLevel + prof.level, player.playerAddonsConfigurator.HasHarvestingProfessionLevel(prof.template, prof.level) ? textColor : errorColor);
                }
            }
    #endif
    
    #if _iMMOCRAFTING
            if (requirements.craftProfessionRequirements.Length > 0)
            {
                AddMessage(requiredCraftProfessions, textColor);
                foreach (Tools_CraftingProfessionRequirement prof in requirements.craftProfessionRequirements)
                {
                    AddMessage(prof.template.name + " " + labelLevel + prof.level, player.playerCraftingExtended.HasCraftingProfessionLevel(prof.template, prof.level) ? textColor : errorColor);
                }
            }
    #endif
    
    #if _iMMOMOUNTS
            if (requirements.mountType == Tools_Requirements.MountType.Mounted)
            {
                AddMessage(labelMountedOnly, (player.playerExtentedMounts.isMounted) ? textColor : errorColor);
            }
            else if (requirements.mountType == Tools_Requirements.MountType.Unmounted)
            {
                AddMessage(labelUnmountedOnly, (!player.playerExtentedMounts.isMounted) ? textColor : errorColor);
            }
    #endif
    
    #if _iMMOTRAVEL
    		if (!string.IsNullOrWhiteSpace(requirements.requiredTravelrouteName))
            {
                //AddMessage(labelTravelroute + requirements.requiredTravelrouteName, player.playerTravelroute.travelroutes.Any(t => t.name == requirements.requiredTravelrouteName) ? textColor : errorColor);
                AddMessage(labelTravelroute + requirements.requiredTravelrouteName, player.playerAddonsConfigurator.travelroutes.Any(t => t.name == requirements.requiredTravelrouteName) ? textColor : errorColor);
            }
    #endif
    
    #if _iMMOWORLDEVENTS
    		if (requirements.worldEvent != null)
    		{
    			AddMessage(labelWorldEvent, textColor);
    			if (player.playerAddonsConfigurator.CheckWorldEvent(requirements.worldEvent, requirements.minEventCount, requirements.maxEventCount))
    			{
    				if (requirements.maxEventCount == 0)
    					AddMessage(requirements.worldEvent.name + " (" + player.playerAddonsConfigurator.GetWorldEventCount(requirements.worldEvent) + "/" + requirements.minEventCount.ToString() + ")", textColor);
    				else
                		AddMessage(requirements.worldEvent.name + " (" + requirements.minEventCount.ToString() + "-" + requirements.maxEventCount.ToString() + ") [" + player.playerAddonsConfigurator.GetWorldEventCount(requirements.worldEvent) + "]", textColor);
                }
                else
                {
                	if (requirements.maxEventCount == 0)
    					AddMessage(requirements.worldEvent.name + " (" + player.playerAddonsConfigurator.GetWorldEventCount(requirements.worldEvent) + "/" + requirements.minEventCount.ToString() + ")", errorColor);
    				else
                		AddMessage(requirements.worldEvent.name + " (" + requirements.minEventCount.ToString() + "-" + requirements.maxEventCount.ToString() + ") [" + player.playerAddonsConfigurator.GetWorldEventCount(requirements.worldEvent) + "]", errorColor);
                }
    		}
    #endif
    
    #if _iMMOGUILDUPGRADES
    		if (requirements.minGuildLevel > 0)
    		{
    			if (player.guild.InGuild())
    				AddMessage(labelGuildUpgrades + player.playerAddonsConfigurator.guildLevel.ToString() + "/" + requirements.minGuildLevel.ToString(), textColor);
    			else
    				AddMessage(labelGuildUpgrades + "0/" + requirements.minGuildLevel.ToString(), errorColor);
    		}
    #endif
    
    #if _iMMOACCOUNTUNLOCKABLES
    		if (requirements.accountUnlockable != null && !string.IsNullOrWhiteSpace(requirements.accountUnlockable.name))
    		{
    			if (player.playerAddonsConfigurator.HasAccountUnlock(requirements.accountUnlockable.name))
    				AddMessage(labelAccountUnlockable + requirements.accountUnlockable, textColor);
    			else
    				AddMessage(labelAccountUnlockable + requirements.accountUnlockable, errorColor);
    		}
    #endif
    
    #if _iMMOPATREON
            if (requirements.activeMinPatreon > 0)
            {
                if (player.playerPatreonManager.HasActivePatreonSubscription(requirements.activeMinPatreon))
                    AddMessage(labelPatreonSubscription, textColor);
                else
                    AddMessage(labelPatreonSubscription, errorColor);
            }
    #endif
    
        }
    
        // -----------------------------------------------------------------------------------
        // Hide
        // -----------------------------------------------------------------------------------
        public void Hide()
        {
            panel.SetActive(false);
        }
    
        // -----------------------------------------------------------------------------------
        // Update
        // -----------------------------------------------------------------------------------
        /*protected virtual void Update()
        {
            if (gameObject.name == "UI_NpcAccessRequirement")
                Debug.Log("Shooowwww CACAO");
            if (!panel.activeSelf) return;
    
            Player player = Player.localPlayer;
            if (!player) return;
        }*/
    
        // -----------------------------------------------------------------------------------
        // AutoScroll
        // -----------------------------------------------------------------------------------
        protected void AutoScroll()
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0;
        }
    
        // -----------------------------------------------------------------------------------
        // AddMessage
        // -----------------------------------------------------------------------------------
        public void AddMessage(string msg, Color color)
        {
            GameObject go = Instantiate(textPrefab);
            go.transform.SetParent(content.transform, false);
            go.GetComponent<Text>().text = msg;
            go.GetComponent<Text>().color = color;
            AutoScroll();
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
