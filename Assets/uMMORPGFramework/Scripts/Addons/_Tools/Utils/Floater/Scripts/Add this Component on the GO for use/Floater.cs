using UnityEngine;
using System.Collections;

namespace uMMORPG
{
    
    public class Floater : MonoBehaviour
    {
        public float degreesPerSecond = 15.0f;
        public float amplitude = 0.25f;
        public float frequency = 0.5f;
    
        public bool enableInterval;
        [Range(0f, 3f)] public float updateInterval = 0.25f;
        protected float fInterval;
    
    
        private Vector3 posOffset;
        private Vector3 tempPos;
    
        private void Start()
        {
            posOffset = transform.localPosition;
        }
    
        private void Update()
        {
            if (Time.time > fInterval || !enableInterval)
            {
                SlowUpdate();
                fInterval = Time.time + updateInterval;
            }
        }
        private void SlowUpdate()
        {
            if (degreesPerSecond != 0)
                transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
    
            if (amplitude != 0 && frequency != 0)
            {
                tempPos = posOffset;
                tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
            }
    
            if (transform.localPosition != tempPos)
                transform.localPosition = tempPos;
        }
    }
    
}
