using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace uMMORPG
{
    
    // ===================================================================================
    // CAST BAR UI
    // ===================================================================================
    public partial class Tools_UI_CastBar : MonoBehaviour
    {
        public GameObject panel;
        public Slider slider;
        public TMP_Text nameText;
        public TMP_Text progressText;
    
        private double duration;
        private double durationRemaining;
    
        // -----------------------------------------------------------------------------------
        // Update
        // @Client
        // -----------------------------------------------------------------------------------
        private void Update()
        {
            Player player = Player.localPlayer;
            if (!player) return;
    
            if (panel.activeSelf)
            {
                if (NetworkTime.time <= durationRemaining)
                {
                    float ratio = Convert.ToSingle((durationRemaining - NetworkTime.time) / duration);
                    double remain = durationRemaining - NetworkTime.time;
                    slider.value = ratio;
                    progressText.text = remain.ToString("F1") + "s";
                }
                else
                {
                    Hide();
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Show
        // @Client
        // -----------------------------------------------------------------------------------
        public void Show(string labelName, float dura)
        {
            duration = dura;
            durationRemaining = NetworkTime.time + duration;
            nameText.text = labelName;
    
            panel.SetActive(true);
        }
    
        // -----------------------------------------------------------------------------------
        // Hide
        // @Client
        // -----------------------------------------------------------------------------------
        public void Hide()
        {
            panel.SetActive(false);
        }
        // -----------------------------------------------------------------------------------
    }
    
}
