using UnityEngine;

namespace uMMORPG
{
    
    // UIElement
    public abstract class UIElement : MonoBehaviour
    {
        [Header("[-=-[ UI Element ]-=-]")]
        [SerializeField] private bool throttleUpdate = true;
    
        [SerializeField] [Range(0.01f, 3f)] private float updateInterval = 0.25f;
    
        protected float fInterval;
    
        // -----------------------------------------------------------------------------------
        // Start
        // -----------------------------------------------------------------------------------
        private void Update()
        {
            if (!throttleUpdate || (throttleUpdate && Time.time > fInterval))
            {
                SlowUpdate();
                fInterval = Time.time + updateInterval;
            }
        }
    
        // -----------------------------------------------------------------------------------
        // SlowUpdate
        // -----------------------------------------------------------------------------------
        protected virtual void SlowUpdate() { }
    
        // -----------------------------------------------------------------------------------
    }
    
}
