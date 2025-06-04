using UnityEngine;
using Mirror;
using UnityEditor;
using UnityEngine.Events;

namespace uMMORPG
{
    [RequireComponent(typeof(PlayerTrading))]
    public partial class PlayerInventory : Inventory
    {
        [Header("Components")]
        public Player player;

        [Header("Inventory")]
        public int size = 30;
        public ScriptableItemAndAmount[] defaultItems;
        public KeyCode[] splitKeys = { KeyCode.LeftShift, KeyCode.RightShift };

        [Header("Trash")]
        [SyncVar] public ItemSlot trash;

#if _SERVER
        [HideInInspector] public bool isDirtyInventory = false;
#endif

        public UnityEvent onInventoryChanged;
        // are inventory operations like swap, merge, split allowed at the moment?
        // -> trading offers are inventory indices. we don't allow any inventory
        //    operations while trading to guarantee the trade offer indices don't
        //    get messed up when swapping items with one of the indices.
        public bool InventoryOperationsAllowed()
        {
            return player.state == "IDLE" ||
                   player.state == "MOVING" ||
                   player.state == "CASTING";
        }
#if MIRROR_90_OR_NEWER
        void OnInventoryChanged(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldData)
#else
        void OnInventoryChanged(SyncList<ItemSlot>.Operation op, int index, ItemSlot oldData, ItemSlot newData)
#endif
        {
#if _SERVER && _iMMOWEIGHTSYSTEM
        player.playerAddonsConfigurator.UpdateEvent_WeightSystem();
#endif

#if _iMMOITEMLEVELUP
#if MIRROR_90_OR_NEWER
            //Debug.Log(index + " | " + slots.Count);
            if (slots[index-1].amount == 0 || slots[index-1].amount >= 1 && !slots[index-1].Equals(itemInLevelUpSlot))
#else
        if (newData.amount == 0 || newData.amount >= 1 && !newData.Equals(itemInLevelUpSlot))
#endif
        {
            OnInventoryChanged_ItemLevelUp();
        }
#endif
#if _SERVER
            isDirtyInventory = true; // <- this is used ny Mysql for save or not save inventory.
#endif
            onInventoryChanged.Invoke();
        }

        public override void OnStartServer()
        {
#if MIRROR_90_OR_NEWER
            slots.OnChange += OnInventoryChanged;
#else
#pragma warning disable CS0618
            slots.Callback += OnInventoryChanged;
#pragma warning restore CS0618
#endif
        }

#if _SERVER
        /**
         * This new function is used by all other script for removing items from the inventory and set inventory is dirty for save inventory.
         */
        /*public bool RemoveInInventory(Item item, int amount)
        {
            isDirtyInventory = true; // <- this is used ny Mysql for save or not save inventory.
            return player.inventory.Remove(item, amount);
        }

        /**
         * This new function is used by all other script for adding items from the inventory and set inventory is dirty for save inventory.
         */
        /*public bool AddInInventory(Item item, int amount)
        {
            isDirtyInventory = true; // <- this is used ny Mysql for save or not save inventory.
            return player.inventory.Add(item, amount);
        }*/
#endif

        [Command]
        public void CmdSwapInventoryTrash(int inventoryIndex)
        {
#if _SERVER
            // dragging an inventory item to the trash always overwrites the trash
            if (InventoryOperationsAllowed() &&
                0 <= inventoryIndex && inventoryIndex < slots.Count)
            {
                // inventory slot has to be valid and destroyable and not summoned
                ItemSlot slot = slots[inventoryIndex];
                if (slot.amount > 0 && slot.item.destroyable && !slot.item.summoned)
                {
                    // overwrite trash
                    trash = slot;

                    // clear inventory slot
                    slot.amount = 0;
                    slots[inventoryIndex] = slot;
                    isDirtyInventory = true;
                }
            }
#endif
        }

        [Command]
        public void CmdSwapTrashInventory(int inventoryIndex)
        {
#if _SERVER
            if (InventoryOperationsAllowed() &&
                0 <= inventoryIndex && inventoryIndex < slots.Count)
            {
                // inventory slot has to be empty or destroyable
                ItemSlot slot = slots[inventoryIndex];
                if (slot.amount == 0 || slot.item.destroyable)
                {
                    // swap them
                    slots[inventoryIndex] = trash;
                    trash = slot;
                    isDirtyInventory = true;
                }
            }
#endif
        }

        [Command]
        public void CmdSwapInventoryInventory(int fromIndex, int toIndex)
        {
#if _SERVER
            // note: should never send a command with complex types!
            // validate: make sure that the slots actually exist in the inventory
            // and that they are not equal
            if (InventoryOperationsAllowed() &&
                0 <= fromIndex && fromIndex < slots.Count &&
                0 <= toIndex && toIndex < slots.Count &&
                fromIndex != toIndex)
            {
                // swap them
                ItemSlot temp = slots[fromIndex];
                slots[fromIndex] = slots[toIndex];
                slots[toIndex] = temp;
                isDirtyInventory = true;
            }
#endif
        }

