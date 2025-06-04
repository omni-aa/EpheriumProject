using UnityEngine;
using Mirror;
using System.Collections;
using TMPro;

namespace uMMORPG
{
    
    public class UI_Latency : MonoBehaviour
    {
        public GameObject panel;
    
        public TMP_Text latencyText;
    
        public float goodThreshold = 0.3f;
        public float okayThreshold = 2;
    
        [Header("Refresh speed UI")]
        public float refreshSpeed = 0.5f;
    
        public Color goodColor = Color.green;
        public Color okayColor = Color.yellow;
        public Color badColor = Color.red;
    
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
    
        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + refreshSpeed;
        }
        void Update()
        {
            Player player = Player.localPlayer;
    #if _iMMOSETTINGS
            //TODO faire les vérification des playerpref dans un update est une mauvaise idée, cela créer du GC
            if (player && !PlayerPrefs.HasKey("ShowPing") || (PlayerPrefs.GetInt("ShowPing") == 1))
    #else
            if (player)
    #endif
            {
                // measure average frames per second
                m_FpsAccumulator++;
                if (Time.realtimeSinceStartup > m_FpsNextPeriod)
                {
                    panel.SetActive(true);
                    // only while connected
                    latencyText.enabled = NetworkClient.isConnected;
    
                    // change color based on status
                    if (NetworkTime.rtt <= goodThreshold)
                        latencyText.color = goodColor;
                    else if (NetworkTime.rtt <= okayThreshold)
                        latencyText.color = okayColor;
                    else
                        latencyText.color = badColor;
    
                    // show latency in milliseconds
                    latencyText.text = Mathf.Round((float)NetworkTime.rtt * 1000) + " Ms";
                }
            }
    
            else
            {
                panel.SetActive(false);
            }
        }
    }
    
}
