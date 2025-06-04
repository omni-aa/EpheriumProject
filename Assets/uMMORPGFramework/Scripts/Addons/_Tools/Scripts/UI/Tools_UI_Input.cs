using UnityEngine;
using UnityEngine.UI;

namespace uMMORPG
{
    
    // UI INPUT
    
    public class Tools_UI_Input : MonoBehaviour
    {
        public GameObject panel;
        public Text messageText;
        public Text amountText;
        public Slider amountSlider;
        public Button buttonConfirm;
    
        private Tools_UI_InputCallback instance;
    
        // -----------------------------------------------------------------------------------
        // Show
        // -----------------------------------------------------------------------------------
        public void Show(string message, int minAmount, int maxAmount, Tools_UI_InputCallback callbackObject)
        {
            instance = callbackObject;
            messageText.text = message;
            amountSlider.value = 0;
            amountSlider.minValue = minAmount;
            amountSlider.maxValue = maxAmount;
            amountText.text = amountSlider.value.ToString() + "/" + maxAmount.ToString();
            panel.SetActive(true);
        }
    
        // -----------------------------------------------------------------------------------
        // SliderValueChanged
        // -----------------------------------------------------------------------------------
        public void SliderValueChanged()
        {
            amountText.text = amountSlider.value.ToString() + "/" + amountSlider.maxValue.ToString();
        }
    
        // -----------------------------------------------------------------------------------
        // Confirm
        // -----------------------------------------------------------------------------------
        public void Confirm()
        {
            instance.ConfirmInput((int)amountSlider.value);
            panel.SetActive(false);
        }
    
        // -----------------------------------------------------------------------------------
        // Hide
        // -----------------------------------------------------------------------------------
        public void Hide()
        {
            panel.SetActive(false);
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
