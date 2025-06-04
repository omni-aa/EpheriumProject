// The Npc class is rather simple. It contains state Update functions that do
// nothing at the moment, because Npcs are supposed to stand around all day.
//
// Npcs first show the welcome text and then have offers for item trading and
// quests.
using Mirror;
using TMPro;
using UnityEngine;

namespace uMMORPG
{

#if _iMMO2D
[RequireComponent(typeof(NetworkNavMeshAgent2D))]
#else
    [RequireComponent(typeof(NavMeshMovement))]
    [RequireComponent(typeof(NetworkNavMeshAgent))]
#endif
    // offer components are optional. don't [RequireComponent] them!
    public partial class Npc : Entity
    {
        [Header("Components")]
        public NpcGuildManagement guildManagement;
        public NpcQuests quests;
        public NpcRevive revive;
        public NpcTrading trading;
        public NpcTeleport teleport;

        [Header("Welcome Text")]
        [TextArea(1, 30)] public string welcome;

        // cache all NpcOffers
        [HideInInspector] public NpcOffer[] offers;

#if !_iMMO2D
        public TMP_Text displayName;
        // Required to keep track of how many players are within this Npcs interaction range
        [SyncVar, HideInInspector] public int accessingPlayers = 0;
#endif

        void Awake()
        {
            offers = GetComponents<NpcOffer>();
        }

        // finite state machine states /////////////////////////////////////////////
#if !_iMMONPCEXTENDED
        [Server] protected override string UpdateServer() { return state; }
        [Client] protected override void UpdateClient() { }

        // attack //////////////////////////////////////////////////////////////////
        public override bool CanAttack(Entity entity) { return false; }

        // interaction /////////////////////////////////////////////////////////////
        protected override void OnInteract()
        {
            Player player = Player.localPlayer;

            // alive, close enough? then talk
            // use collider point(s) to also work with big entities
            if (health.current > 0 &&
                Utils.ClosestDistance(player, this) <= player.interactionRange)
            {
                UINpcDialogue.singleton.Show();
            }
            // otherwise walk there
            // use collider point(s) to also work with big entities
            else
            {
#if _iMMO2D
            Vector2 destination = Utils.ClosestPoint(this, player.transform.position);
#else
                Vector3 destination = Utils.ClosestPoint(this, player.transform.position);
#endif
                player.movement.Navigate(destination, player.interactionRange);
            }
        }
#endif
    }
}
	