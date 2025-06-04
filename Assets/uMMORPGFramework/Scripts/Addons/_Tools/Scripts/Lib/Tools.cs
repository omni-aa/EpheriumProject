using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace uMMORPG
{
    
    // TOOLS
    [Serializable] public class UnityEventInt : UnityEvent<Int32> { }
    
    public partial class UMMO_Tools
    {
    
        private const char CONST_DELIMITER = ';';
    #if _iMMOTOOLS
    
    
        // -----------------------------------------------------------------------------------
        // ReachableRandomUnitCircleOnNavMesh
        // -----------------------------------------------------------------------------------
    #if _iMMO2D
        public static Vector2 ReachableRandomUnitCircleOnNavMesh(Vector2 position, float radiusMultiplier, int solverAttempts = 3)
    #else
        public static Vector3 ReachableRandomUnitCircleOnNavMesh(Vector3 position, float radiusMultiplier, int solverAttempts = 3)
    #endif
        {
            for (int i = 0; i < solverAttempts; ++i)
            {
    #if _iMMO2D
                Vector2 candidate = RandomUnitCircleOnNavMesh(position, radiusMultiplier);
    #else
                Vector3 candidate = RandomUnitCircleOnNavMesh(position, radiusMultiplier);
    #endif
    #if _iMMO2D
                NavMeshHit2D hit;
                if (!NavMesh2D.Raycast(position, candidate, out hit, NavMesh.AllAreas))
    #else
                NavMeshHit hit;
                if (!NavMesh.Raycast(position, candidate, out hit, NavMesh.AllAreas))
    #endif
                    return candidate;
            }
    
            return position;
        }
    
        // -----------------------------------------------------------------------------------
        // RandomUnitCircleOnNavMesh
        // -----------------------------------------------------------------------------------
    #if _iMMO2D
        public static Vector2 RandomUnitCircleOnNavMesh(Vector2 position, float radiusMultiplier)
    #else
        public static Vector3 RandomUnitCircleOnNavMesh(Vector3 position, float radiusMultiplier)
    #endif
        {
            Vector2 r = UnityEngine.Random.insideUnitCircle * radiusMultiplier;
    
    #if _iMMO2D
            Vector2 randomPosition = new Vector2(position.x + r.x, position.y + r.y);
    #else
            Vector3 randomPosition = new Vector3(position.x + r.x, position.y, position.z + r.y);
    #endif
    
    
    #if _iMMO2D
            NavMeshHit2D hit;
            if (NavMesh2D.SamplePosition(randomPosition, out hit, radiusMultiplier * 2, NavMesh.AllAreas))
    #else
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, radiusMultiplier * 2, NavMesh.AllAreas))
    #endif
                return hit.position;
            return position;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_SelectionHandling
        // V�rifie si le client a cliqu� sur un objet cliquable dans la sc�ne (pas sur l'interface utilisateur)
        // @Client
        // -----------------------------------------------------------------------------------
        public static bool Tools_SelectionHandling(GameObject target)
        {
            Player player = Player.localPlayer;
            if (!player) return false;
            
            if (player.isAlive && Input.GetMouseButtonDown(0) && !Utils.IsCursorOverUserInterface() && Input.touchCount <= 1)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
    
    
    #if _iMMO2D
                RaycastHit2D hit;
                bool cast = player.localPlayerClickThrough ? hit = Utils.Raycast2DWithout(ray, player.gameObject) : hit = Physics2D.GetRayIntersection(ray);
    #else
                RaycastHit hit;
                bool cast = player.localPlayerClickThrough ? Utils.RaycastWithout(ray.origin, ray.direction, out hit, Mathf.Infinity, player.gameObject) : Physics.Raycast(ray, out hit);
    #endif
                if (cast && hit.transform.gameObject == target)
                {
                    return true;
                }
            }
    
            return false;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CheckSelectionHandling
        // Valide la plage d'interaction, l'�tat du joueur et si le joueur est vivant ou non
        // @Client OR @Server
        // -----------------------------------------------------------------------------------
        public static bool Tools_CheckSelectionHandling(GameObject target, Player localPlayer = null)
        {
            if (localPlayer == null)
                localPlayer = Player.localPlayer;
    
            if (!localPlayer || !target) return false;
            
    
            return localPlayer.isAlive &&
                    (
    #if _iMMO2D
                    (target.GetComponent<Collider2D>() && Tools_ClosestDistance.ClosestDistance(localPlayer.collider, target.GetComponent<Collider2D>()) <= localPlayer.interactionRange) ||
    #else
                    (target.GetComponent<Collider>() && Tools_ClosestDistance.ClosestDistance(localPlayer.collider, target.GetComponent<Collider>()) <= localPlayer.interactionRange) ||
    #endif
                    (target.GetComponent<Entity>() && Tools_ClosestDistance.ClosestDistance(localPlayer.collider, target.GetComponent<Entity>().collider) <= localPlayer.interactionRange)
                    );
        }
    
        // -----------------------------------------------------------------------------------
        // FindOnlinePlayerByName
        // -----------------------------------------------------------------------------------
        public static Player FindOnlinePlayerByName(string playerName)
        {
            if (!string.IsNullOrWhiteSpace(playerName))
            {
                if (Player.onlinePlayers.ContainsKey(playerName))
                {
                    return Player.onlinePlayers[playerName].GetComponent<Player>();
                }
            }
            return null;
        }
        // -----------------------------------------------------------------------------------
        // IntArrayToString
        // -----------------------------------------------------------------------------------
        public static string IntArrayToString(int[] array)
        {
            if (array == null || array.Length == 0) return null;
            string arrayString = "";
            for (int i = 0; i < array.Length; i++)
            {
                arrayString += array[i].ToString();
                if (i < array.Length - 1)
                    arrayString += CONST_DELIMITER;
            }
            return arrayString;
        }
    
        // -----------------------------------------------------------------------------------
        // IntStringToArray
        // -----------------------------------------------------------------------------------
        public static int[] IntStringToArray(string array)
        {
            if (string.IsNullOrWhiteSpace(array)) return null;
            string[] tokens = array.Split(CONST_DELIMITER);
            int[] arrayInt = Array.ConvertAll<string, int>(tokens, int.Parse);
            return arrayInt;
        }
    
        // -------------------------------------------------------------------------------
        // ArrayContains
        // -------------------------------------------------------------------------------
        public static bool ArrayContains(int[] defines, int define)
        {
            foreach (int def in defines)
            {
                if (def == define)
                    return true;
            }
            return false;
        }
        // -----------------------------------------------------------------------------------
    
    
        public static EntityType GetEntityType(Entity entity)
        {
            EntityType inCheck;
    
            if (entity is Player)       inCheck = EntityType.Player;
            else if (entity is Npc)     inCheck = EntityType.Npc;
            else if (entity is Monster) inCheck = EntityType.Monster;
            else if (entity is Pet)     inCheck = EntityType.Pet;
            else if (entity is Mount)   inCheck = EntityType.Mount;
            else                        inCheck = EntityType.Monster;
    
            return inCheck;
        }
    
    #endif
    
    
        // -------------------------------------------------------------------------------
        // ArrayContains
        // -------------------------------------------------------------------------------
        public static bool ArrayContains(string[] defines, string define)
        {
            foreach (string def in defines)
            {
                if (def == define)
                    return true;
            }
            return false;
        }
    
        // -------------------------------------------------------------------------------
        // RemoveFromArray
        // -------------------------------------------------------------------------------
        public static string[] RemoveFromArray(string[] defines, string define)
        {
            return defines.Where(x => x != define).ToArray();
        }
    
    }
    
}
