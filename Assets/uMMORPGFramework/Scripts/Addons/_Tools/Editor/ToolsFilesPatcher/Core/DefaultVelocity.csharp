// Sets the Rigidbody's velocity in Start().
using UnityEngine;

namespace uMMORPG
{

#if _iMMO2D
    [RequireComponent(typeof(Rigidbody2D))]
#else
    [RequireComponent(typeof(Rigidbody))]
#endif
    public class DefaultVelocity : MonoBehaviour
    {
#if _iMMO2D
        public Rigidbody2D rigidBody;
        public Vector2 velocity;
#else
        public Rigidbody rigidBody;
        public Vector3 velocity;
#endif

        void Start()
        {
#if UNITY_6000_0_OR_NEWER
            rigidBody.linearVelocity = velocity;
#else
            rigidBody.velocity = velocity;
#endif
        }
    }
}