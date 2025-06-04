using UnityEngine;

namespace uMMORPG
{
    
    // UI INPUT CALLBACK
    
    public class Tools_UI_InputCallback : MonoBehaviour
    {
        private Tools_UI_Input instance;
    
        [HideInInspector] public int chosenAmount;
        [HideInInspector] public int selectedID;
        [HideInInspector] public bool confirmed = false;
    
        // -----------------------------------------------------------------------------------
        // Show
        // -----------------------------------------------------------------------------------
        public void Show(string message, int minAmount, int maxAmount, int _selectedID)
        {
            if (instance == null)
                instance = FindFirstObjectByType<Tools_UI_Input>();
                //instance = FindObjectOfType<Tools_UI_Input>();
    
            confirmed = false;
            chosenAmount = 0;
            selectedID = _selectedID;
    
            instance.Show(message, minAmount, maxAmount, this);
        }
    
        // -----------------------------------------------------------------------------------
        // ConfirmInput
        // -----------------------------------------------------------------------------------
        public void ConfirmInput(int amount)
        {
            chosenAmount = amount;
            confirmed = true;
        }
    
        // -----------------------------------------------------------------------------------
        // Reset
        // -----------------------------------------------------------------------------------
        public void Reset()
        {
            confirmed = false;
            chosenAmount = 0;
            selectedID = -1;
    
            if (instance != null)
                instance.Hide();
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