        [Command]
        public void CmdInventorySplit(int fromIndex, int toIndex)
        {
#if _SERVER
            // note: should never send a command with complex types!
            // validate: make sure that the slots actually exist in the inventory
            // and that they are not equal
            if (InventoryOperationsAllowed() &&
                0 <= fromIndex && fromIndex < slots.Count &&
                0 <= toIndex && toIndex < slots.Count &&
                fromIndex != toIndex)
            {
                // slotFrom needs at least two to split, slotTo has to be empty
                ItemSlot slotFrom = slots[fromIndex];
                ItemSlot slotTo = slots[toIndex];
                if (slotFrom.amount >= 2 && slotTo.amount == 0)
                {
                    // split them serversided (has to work for even and odd)
                    slotTo = slotFrom; // copy the value

                    slotTo.amount = slotFrom.amount / 2;
                    slotFrom.amount -= slotTo.amount; // works for odd too

                    // put back into the list
                    slots[fromIndex] = slotFrom;
                    slots[toIndex] = slotTo;
                    isDirtyInventory = true;
                }
            }
#endif
        }

        [Command]
        public void CmdInventoryMerge(int fromIndex, int toIndex)
        {
#if _SERVER
            if (InventoryOperationsAllowed() &&
                0 <= fromIndex && fromIndex < slots.Count &&
                0 <= toIndex && toIndex < slots.Count &&
                fromIndex != toIndex)
            {
                // both items have to be valid
                ItemSlot slotFrom = slots[fromIndex];
                ItemSlot slotTo = slots[toIndex];
                if (slotFrom.amount > 0 && slotTo.amount > 0)
                {
                    // make sure that items are the same type
                    // note: .Equals because name AND dynamic variables matter (petLevel etc.)
                    if (slotFrom.item.Equals(slotTo.item))
                    {
                        // merge from -> to
                        // put as many as possible into 'To' slot
                        int put = slotTo.IncreaseAmount(slotFrom.amount);
                        slotFrom.DecreaseAmount(put);

                        // put back into the list
                        slots[fromIndex] = slotFrom;
                        slots[toIndex] = slotTo;
                        isDirtyInventory = true;
                    }
                }
            }
#endif
        }

        [ClientRpc]
        public void RpcUsedItem(Item item)
        {
            // validate
            if (item.data is UsableItem usable)
            {
                usable.OnUsed(player);
            }
        }

        [Command]
        public void CmdUseItem(int index)
        {
#if _SERVER
            // validate
            if (InventoryOperationsAllowed() &&
                0 <= index && index < slots.Count && slots[index].amount > 0 &&
                slots[index].item.data is UsableItem usable)
            {
                // use item
                // note: we don't decrease amount / destroy in all cases because
                // some items may swap to other slots in .Use()
                if (usable.CanUse(player, index))
                {
                    // .Use might clear the slot, so we backup the Item first for the Rpc
                    Item item = slots[index].item;
                    usable.Use(player, index);
                    RpcUsedItem(item);
                    isDirtyInventory = true;
                }
            }
#endif
        }

        // drag & drop /////////////////////////////////////////////////////////////
        void OnDragAndDrop_InventorySlot_InventorySlot(int[] slotIndices)
        {
            // slotIndices[0] = slotFrom; slotIndices[1] = slotTo

            // merge? check Equals because name AND dynamic variables matter (petLevel etc.)
            if (slots[slotIndices[0]].amount > 0 && slots[slotIndices[1]].amount > 0 &&
                slots[slotIndices[0]].item.Equals(slots[slotIndices[1]].item))
            {
                CmdInventoryMerge(slotIndices[0], slotIndices[1]);
            }
            // split?
            else if (Utils.AnyKeyPressed(splitKeys))
            {
                CmdInventorySplit(slotIndices[0], slotIndices[1]);
            }
            // swap?
            else
            {
                CmdSwapInventoryInventory(slotIndices[0], slotIndices[1]);
            }
        }

        void OnDragAndDrop_InventorySlot_TrashSlot(int[] slotIndices)
        {
            // slotIndices[0] = slotFrom; slotIndices[1] = slotTo
            CmdSwapInventoryTrash(slotIndices[0]);
        }

        void OnDragAndDrop_TrashSlot_InventorySlot(int[] slotIndices)
        {
            // slotIndices[0] = slotFrom; slotIndices[1] = slotTo
            CmdSwapTrashInventory(slotIndices[1]);
        }

        // validation
        protected override void OnValidate()
        {
            base.OnValidate();
            // defaultItems is null when first adding the component. avoid error.
            if (defaultItems != null)
            {
                // it's easy to set a default item and forget to set amount from 0 to 1
                // -> let's do this automatically.
                for (int i = 0; i < defaultItems.Length; ++i)
                    if (defaultItems[i].item != null && defaultItems[i].amount == 0)
                        defaultItems[i].amount = 1;
            }

            // force syncMode to observers for now.
            // otherwise trade offer items aren't shown when trading with someone
            // else, because we can't see the other person's inventory slots.
            if (syncMode != SyncMode.Observers)
            {
                syncMode = SyncMode.Observers;
#if UNITY_EDITOR
                Undo.RecordObject(this, name + " " + GetType() + " component syncMode changed to Observers.");
#endif
            }
        }
    }
}