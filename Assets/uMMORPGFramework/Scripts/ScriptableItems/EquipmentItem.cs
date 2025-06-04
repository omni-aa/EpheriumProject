#if _iMMOTOOLS
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if _iMMOUMACHARACTERS
using UMA;
#endif

namespace uMMORPG
{
    [CreateAssetMenu(menuName = "uMMORPG Item/Equipment", order = 999)]
    public partial class EquipmentItem : UsableItem
    {
        [Header("Equipment")]
        public string category;
        public int healthBonus;
        public int manaBonus;
    #if _iMMOSTAMINA
        public int staminaBonus;
    #endif
        public int damageBonus;
        public int defenseBonus;
        [Range(0, 1)] public float blockChanceBonus;
        [Range(0, 1)] public float criticalChanceBonus;

    #if _iMMO2D
        public List<Sprite> sprites = new List<Sprite>();
    #else
        public GameObject modelPrefab;
    #endif
        // usage
        // -> can we equip this into any slot?
        public override bool CanUse(Player player, int inventoryIndex)
        {
            return FindEquipableSlotFor(player, inventoryIndex) != -1;
        }

    #if _iMMOUMACHARACTERS
        [Header("[-=-=-[ UMA Recipe ]-=-=-]")]
        public UMATextRecipe maleUmaRecipe;
        public UMATextRecipe femaleUmaRecipe;
    #endif


        //[Serializable] public class UnityEventUneqip : UnityEvent<Player , int , int > { }
        /*[Header("Events")]
        public UnityEvent<Player, int, int, MutableWrapper<bool>> canUnequip;
        public UnityEvent<Player, EquipmentItem, MutableWrapper<bool>> canUnequipClick;*/
        /*
    #if _iMMOCURSEDEQUIPMENT

            void Start()
            {
                canUnequip.AddListener(CanUnequip_CursedEquipment);
                canUnequipClick.AddListener(CanUnequipClick_CursedEquipment);
            }

    #endif
        */
        // -----------------------------------------------------------------------------------
        // CanEquip (Swapping)
        // -----------------------------------------------------------------------------------
        public bool CanEquip(Player player, int inventoryIndex, int equipmentIndex)
        {
            EquipmentInfo slotInfo = ((PlayerEquipment)player.equipment).slotInfo[equipmentIndex];
            string requiredCategory = slotInfo.requiredCategory;
            //Debug.Log(inventoryIndex + "<<<---");
            bool valid = base.CanUse(player, inventoryIndex) && requiredCategory != "" && category.StartsWith(requiredCategory);
            if (!valid) return false;

            ItemSlot slot = player.equipment.slots[equipmentIndex];

            if (slot.amount <= 0) return true;
    #if _iMMOCURSEDEQUIPMENT
            int overrideLevel = 0;


            if (player.inventory.slots[inventoryIndex].amount > 0)
            {
                EquipmentItem item = (EquipmentItem)player.inventory.slots[inventoryIndex].item.data;
                overrideLevel = item.overrideCursedLevel;
            }
            if (player.equipment.slots.Any(x => (x.amount > 0 && ((EquipmentItem)x.item.data).nullsAllCurses == true))) 
                return true; // no need to check if any item can nullify curses

            valid = (((EquipmentItem)slot.item.data).cursedLevel <= 0) || (((EquipmentItem)slot.item.data).cursedLevel > 0 && overrideLevel >= ((EquipmentItem)slot.item.data).cursedLevel);

            if (valid == false)
                player.Tools_TargetAddMessage(cursedCannotUnequipMsg);
    #endif
            return valid;
        }
        // -----------------------------------------------------------------------------------
        // CanUnequip (Swapping)
        // -----------------------------------------------------------------------------------
        public bool CanUnequip(Player player, int inventoryIndex, int equipmentIndex)
        {
            //Debug.Log("Call CanUnequip_function");
            MutableWrapper<bool> bValid = new MutableWrapper<bool>(true);
            Utils.InvokeMany(typeof(EquipmentItem), this, "CanUnequip_", player, inventoryIndex, equipmentIndex, bValid);
            return bValid.Value;
        }

        // -----------------------------------------------------------------------------------
        // CanUnequipClick (Clicking)
        // -----------------------------------------------------------------------------------
        public bool CanUnequipClick(Player player, EquipmentItem item)
        {
            MutableWrapper<bool> bValid = new MutableWrapper<bool>(true);
            Utils.InvokeMany(typeof(EquipmentItem), this, "CanUnequipClick_", player, item, bValid);
            return bValid.Value;
        }

        // -----------------------------------------------------------------------------------
        int FindEquipableSlotFor(Player player, int inventoryIndex)
        {
            for (int i = 0; i < player.equipment.slots.Count; ++i)
                if (CanEquip(player, inventoryIndex, i))
                    return i;
            return -1;
        }

        public override void Use(Player player, int inventoryIndex)
        {
    #if _SERVER
            // always call base function too
            base.Use(player, inventoryIndex);

            // find a slot that accepts this category, then equip it
            int equipmentIndex = FindEquipableSlotFor(player, inventoryIndex);
            if (equipmentIndex != -1)
            {
                ItemSlot inventorySlot = player.inventory.slots[inventoryIndex];
                ItemSlot equipmentSlot = player.equipment.slots[equipmentIndex];

                // merge? check Equals because name AND dynamic variables matter (petLevel etc.)
                // => merge is important when dragging more arrows into an arrow slot!
                if (inventorySlot.amount > 0 && equipmentSlot.amount > 0 && inventorySlot.item.Equals(equipmentSlot.item))
                    ((PlayerEquipment)player.equipment).MergeInventoryEquip(inventoryIndex, equipmentIndex);
                // swap?
                else
                    ((PlayerEquipment)player.equipment).SwapInventoryEquip(inventoryIndex, equipmentIndex);
            }
    #endif
        }

        // tooltip
        public override string ToolTip()
        {
            StringBuilder tip = new StringBuilder(base.ToolTip());
            tip.Replace("{CATEGORY}", category);
            tip.Replace("{DAMAGEBONUS}", damageBonus.ToString());
            tip.Replace("{DEFENSEBONUS}", defenseBonus.ToString());
            tip.Replace("{HEALTHBONUS}", healthBonus.ToString());
            tip.Replace("{MANABONUS}", manaBonus.ToString());
            tip.Replace("{BLOCKCHANCEBONUS}", Mathf.RoundToInt(blockChanceBonus * 100).ToString());
            tip.Replace("{CRITICALCHANCEBONUS}", Mathf.RoundToInt(criticalChanceBonus * 100).ToString());
            return tip.ToString();
        }
    }
}
#endif