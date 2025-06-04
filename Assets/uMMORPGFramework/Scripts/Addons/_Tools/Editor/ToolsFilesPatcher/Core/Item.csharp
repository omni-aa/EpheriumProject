// The Item struct only contains the dynamic item properties, so that the static
// properties can be read from the scriptable object.
//
// Items have to be structs in order to work with SyncLists.
//
// Use .Equals to compare two items. Comparing the name is NOT enough for cases
// where dynamic stats differ. E.g. two pets with different levels shouldn't be
// merged.
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Mirror;

namespace uMMORPG
{

    [Serializable]
    public partial struct Item
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int hash;

#if !_iMMO2D
        // current durability
        public int durability;
#endif
        // dynamic stats (cooldowns etc. later)
        public NetworkIdentity summoned; // summonable that's currently summoned
        public int summonedHealth; // stored in item while summonable unsummoned
        public int summonedLevel; // stored in item while summonable unsummoned
        public long summonedExperience; // stored in item while summonable unsummoned
        public int equipmentLevel; // stored in item while equipment level
        public string equipmentGems; // stored in item while equipment gems

        // constructors
        public Item(ScriptableItem data)
        {
            hash = data.name.GetStableHashCode();

#if !_iMMO2D
            durability = data.maxDurability;
#endif
            summoned = null;
            summonedHealth = data is SummonableItem summonable ? summonable.summonPrefab.health.max : 0;
            summonedLevel = data is SummonableItem ? 1 : 0;
            summonedExperience = 0;
            equipmentLevel = 0;// data is EquipmentItem ? 0 : 0 ; // stored in item while summonable unsummoned
            equipmentGems = data is EquipmentItem ? "" : ""; // stored in item while summonable unsummoned
        }

        // wrappers for easier access
        public ScriptableItem data
        {
            get
            {
                // show a useful error message if the key can't be found
                // note: ScriptableItem.OnValidate 'is in resource folder' check
                //       causes Unity SendMessage warnings and false positives.
                //       this solution is a lot better.
                if (!ScriptableItem.All.ContainsKey(hash))
                    throw new KeyNotFoundException("There is no ScriptableItem with hash=" + hash + ". Make sure that all ScriptableItems are in the Resources folder so they are loaded properly.");
                return ScriptableItem.All[hash];
            }
        }
        public string name => data.name;
        public int maxStack => data.maxStack;
#if !_iMMO2D
        public int maxDurability => data.maxDurability;
#endif

#if !_iMMO2D
        public float DurabilityPercent()
        {
            return (durability != 0 && maxDurability != 0) ? (float)durability / (float)maxDurability : 0;
        }
#endif
        public long buyPrice => data.buyPrice;
        public long sellPrice => data.sellPrice;
        public long itemMallPrice => data.itemMallPrice;
        public bool sellable => data.sellable;
        public bool tradable => data.tradable;
        public bool destroyable => data.destroyable;
        public Sprite image => data.image;

        // helper function to check for valid durability if a durability item

#if !_iMMO2D
        public bool CheckDurability() =>
            maxDurability == 0 || durability > 0;
#endif

        // tooltip
        public string ToolTip()
        {
            // note: caching StringBuilder is worse for GC because .Clear frees the internal array and reallocates.
            StringBuilder tip = new StringBuilder(data.ToolTip());

#if _iMMOITEMLEVELUP
        bool equipmentUpgraded = data is EquipmentItem;
        if (equipmentUpgraded)
        {
            EquipmentItem equipmentItem = (EquipmentItem)data;
            tip = new StringBuilder(equipmentItem.Tooltip_EquipmentLevel(equipmentLevel));
        }
#endif

            // show durability only if item has durability
#if !_iMMO2D
            if (maxDurability > 0)
                tip.Replace("{DURABILITY}", (DurabilityPercent() * 100).ToString("F0"));
#endif
            tip.Replace("{SUMMONEDHEALTH}", summonedHealth.ToString());
            tip.Replace("{SUMMONEDLEVEL}", summonedLevel.ToString());
            tip.Replace("{SUMMONEDEXPERIENCE}", summonedExperience.ToString());
            tip.Replace("{EQUIPMENTLEVEL}", ((equipmentLevel > 0) ? "+" + equipmentLevel.ToString() : ""));
            tip.Replace("{EQUIPMENGEMS}", equipmentGems.ToString());

            // addon system hooks
            Utils.InvokeMany(typeof(Item), this, "ToolTip_", tip);

            return tip.ToString();
        }
    }
}