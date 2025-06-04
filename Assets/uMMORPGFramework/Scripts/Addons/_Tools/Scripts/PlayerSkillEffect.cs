using UnityEngine;
using Mirror;

namespace uMMORPG
{
    
    public class PlayerSkillEffect : SkillEffect
    {
        void Update()
        {
            
            // follow the target's position (because we can't make a NetworkIdentity
            // a child of another NetworkIdentity)
            //if (caster != null)
                transform.position = caster.collider.bounds.center;
    
            // destroy self if target disappeared or particle ended
            if (isServer)
                if (!GetComponent<ParticleSystem>().IsAlive())
                    NetworkServer.Destroy(gameObject);
        }
    }
    
}
