// Catches the Aggro Sphere's OnTrigger functions and forwards them to the
// Entity. Make sure that the aggro area's layer is IgnoreRaycast, so that
// clicking on the area won't select the entity.
//
// Note that a player's collider might be on the pelvis for animation reasons,
// so we need to use GetComponentInParent to find the Entity script.
//
// IMPORTANT: Monster.OnTriggerEnter would catch it too. But this way we don't
//            need to add OnTriggerEnter code to all the entity types that need
//            an aggro area. We can just reuse it.
//            (adding it to Entity.OnTriggerEnter would be strange too, because
//             not all entity types should react to OnTriggerEnter with aggro!)
using UnityEngine;

namespace uMMORPG
{
#if _iMMO2D
[RequireComponent(typeof(CircleCollider2D))] // aggro area trigger
#else
    [RequireComponent(typeof(SphereCollider))] // aggro area trigger
#endif
    public class AggroArea : MonoBehaviour
    {
        public Entity owner; // set in the inspector

        // same as OnTriggerStay
#if _iMMO2D
    void OnTriggerEnter2D(Collider2D co)
#else
        void OnTriggerEnter(Collider co)
#endif
        {
            HandleAggro(co);
        }

#if _iMMO2D
    void OnTriggerStay2D(Collider2D co)
#else
        void OnTriggerStay(Collider co)
#endif
        {
            HandleAggro(co);
        }

#if _iMMO2D
     private void HandleAggro(Collider2D co)
#else
        private void HandleAggro(Collider co)
#endif
        {
            Entity entity = co.GetComponentInParent<Entity>();

            if (entity != null && !(entity is Player) || (entity is Player p && Player.onlinePlayers.ContainsKey(p.name)))
            {
                owner.OnAggro(entity);
            }
        }
    }
}
