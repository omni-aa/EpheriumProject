using UnityEngine;

namespace uMMORPG
{
    
    public class Tools_ClosestDistance : MonoBehaviour
    {
    #if _iMMO2D
        public static float ClosestDistance(Collider2D a, Collider2D b)
    #else
        public static float ClosestDistance(Collider a, Collider b)
    #endif
        {
            // return 0 if both intersect or if one is inside another.
            // ClosestPoint distance wouldn't be > 0 in those cases otherwise.
            if (a.bounds.Intersects(b.bounds))
                return 0;
    
            // Unity offers ClosestPointOnBounds and ClosestPoint.
            // ClosestPoint is more accurate. OnBounds often doesn't get <1 because
            // it uses a point at the top of the player collider, not in the center.
            // (use Debug.DrawLine here to see the difference)
    #if _iMMO2D
            return Vector2.Distance(a.ClosestPoint(b.transform.position),
                                    b.ClosestPoint(a.transform.position));
    #else
            return Vector3.Distance(a.ClosestPoint(b.transform.position),
                                    b.ClosestPoint(a.transform.position));
    #endif
        }
    }
    
}
