using System.Collections.Generic;
using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // COST - CLASS
    [System.Serializable]
    public class Tools_Cost
    {
        [Tooltip("[Optional] These items will be removed from players inventory")]
        public Tools_ItemRequirement[] itemCost;
        public long goldCost = 0;
        public long coinCost = 0;
        public int healthCost = 0;
        public int manaCost = 0;
    #if _iMMOSTAMINA
        public int staminaCost = 0;
    #endif
        public int experienceCost = 0;
        public int skillExperienceCost = 0;
    
    #if _iMMOHONORSHOP
        [Tooltip("[Optional] Honor Currency costs to use the teleporter")]
        public HonorShopCurrencyCost[] honorCurrencyCost;
    #endif
    
        // -----------------------------------------------------------------------------------
        // checkCost
        // -----------------------------------------------------------------------------------
        public bool CheckCost(Player player)
        {
            if (!player) return false;
    
            bool valid = true;
    
            valid = (itemCost.Length == 0 || player.Tools_checkHasItems(player, itemCost)) && valid;
            valid = (goldCost == 0 || player.gold >= goldCost) && valid;
            valid = (coinCost == 0 || player.itemMall.coins >= coinCost) && valid;
            valid = (healthCost == 0 || player.health.current > healthCost) && valid;
            valid = (manaCost == 0 || player.mana.current >= manaCost) && valid;
    #if _iMMOSTAMINA
            if(player.stamina)
                valid = (staminaCost == 0 || player.stamina.current >= staminaCost) && valid;
    #endif
            valid = (experienceCost == 0 || player.experience.current >= experienceCost) && valid;
            valid = (skillExperienceCost == 0 || ((PlayerSkills)player.skills).skillExperience >= skillExperienceCost) && valid;
    
    #if _iMMOHONORSHOP
            if(player.playerHonorShop)
                valid = player.playerHonorShop.CheckHonorCurrencyCost(honorCurrencyCost) && valid;
    #endif
    
            return valid;
        }
    
        // -----------------------------------------------------------------------------------
        // ListCost
        // -----------------------------------------------------------------------------------
        public List<ListCostValue> ListCost(Player player)
        {
            List<ListCostValue> listCost = new List<ListCostValue>();
            if (!player) return listCost;
            // Item
            if (itemCost.Length > 0)
            {
                foreach (Tools_ItemRequirement item in itemCost)
                {
                    ListCostValue listCostValue = new()
                    {
                        requireName = item.item.name,
                        amout = item.amount,
                        completed = (player.Tools_getTotalItemCount(player, item.item) >= item.amount)
                    };
                    listCost.Add(listCostValue);
                }
            }
            // Gold
            if (goldCost > 0)
            {
                ListCostValue listCostValue = new()
                {
                    requireName = "Gold",
                    amout = goldCost,
                    completed = player.gold >= goldCost
                };
                listCost.Add(listCostValue);
            }
            // Coin
            if (coinCost > 0)
            {
                ListCostValue listCostValue = new()
                {
                    requireName = "Coins",
                    amout = coinCost,
                    completed = player.itemMall.coins >= coinCost
                };
                listCost.Add(listCostValue);
            }
            // Health
            if (healthCost > 0)
            {
                ListCostValue listCostValue = new()
                {
                    requireName = "Health",
                    amout = healthCost,
                    completed = player.health.current >= healthCost
                };
                listCost.Add(listCostValue);
            }
            // Mana
            if (manaCost > 0)
            {
                ListCostValue listCostValue = new()
                {
                    requireName = "Mana",
                    amout = manaCost,
                    completed = player.mana.current >= manaCost
                };
                listCost.Add(listCostValue);
            }
    #if _iMMOSTAMINA
            // Stamina
            if (staminaCost > 0)
            {
                ListCostValue listCostValue = new()
                {
                    requireName = "Stamina",
                    amout = staminaCost,
                    completed = player.stamina.current >= staminaCost
                };
                listCost.Add(listCostValue);
            }
    #endif
            // Experience
            if(experienceCost > 0)
            {
                ListCostValue listCostValue = new()
                {
                    requireName = "Experience Cost",
                    amout = experienceCost,
                    completed = player.experience.current >= experienceCost
                };
                listCost.Add(listCostValue);
            }
            // Skill Experience
            if (skillExperienceCost > 0)
            {
                ListCostValue listCostValue = new()
                {
                    requireName = "Skill Experience Cost",
                    amout = skillExperienceCost,
                    completed = (((PlayerSkills)player.skills).skillExperience >= skillExperienceCost)
                };
                listCost.Add(listCostValue);
            }
    #if _iMMOHONORSHOP
            // HonorShop
            if (honorCurrencyCost.Length > 0)
            {
                foreach (HonorShopCurrencyCost honorCost in honorCurrencyCost)
                {
                    ListCostValue listCostValue = new()
                    {
                        requireName = honorCost.honorCurrency.name,
                        amout = honorCost.amount,
                        completed = honorCost.amount < player.playerHonorShop.GetHonorCurrency(honorCost.honorCurrency)
                    };
                    listCost.Add(listCostValue);
                }
            }
    #endif
            return listCost;
        }
    
        // -----------------------------------------------------------------------------------
        // hasCosts
        // -----------------------------------------------------------------------------------
        public bool HasCosts()
        {
            return itemCost.Length > 0 ||
                    goldCost > 0 ||
                    coinCost > 0 ||
                    healthCost > 0 ||
                    experienceCost > 0 ||
                    skillExperienceCost > 0 ||
                    manaCost > 0
    #if _iMMOSTAMINA
                    || staminaCost > 0
    #endif
    #if _iMMOHONORSHOP
                    || honorCurrencyCost.Length > 0
    #endif
                    ;
        }
    
        // -----------------------------------------------------------------------------------
        // payCost
        // -----------------------------------------------------------------------------------
        public void PayCost(Player player)
        {
            if (!player || !CheckCost(player)) return;
    
            player.Tools_removeItems(player, itemCost);
    
            if(goldCost > 0)
                player.gold -= goldCost;
            if(coinCost > 0)
                player.itemMall.coins -= coinCost;
            if(healthCost > 0)
                player.health.current -= healthCost;
            if (manaCost > 0)
                player.mana.current -= manaCost;
    #if _iMMOSTAMINA
            if (staminaCost > 0)
                player.stamina.current -= staminaCost;
    #endif
            if (experienceCost > 0)
                player.experience.current -= experienceCost;
            if(skillExperienceCost > 0)
                ((PlayerSkills)player.skills).skillExperience -= skillExperienceCost;
    
    #if _iMMOHONORSHOP
            player.playerHonorShop.PayHonorCurrencyCost(honorCurrencyCost);
    #endif
        }
    }
    
    public struct ListCostValue
    {
        public string requireName;
        public long amout;
        public bool completed;
    }
    
}
    #endif
