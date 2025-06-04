using UnityEngine;

namespace uMMORPG
{
    
    public class ScreenSleepControl : MonoBehaviour
    {
        [Header("Screen Sleep Control")]
        [Tooltip("select the number of minutes before the screen goes to sleep, if you want the screen to never go to sleep check Never Sleep Screen")]
        [Range(1, 180)] public int minToSleepScreen = 10;
    
        public bool neverSleepScreen = false;
    
        public void Start()
        {
            if (neverSleepScreen)
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            else
                Screen.sleepTimeout = (minToSleepScreen * 60);
        }
    
    }
    
}
