using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // COST - CLASS
    [System.Serializable]
    public class Tools_Reward
    {
        [Tooltip("[Optional] These items will be removed from players inventory")]
        public Tools_ItemRequirement[] itemReward;
        public long goldReward = 0;
        public long coinReward = 0;
        public int healthReward = 0;
        public int manaReward = 0;
    #if _iMMOSTAMINA
        public int staminaReward = 0;
    #endif
        public int experienceReward = 0;
        public int skillExperienceReward = 0;
    
    #if _iMMOHONORSHOP
        [Tooltip("[Optional] Honor Currency costs to use the teleporter")]
        public HonorShopCurrencyDrop[] honorCurrencyReward;
    #endif
        [Tooltip("[Optional] Show a message in the chat window")]
        public bool showChatMessage = true;
        [Tooltip("[Optional] Check if there is enough space in the inventory")]
        public bool checkSpaceInventory = true;
    
        [Header("# Custom message #")]
        public bool editMessage = false;
        [BoolShowConditional(conditionFieldName: "editMessage", conditionValue: true)]
        public string chatMessageGold = "You received: {0} Gold(s)";
        [BoolShowConditional(conditionFieldName: "editMessage", conditionValue: true)]
        public string chatMessageCoin = "You received: {0} Coin(s)";
        [BoolShowConditional(conditionFieldName: "editMessage", conditionValue: true)]
        public string chatMessageHealth = "You received: {0} Health";
        [BoolShowConditional(conditionFieldName: "editMessage", conditionValue: true)]
        public string chatMessageMana = "You received: {0} Mana";
        [BoolShowConditional(conditionFieldName: "editMessage", conditionValue: true)]
        public string chatMessageStamina = "You received: {0} Stamina";
        [BoolShowConditional(conditionFieldName: "editMessage", conditionValue: true)]
        public string chatMessageExperience = "You received: {0} Experience";
        [BoolShowConditional(conditionFieldName: "editMessage", conditionValue: true)]
        public string chatMessageSkillExperience = "You received: {0} Skill Experience";
        [BoolShowConditional(conditionFieldName: "editMessage", conditionValue: true)]
        public string chatMessageItem = "You received: {0} {1}";
    #if _iMMOHONORSHOP
        [BoolShowConditional(conditionFieldName: "editMessage", conditionValue: true)]
        public string chatMessageHonorCurrency = "You received: {0} {1}";
        #endif
    
        // -----------------------------------------------------------------------------------
        // checkCost
        // -----------------------------------------------------------------------------------
        public bool checkInventorySpace(Player player)
        {
            if (!player) return false;
    
            bool valid = true;
    
            return valid;
        }
    
        // -----------------------------------------------------------------------------------
        // payCost
        // -----------------------------------------------------------------------------------
        public void GiveReward(Player player, int bonusPercent = 0)
        {
            //if (!player || !checkCost(player)) return;
            player.Tools_AddItems(player, itemReward);
    
            if (goldReward > 0)
            {
                long golRewardCalc = goldReward + ((bonusPercent > 0) ? ((goldReward * bonusPercent) / 100) : 0);
                player.gold += golRewardCalc;
                if(showChatMessage)
                    player.Tools_TargetAddMessage(string.Format(chatMessageGold, golRewardCalc));
            }
            if (coinReward > 0)
            {
                long coinRewardCalc = coinReward + ((bonusPercent > 0) ? ((coinReward * bonusPercent) / 100) : 0);
                player.itemMall.coins += coinReward;
                if(showChatMessage)
                    player.Tools_TargetAddMessage(string.Format(chatMessageCoin, coinRewardCalc));
            }
            if(healthReward > 0)
                player.health.current += healthReward;
            if (manaReward > 0)
                player.mana.current += manaReward;
    #if _iMMOSTAMINA
            if (staminaReward > 0)
                player.stamina.current += staminaReward;
    #endif
            if (experienceReward > 0)
            {
                long experienceRewardCalc = experienceReward + ((bonusPercent > 0) ? ((experienceReward * bonusPercent) / 100) : 0);
                player.experience.current += experienceReward;
            }
            if (skillExperienceReward > 0)
            {
    
                long skillExperienceRewardCalc = skillExperienceReward + ((bonusPercent > 0) ? ((skillExperienceReward * bonusPercent) / 100) : 0);
                ((PlayerSkills)player.skills).skillExperience += skillExperienceReward;
            }
    
    #if _iMMOHONORSHOP
            player.playerHonorShop.GiveHonorCurrency(honorCurrencyReward, bonusPercent);
    #endif
        }
    }
    
}
    #endif
