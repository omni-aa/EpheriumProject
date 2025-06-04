// Note: this script has to be on an always-active UI parent, so that we can
// always react to the hotkey.
using UnityEngine;

namespace uMMORPG
{
    public partial class UIEquipment : MonoBehaviour
    {
        public KeyCode hotKey = KeyCode.U; // 'E' is already used for rotating
        public GameObject panel;
#if !_iMMOTOOLS
        public UIEquipmentSlot slotPrefab;
#else
    public UI_UniversalSlot slotPrefab;
#endif
        public Transform content;

        [Header("Durability Colors")]
        public Color brokenDurabilityColor = Color.red;
        public Color lowDurabilityColor = Color.magenta;
        [Range(0.01f, 0.99f)] public float lowDurabilityThreshold = 0.1f;

        void Update()
        {
            Player player = Player.localPlayer;
            if (player)
            {
                // hotkey (not while typing in chat, etc.)
                if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
                    panel.SetActive(!panel.activeSelf);

                // only enable avatar camera while panel is active.
                // no need to render while the window is hidden!
                ((PlayerEquipment)player.equipment).avatarCamera.enabled = panel.activeSelf;

                // only update the panel if it's active
                if (panel.activeSelf)
                {
                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(slotPrefab.gameObject, player.equipment.slots.Count, content);

                    // refresh all
                    for (int i = 0; i < player.equipment.slots.Count; ++i)
                    {
#if !_iMMOTOOLS
                        UIEquipmentSlot slot = content.GetChild(i).GetComponent<UIEquipmentSlot>();
#else
                    UI_UniversalSlot slot = content.GetChild(i).GetComponent<UI_UniversalSlot>();
#endif
                        slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                        ItemSlot itemSlot = player.equipment.slots[i];

                        // set category overlay in any case. we use the last noun in the
                        // category string, for example EquipmentWeaponBow => Bow
                        // (disabled if no category, e.g. for archer shield slot)
                        EquipmentInfo slotInfo = ((PlayerEquipment)player.equipment).slotInfo[i];
                        slot.categoryOverlay.SetActive(slotInfo.requiredCategory != "");
                        string overlay = Utils.ParseLastNoun(slotInfo.requiredCategory);
                        slot.categoryText.text = overlay != "" ? overlay : "?";
#if _iMMOTOOLS
                    RemoveEquipmentItem(i, slot, player);
#endif

                        if (itemSlot.amount > 0)
                        {

                            // refresh valid item

                            // only build tooltip while it's actually shown. this
                            // avoids MASSIVE amounts of StringBuilder allocations.
                            slot.tooltip.enabled = true;
                            if (slot.tooltip.IsVisible())
                                slot.tooltip.text = itemSlot.ToolTip();
                            slot.dragAndDropable.dragable = true;

                            // use durability colors?
#if _iMMO2D
                        slot.image.color = Color.white;
#else
                            if (itemSlot.item.maxDurability > 0)
                            {
                                if (itemSlot.item.durability == 0)
                                    slot.image.color = brokenDurabilityColor;
                                else if (itemSlot.item.DurabilityPercent() < lowDurabilityThreshold)
                                    slot.image.color = lowDurabilityColor;
                                else
                                    slot.image.color = Color.white;
                            }
                            else slot.image.color = Color.white; // reset for no-durability items
#endif
                            slot.image.sprite = itemSlot.item.image;

                            // cooldown if usable item
                            if (itemSlot.item.data is UsableItem usable)
                            {
                                float cooldown = player.GetItemCooldown(usable.cooldownCategory);
                                slot.cooldownCircle.fillAmount = usable.cooldown > 0 ? cooldown / usable.cooldown : 0;
                            }
                            else slot.cooldownCircle.fillAmount = 0;
                            slot.amountOverlay.SetActive(itemSlot.amount > 1);
                            slot.amountText.text = itemSlot.amount.ToString();

#if _iMMOITEMRARITY
                        ScriptableItem itemData = itemSlot.item.data;
                        slot.rarityOutline.color = RarityColor.SetRarityColorResult(itemSlot.item);
#endif
                            // Equipment Level Up Integration
#if _iMMOITEMLEVELUP
                        if (itemSlot.item.data is EquipmentItem equipmentItem)
                        {
                            if (itemSlot.item.equipmentLevel > 0)
                            {
                                slot.equipmentLevelUpText.gameObject.SetActive(true);
                                slot.equipmentLevelUpText.text = "+" + itemSlot.item.equipmentLevel.ToString();
                            }
                            else
                            {
                                slot.equipmentLevelUpText.gameObject.SetActive(false); // Hide if level is 0
                            }
                        }
#endif
                        }
                        else
                        {
                            // refresh invalid item
                            slot.tooltip.enabled = false;
                            slot.dragAndDropable.dragable = false;
                            slot.image.color = Color.clear;
                            slot.image.sprite = null;
                            slot.cooldownCircle.fillAmount = 0;
                            slot.amountOverlay.SetActive(false);
#if _iMMOITEMRARITY
                        slot.rarityOutline.color = Color.clear;
#endif
                            // Disable Equipment Level Up Text if no valid item
#if _iMMOITEMLEVELUP
                        slot.equipmentLevelUpText.gameObject.SetActive(false);
#endif
                        }
                    }
                }
            }
            else panel.SetActive(false);
        }
    }
}