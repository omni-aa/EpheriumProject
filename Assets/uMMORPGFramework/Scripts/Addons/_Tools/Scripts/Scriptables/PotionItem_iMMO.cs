using System.Text;
using UnityEngine;

namespace uMMORPG
{
    
    [CreateAssetMenu(menuName="MMO-Indie/Complete Items/Item Potion", order=999)]
    public class PotionItem_iMMO : UsableItem
    {
        [Header("Potion")]
        public int usageHealth;
        public int usageMana;
     #if _iMMOSTAMINA
        public int usageStamina;
    #endif 
        public int usageExperience;
        public int usagePetHealth; // to heal pet
    
        // usage
        public override void Use(Player player, int inventoryIndex)
        {
            // always call base function too
            base.Use(player, inventoryIndex);
    
            // increase health/mana/etc.
            player.health.current += usageHealth;
            player.mana.current += usageMana;
    #if _iMMOSTAMINA
            player.stamina.current 		+= usageStamina;
    #endif 
            player.experience.current 	+= usageExperience;
            if (player.petControl.activePet != null)
                player.petControl.activePet.health.current += usagePetHealth;
    
            // decrease amount
            ItemSlot slot = player.inventory.slots[inventoryIndex];
            slot.DecreaseAmount(1);
            player.inventory.slots[inventoryIndex] = slot;
        }
    
        // tooltip
        public override string ToolTip()
        {
            StringBuilder tip = new StringBuilder(base.ToolTip());
            tip.Replace("{USAGEHEALTH}", usageHealth.ToString());
            tip.Replace("{USAGEMANA}", usageMana.ToString());
    #if _iMMOSTAMINA
            tip.Replace("{USAGESTAMINA}", usageStamina.ToString());
    #endif 
            tip.Replace("{USAGEEXPERIENCE}", usageExperience.ToString());
            tip.Replace("{USAGEPETHEALTH}", usagePetHealth.ToString());
            return tip.ToString();
        }
    }
    
}
