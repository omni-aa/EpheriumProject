using UnityEngine.UI;

namespace uMMORPG
{
    
    public partial class UIEquipment
    {
        // Adds a listener to the equipment item button to allow click removal.
    #if _iMMOTOOLS
        private void RemoveEquipmentItem(int currentIndex, UI_UniversalSlot slot, Player player)
    #else
        private void RemoveEquipmentItem(int currentIndex, UIEquipmentSlot slot, Player player)
    #endif
        {
    #if _iMMOTOOLS
            slot.button.onClick.SetListener(() =>
    #else
            Button button = slot.transform.GetChild(0).GetComponent<Button>();
            button.onClick.SetListener(() =>
    #endif
            {
    
    #if _iMMOTOOLS
                ((PlayerEquipment)player.equipment).CmdRemoveEquipmentItem(currentIndex); // added to move equipment into open inventory slot
    #endif
            });
        }
    }
    
}
