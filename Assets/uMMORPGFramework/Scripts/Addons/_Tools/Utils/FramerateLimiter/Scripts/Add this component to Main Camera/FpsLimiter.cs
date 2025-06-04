using UnityEngine;

namespace uMMORPG
{
    
    public class FpsLimiter : MonoBehaviour
    {
        public int targetFrameRate = 60;
        public bool DisableVSync = false;
        private void Start()
        {
            if (DisableVSync)
            {
                Debug.Log("Vsync disabled");
                QualitySettings.vSyncCount = 0;
            }
            Application.targetFrameRate = targetFrameRate;
        }
    }
    
}
