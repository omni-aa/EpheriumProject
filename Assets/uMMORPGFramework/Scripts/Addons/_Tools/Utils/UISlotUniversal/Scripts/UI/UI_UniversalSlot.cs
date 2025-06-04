namespace uMMORPG
{
    // Attach to the prefab for easier component access by the UI Scripts.
    // Otherwise we would need slot.GetChild(0).GetComponentInChildren<Text> etc.
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class UI_UniversalSlot : MonoBehaviour
    {
        public UIShowToolTip tooltip;
        public Button button;
        public UIDragAndDropable dragAndDropable;
        public Image image;
        public Image cooldownCircle;
    #if _iMMOITEMRARITY
        public Image rarityOutline;
    #endif
        public GameObject amountOverlay;
        public TMP_Text amountText;
    
    #if _iMMOITEMLEVELUP
        public TMP_Text equipmentLevelUpText;
    #endif
    
        public GameObject categoryOverlay;
        public TMP_Text categoryText;
    
    
        public void ApplyItemSlot(int amount, int equipmentLevel, string tooltipText, Color itemColor, Sprite itemImage, Color itemRarityColor, bool enableTooltip = true, bool dragable = true)
        {
            // ajouter le slot et ajouter les d�tails
            gameObject.SetActive(amount > 0);
    
            tooltip.enabled = enableTooltip;
            tooltip.text = tooltipText;
    
            dragAndDropable.dragable = dragable;
    
            image.color = Color.white; // reset for no-durability items
            image.sprite = itemImage;
    
            amountOverlay.SetActive(amount > 1);
            amountText.text = amount.ToString();
    
    #if _iMMOITEMRARITY
            rarityOutline.color = itemRarityColor;
    #endif
    
    #if _iMMOITEMLEVELUP
            if (equipmentLevel > 0)
            {
                equipmentLevelUpText.gameObject.SetActive(true);
                equipmentLevelUpText.text = equipmentLevel > 0 ? "+" + equipmentLevel.ToString() : "";
            }
            else
            {
                equipmentLevelUpText.gameObject.SetActive(false);
            }
    #endif
        }
    }
    
}
