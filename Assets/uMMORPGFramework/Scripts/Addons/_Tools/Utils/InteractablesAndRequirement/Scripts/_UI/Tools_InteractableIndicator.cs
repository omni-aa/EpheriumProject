using UnityEngine;

namespace uMMORPG
{
    
    // INTERACTABLE INDICATOR
    public partial class Tools_InteractableIndicator : MonoBehaviour
    {
        public float degreesPerSecond = 15.0f;
        public float amplitude = 0.25f;
        public float frequency = 0.5f;
    
        private Vector3 posOffset;
    #pragma warning disable CS0169
        private Vector3 tempPos;
    #pragma warning restore
    
        private void Start()
        {
            posOffset = transform.localPosition;
        }
    #if _CLIENT
        private void Update()
        {
    #if !_iMMO2D
            if (degreesPerSecond != 0)
                transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
    #endif
            if (amplitude != 0 && frequency != 0)
            {
                tempPos = posOffset;
                tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
            }
    
            if (transform.localPosition != tempPos)
                transform.localPosition = tempPos;
        }
    #endif
    }
    
}
