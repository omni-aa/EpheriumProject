using UnityEngine;
using UnityEngine.Events;

namespace uMMORPG
{
    
    
    public class UI_BindKeyPanel : MonoBehaviour
    {
        [Header("[-=-[ Key panel binding ]-=-]")]
        public keyPanel[] keyPanels;
    
        [Header("[-=-[ Start on LocalPlayer ]-=-]")]
        public GameObject[] activePanelLocalPlayer;
    
        [Header("[-=-[ Close All open panel ]-=-]")]
        public KeyCode forceClose = KeyCode.Escape;
        // Update is called once per frame
    
        private bool _panelLocalPlayer = false;
        void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                if (Input.anyKeyDown)
                {
                    // foreach all keypanels added
                    foreach (var item in keyPanels)
                    {
                        // hotkey (not while typing in chat, etc.)
                        if (Input.GetKeyDown(item.keyCode) && !UIUtils.AnyInputActive())
                            item.panel.SetActive(!item.panel.activeSelf);
    
                        if (Input.GetKeyDown(forceClose))
                            item.panel.SetActive(false);
                    }
                }
                // Check if panel for local player is already active
                if (!_panelLocalPlayer && player.isActiveAndEnabled) {
                    PanelActivator(true);
                }
            }
            else
            {
                if (_panelLocalPlayer)
                    PanelActivator(false);
            }
        }
    
        private void PanelActivator(bool acitve)
        {
            foreach (var item in activePanelLocalPlayer)
            {
                item.SetActive(acitve);
            }
            _panelLocalPlayer = acitve;
        }
    
    }
    
}
