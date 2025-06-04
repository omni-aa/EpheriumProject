using UnityEngine;

namespace uMMORPG
{
    
    public class NpcDistanceDisablePanel : MonoBehaviour
    {
        public GameObject panel;
    
        // Update is called once per frame
        void Update()
        {
            Player player = Player.localPlayer;
            if (!player) return;
    
            // use collider point(s) to also work with big entities
            if (player.target != null && player.target is Npc && Utils.ClosestDistance(player, player.target) <= player.interactionRange){}
            else panel.SetActive(false);
        }
    }
    
}
