using Mirror;
using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // ===================================================================================
    // POPUP AREA - BOX
    // ===================================================================================
    #if _iMMO2D
    [RequireComponent(typeof(Collider2D))]
    #else
    [RequireComponent(typeof(Collider))]
    #endif
    public partial class Tools_AreaPopup : NetworkBehaviour
    {
        [Header("[-=-[ Popup display requierements ]-=-]")]
        public Tools_Requirements requirements; 
    
        [Header("[-=-[ Popups Enter ]-=-]")]
        public bool showEnterPopup = true;
        [BoolShowConditional("showEnterPopup", true)]
        [Tooltip("The popup require a message for display")]
        public Tools_PopupClass popupEnter;
    
        [Header("[-=-[ Popups Exit ]-=-]")]
        public bool showExitPopup = true;
        [BoolShowConditional("showExitPopup", true)]
        [Tooltip("The popup require a message for display")]
        public Tools_PopupClass popupExit;
    
    #if _SERVER
        // -----------------------------------------------------------------------------------
        // OnTriggerEnter
        // @Server 
        // -----------------------------------------------------------------------------------
    #if _iMMO2D
        private void OnTriggerEnter2D(Collider2D co)
    #else
        private void OnTriggerEnter(Collider co)
    #endif
        {
            if (showEnterPopup && popupEnter.message != "")
            {
                Player player = co.GetComponentInParent<Player>();
                if (player && requirements.checkRequirements(player))
                {
                    player.Tools_ShowPopup(popupEnter.message, popupEnter.iconId, popupEnter.iconId);
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // OnTriggerExit
        // @Server
        // -----------------------------------------------------------------------------------
    #if _iMMO2D
        private void OnTriggerExit2D(Collider2D co)
    #else
        private void OnTriggerExit(Collider co)
    #endif
        {
            if (showExitPopup && popupExit.message != "")
            {
                Player player = co.GetComponentInParent<Player>();
                if (player && requirements.checkRequirements(player))
                {                
                        player.Tools_ShowPopup(popupExit.message, popupExit.iconId, popupExit.iconId);
                }
            }
        }
    #endif
        // -----------------------------------------------------------------------------------
    }
    
}

    #endif