using TMPro;
using UnityEngine;

namespace uMMORPG
{
    
    
    public class UI_FPS : MonoBehaviour
    {
    
        public GameObject panel;
    
        public TMP_Text fpsText;
    #if _CLIENT
        public float goodThreshold = 90f;
        public float okayThreshold = 60f;
    
        [Header("Refresh Time")]
        public float fpsMeasurePeriod = 0.5f;
    
        public Color goodColor = Color.green;
        public Color okayColor = Color.yellow;
        public Color badColor = Color.red;
    
    
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} FPS";
    
    
        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        }
    
    
        private void Update()
        {
            Player player = Player.localPlayer;
    #if _iMMOSETTINGS
            //TODO faire les v�rification des playerpref dans un update est une mauvaise id�e, cela cr�er du GC
            if (player && !PlayerPrefs.HasKey("ShowFps") || (PlayerPrefs.GetInt("ShowFps") == 1))
    #else
            if (player)
    #endif
            {
                
                // measure average frames per second
                m_FpsAccumulator++;
                if (Time.realtimeSinceStartup > m_FpsNextPeriod)
                {
                    panel.SetActive(true);
                    if (m_CurrentFps >= goodThreshold)
                        fpsText.color = goodColor;
                    else if (m_CurrentFps >= okayThreshold)
                        fpsText.color = okayColor;
                    else
                        fpsText.color = badColor;
                    m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                    m_FpsAccumulator = 0;
                    m_FpsNextPeriod += fpsMeasurePeriod;
                    fpsText.text = string.Format(display, m_CurrentFps);
                }
            }
            else
            {
                panel.SetActive(false);
            }
        }
    #endif
    }
    
}
