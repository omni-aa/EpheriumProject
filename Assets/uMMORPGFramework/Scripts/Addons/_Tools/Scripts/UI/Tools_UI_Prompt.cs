using UnityEngine;
using UnityEngine.UI;

namespace uMMORPG
{
    
    // UI PROMPT
    public class Tools_UI_Prompt : MonoBehaviour
    {
        public GameObject panel;
        public Text messageText;
        public bool forceUseChat;
    
        // -----------------------------------------------------------------------------------
        // Show
        // -----------------------------------------------------------------------------------
        public void Show(string message)
        {
            messageText.text = message;
            panel.SetActive(true);
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
