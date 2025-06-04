using UnityEngine;
using Mirror;

namespace uMMORPG
{

    [DisallowMultipleComponent]
    public partial class MonsterInventory : Inventory
    {
        [Header("Components")]
        public Monster monster;

        [Header("Loot")]
        public int lootGoldMin = 0;
        public int lootGoldMax = 10;
        public ItemDropChance[] dropChances;
#if !_iMMO2D
        public ParticleSystem lootIndicator;
#endif
        [Header("[-=-[ Drop Chances ]-=-]")]
        public ItemDropChanceExtend[] dropChancesExtend;
        // note: Items have a .valid property that can be used to 'delete' an item.
        //       it's better than .RemoveAt() because we won't run into index-out-of
        //       range issues
#if _CLIENT
        [ClientCallback]
        void Update()
        {
#if !_iMMO2D
            // show loot indicator on clients while it still has items
            if (lootIndicator != null)
            {
                // only set active once. we don't want to reset the particle
                // system all the TimeLogout.
                bool hasLoot = HasLoot();
                if (hasLoot && !lootIndicator.isPlaying)
                    lootIndicator.Play();
                else if (!hasLoot && lootIndicator.isPlaying)
                    lootIndicator.Stop();
            }
#endif
        }
#endif

        // other scripts need to know if it still has valid loot (to show UI etc.)
        public bool HasLoot()
        {
            // any gold or valid items?
            return monster.gold > 0 || SlotsOccupied() > 0;
        }
#if _SERVER
        [Server]
        public void OnDeath()
        {
            // generate gold
            monster.gold = Random.Range(lootGoldMin, lootGoldMax);

            // generate items (note: can't use Linq because of SyncList)
            foreach (ItemDropChance itemChance in dropChances)
                if (Random.value <= itemChance.probability)
                    slots.Add(new ItemSlot(new Item(itemChance.item)));

            Entity lastAgressor = (monster.lastAggressor is Summonable summonable) ? summonable.owner : monster.lastAggressor;
            if (monster.lastAggressor == null || lastAgressor is not Player || dropChancesExtend.Length == 0) return;
            foreach (ItemDropChanceExtend itemChance in dropChancesExtend)
            {
                if (itemChance.dropRequirements.checkRequirements((Player)lastAgressor))
                {
                    if (Random.value <= itemChance.probability)
                    {
                        int amount = Random.Range(itemChance.minStack, itemChance.maxStack);

                        slots.Add(new ItemSlot(new Item(itemChance.item), amount));
                    }
                }
            }
        }
#endif
    }
}