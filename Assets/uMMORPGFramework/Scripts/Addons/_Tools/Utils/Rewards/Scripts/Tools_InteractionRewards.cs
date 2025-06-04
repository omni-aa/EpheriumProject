using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // ACCESS REWARDS CLASS
    
    [System.Serializable]
    public partial class Tools_InteractionRewards
    {
        [Header("[-=-[ INTERACTION REWARDS ]-=-]")]
        [Tooltip("[Optional] Items awarded after sucessful interaction")]
        public Tools_ItemReward[] items;
    
        [Tooltip("[Optional] Minimum Experience awarded after sucessful interaction")]
        public int minExperience;
    
        [Tooltip("[Optional] Maximum Experience awarded after sucessful interaction")]
        public int maxExperience;
    
        [Tooltip("[Optional] Minimum SkillExperience awarded after sucessful interaction")]
        public int minSkillExperience;
    
        [Tooltip("[Optional] Maximum SkillExperience awarded after sucessful interaction")]
        public int maxSkillExperience;
    
        [Tooltip("[Optional] Minimum Experience awarded after sucessful interaction")]
        public int minGold;
    
        [Tooltip("[Optional] Maximum Experience awarded after sucessful interaction")]
        public int maxGold;
    
        [Tooltip("[Optional] Minimum Experience awarded after sucessful interaction")]
        public int minCoins;
    
        [Tooltip("[Optional] Maximum Experience awarded after sucessful interaction")]
        public int maxCoins;
    
    #if _iMMOHONORSHOP
        [Tooltip("[Optional] Honor Currency rewarded")]
        public HonorShopCurrencyCost[] honorCurrencyReward;
    #endif
    
    #if _iMMOTRAVEL
        public Unlockroute[] unlockTravelroutes;
    #endif
    
        public string labelGainGold = "You got gold: ";
        public string labelGainCoins = "You got coins: ";
        public string labelGainExperience = "You got experience: ";
        public string labelGainSkillExperience = "You got skill experience: ";
        public string labelGainItem = "You got: ";
    
        // -----------------------------------------------------------------------------------
        // gainRewards
        // Server
        // -----------------------------------------------------------------------------------
        public void gainRewards(Player player)
        {
            int gold = UnityEngine.Random.Range(minGold, maxGold);
            if (gold > 0)
            {
                player.gold += gold;
                player.Tools_TargetAddMessage(labelGainGold + gold.ToString());
            }
    
            int coins = UnityEngine.Random.Range(minCoins, maxCoins);
            if (coins > 0)
            {
                player.itemMall.coins += coins;
                player.Tools_TargetAddMessage(labelGainCoins + coins.ToString());
            }
    
            int experience = UnityEngine.Random.Range(minExperience, maxExperience);
            if (experience > 0)
            {
                player.experience.current += experience;
                player.Tools_TargetAddMessage(labelGainExperience + experience.ToString());
            }
    
            int skillExperiences = UnityEngine.Random.Range(minSkillExperience, maxSkillExperience);
            if (skillExperiences > 0)
            {
                ((PlayerSkills)player.skills).skillExperience += skillExperiences;
                player.Tools_TargetAddMessage(labelGainSkillExperience + skillExperiences.ToString());
            }
    
            // -- reward honor currency
    #if _iMMOHONORSHOP
            foreach (HonorShopCurrencyCost currency in honorCurrencyReward)
            {
                player.playerHonorShop.AddHonorCurrency(currency.honorCurrency, currency.amount);
                player.Tools_TargetAddMessage("You got " + currency.amount.ToString()  + " honnor token");
            }
    #endif
    
            // -- unlock travelroutes
    #if _iMMOTRAVEL
            foreach (Unlockroute route in unlockTravelroutes)
            {
    #if _SERVER
                player.playerAddonsConfigurator.UnlockTravelroute(route);
    #endif
                player.Tools_TargetAddMessage("You got unlock route for " + route.ToString());
            }
    #endif
    
            // -- reward items
            foreach (Tools_ItemReward item in items)
            {
                if (UnityEngine.Random.value <= item.probability)
                {
                    if (player.inventory.CanAdd(new Item(item.item), item.amount))
                    {
                        player.inventory.Add(new Item(item.item), item.amount);
                        player.Tools_TargetAddMessage(labelGainItem + item.amount + " " + item.item.name );
                    }
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
