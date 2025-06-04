using Mirror;
using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // PLAYER
    public partial class Player
    {
    #if _SERVER
        // -----------------------------------------------------------------------------------
        // Tools_ShowPrompt
        // Shows a popup, triggered on the server and sent to the client
        // @Server
        // -----------------------------------------------------------------------------------
        [ServerCallback]
        public void Tools_ShowPrompt(string message)
        {
            Target_Tools_ShowPrompt(connectionToClient, message);
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_ShowPopup
        // Shows a popup, triggered on the server and sent to the client
        // @Server
        // -----------------------------------------------------------------------------------
        [ServerCallback]
        public void Tools_ShowPopup(string message, byte iconId = 0, byte soundId = 0)
        {
            Target_Tools_ShowPopup(connectionToClient, message, iconId, soundId);
        }
    #endif
    
        // -----------------------------------------------------------------------------------
        // Target_Tools_ShowPrompt
        // @Client
        // @Server -> @Client
        // -----------------------------------------------------------------------------------
        [TargetRpc]
        protected void Target_Tools_ShowPrompt(NetworkConnection target, string message)
        {
            Tools_PopupShow(message);
        }
    
        // -----------------------------------------------------------------------------------
        // Target_Tools_ShowPopup
        // @Client
        // @Server -> @Client
        // -----------------------------------------------------------------------------------
        [TargetRpc]
        public void Target_Tools_ShowPopup(NetworkConnection target, string message, byte iconId, byte soundId)
        {
            Tools_ClientShowPopup(message, iconId, soundId);
        }
        // -----------------------------------------------------------------------------------
        // Tools_ClientShowPopup
        // Shows a popup, triggered on the client, shown on the client
        //  @Client
        // -----------------------------------------------------------------------------------
        public void Tools_ClientShowPopup(string message, byte iconId, byte soundId)
        {
            if (playerAddonsConfigurator.popupManager != null)
            {
                //Debug.Log(message + "," + iconId + ", " + soundId);
                playerAddonsConfigurator.popupManager.EnqueuePopup(message, soundId, iconId);
            }
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
