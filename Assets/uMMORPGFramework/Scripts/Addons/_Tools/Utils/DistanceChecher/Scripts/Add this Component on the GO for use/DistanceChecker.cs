using UnityEngine;

namespace uMMORPG
{
    
    // DISTANCE CHECKER
    [RequireComponent(typeof(SphereCollider))]
    public partial class DistanceChecker : MonoBehaviour
    {
        [Header("[-=-[ DISTANCE CHECKER ]-=-]")]
        [Tooltip("Maximum distance = (sphere collider raduis), allows to deactivate GameObject if the player is not in the collider")]
        [Range(1, 999)] public int maxDistance = 50;
    
        public GameObject[] ListGameObject;
        public SphereCollider colliderTriger;
    
    
        private void Start()
        {
            //Disable ALl GameObject if enabled
            foreach (GameObject obj in ListGameObject)
            {
                obj.SetActive(false);
            }
            colliderTriger.radius = maxDistance;
            colliderTriger.isTrigger = true;
        }
    
    
        private void OnTriggerEnter(Collider other)
        {
            foreach(GameObject obj in ListGameObject)
            {
                obj.SetActive(true);
            }
        }
    
        private void OnTriggerExit(Collider other)
        {
            foreach (GameObject obj in ListGameObject)
            {
                obj.SetActive(false);
            }
        }
        // -----------------------------------------------------------------------------------*/
    }
    
}
