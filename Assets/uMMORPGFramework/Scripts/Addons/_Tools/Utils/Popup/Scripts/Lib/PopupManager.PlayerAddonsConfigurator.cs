using UnityEngine;

namespace uMMORPG
{
    
    public partial class PlayerAddonsConfigurator
    {
        [Header("[-=-[ Popup Configuration ]-=-]")]
        public Tmpl_PopupConfiguration popupConfiguration;
    
        [HideInInspector] public UI_PopupManager popupManager;
    
        // -----------------------------------------------------------------------------------
        // Awake on client
        // -----------------------------------------------------------------------------------
        private void Awake_Client_ToolsPopUpManager()
        {
            if (popupManager == null)
                popupManager = FindFirstObjectByType<UI_PopupManager>();
        }
    }
    
}
