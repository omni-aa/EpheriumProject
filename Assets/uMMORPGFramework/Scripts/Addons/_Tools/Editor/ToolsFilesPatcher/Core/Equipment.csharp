using UnityEngine;

namespace uMMORPG
{

    [DisallowMultipleComponent]
    public abstract class Equipment : ItemContainer, IHealthBonus, IManaBonus, ICombatBonus
    {
        // boni ////////////////////////////////////////////////////////////////////
        public int GetHealthBonus(int baseHealth)
        {
            // calculate equipment bonus
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ItemSlot slot in slots)
                if (slot.amount > 0
#if !_iMMO2D
                    && slot.item.CheckDurability()
#endif
                    )
                {
                    bonus += ((EquipmentItem)slot.item.data).healthBonus;
#if _iMMOITEMLEVELUP
                if (slot.item.equipmentLevel > 0 && ((EquipmentItem)slot.item.data).enableLevelUp)
                    bonus += ((EquipmentItem)slot.item.data).LevelUpParameters[slot.item.equipmentLevel - 1].equipmentLevelUpModifier.equipmentStatModifier.healthBonus;
#endif
                }

            return bonus;
        }

        public int GetHealthRecoveryBonus() => 0;

        public int GetManaBonus(int baseMana)
        {
            // calculate equipment bonus
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ItemSlot slot in slots)
                if (slot.amount > 0
#if !_iMMO2D
                    && slot.item.CheckDurability()
#endif
                    )
                {
                    bonus += ((EquipmentItem)slot.item.data).manaBonus;
#if _iMMOITEMLEVELUP
                if (slot.item.equipmentLevel > 0 && ((EquipmentItem)slot.item.data).enableLevelUp)
                    bonus += ((EquipmentItem)slot.item.data).LevelUpParameters[slot.item.equipmentLevel - 1].equipmentLevelUpModifier.equipmentStatModifier.manaBonus;
#endif
                }
            return bonus;
        }

        public int GetManaRecoveryBonus() => 0;

        public int GetDamageBonus()
        {
            // calculate equipment bonus
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ItemSlot slot in slots)
                if (slot.amount > 0
#if !_iMMO2D
                    && slot.item.CheckDurability()
#endif
                    )
                {
                    bonus += ((EquipmentItem)slot.item.data).damageBonus;
#if _iMMOITEMLEVELUP
                if (slot.item.equipmentLevel > 0 && ((EquipmentItem)slot.item.data).enableLevelUp)
                    bonus += ((EquipmentItem)slot.item.data).LevelUpParameters[slot.item.equipmentLevel - 1].equipmentLevelUpModifier.equipmentStatModifier.damageBonus;
#endif
                }
            return bonus;
        }

        public int GetDefenseBonus()
        {
            // calculate equipment bonus
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            foreach (ItemSlot slot in slots)
                if (slot.amount > 0
#if !_iMMO2D
                    && slot.item.CheckDurability()
#endif
                    )
                {
                    bonus += ((EquipmentItem)slot.item.data).defenseBonus;
#if _iMMOITEMLEVELUP
                if (slot.item.equipmentLevel > 0 && ((EquipmentItem)slot.item.data).enableLevelUp)
                    bonus += ((EquipmentItem)slot.item.data).LevelUpParameters[slot.item.equipmentLevel - 1].equipmentLevelUpModifier.equipmentStatModifier.defenseBonus;
#endif
                }
            return bonus;
        }

        public float GetCriticalChanceBonus()
        {
            // calculate equipment bonus
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            float bonus = 0;
            foreach (ItemSlot slot in slots)
                if (slot.amount > 0
#if !_iMMO2D
                    && slot.item.CheckDurability()
#endif
                    )
                {
                    bonus += ((EquipmentItem)slot.item.data).criticalChanceBonus;
#if _iMMOITEMLEVELUP
                if (slot.item.equipmentLevel > 0 && ((EquipmentItem)slot.item.data).enableLevelUp)
                    bonus += ((EquipmentItem)slot.item.data).LevelUpParameters[slot.item.equipmentLevel - 1].equipmentLevelUpModifier.equipmentStatModifier.criticalChanceBonus;
#endif
                }
            return bonus;
        }

        public float GetBlockChanceBonus()
        {
            // calculate equipment bonus
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            float bonus = 0;
            foreach (ItemSlot slot in slots)
                if (slot.amount > 0
#if !_iMMO2D
                    && slot.item.CheckDurability()
#endif
                    )
                {
                    bonus += ((EquipmentItem)slot.item.data).blockChanceBonus;
#if _iMMOITEMLEVELUP
                if (slot.item.equipmentLevel > 0 && ((EquipmentItem)slot.item.data).enableLevelUp)
                    bonus += ((EquipmentItem)slot.item.data).LevelUpParameters[slot.item.equipmentLevel - 1].equipmentLevelUpModifier.equipmentStatModifier.blockChanceBonus;
#endif
                }
            return bonus;
        }

        ////////////////////////////////////////////////////////////////////////////
        // helper function to find the equipped weapon index
        // -> works for all entity types. returns -1 if no weapon equipped.
        public int GetEquippedWeaponIndex()
        {
            // (avoid FindIndex to minimize allocations)
            for (int i = 0; i < slots.Count; ++i)
            {
                ItemSlot slot = slots[i];
                if (slot.amount > 0 && slot.item.data is WeaponItem)
                    return i;
            }
            return -1;
        }

        // get currently equipped weapon category to check if skills can be casted
        // with this weapon. returns "" if none.
        public string GetEquippedWeaponCategory()
        {
            // find the weapon slot
            int index = GetEquippedWeaponIndex();
            return index != -1 ? ((WeaponItem)slots[index].item.data).category : "";
        }

#if _iMMOCOMBATREMASTERED
    public WeaponType GetEquippedWeaponType()
    {
        // find the weapon slot
        int index = GetEquippedWeaponIndex();
        return index != -1 ? ((WeaponItem)slots[index].item.data).weaponType : WeaponType.Unarmed;
    }
#endif
    }
}