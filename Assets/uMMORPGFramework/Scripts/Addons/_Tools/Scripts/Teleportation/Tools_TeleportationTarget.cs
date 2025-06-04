using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // TELEPORTATION TARGET
    
    [System.Serializable]
    public partial class Tools_TeleportationTarget
    {
    
        public Transform targetPosition;
        //public string sceneTarget;
    /*
        [Header("[-=-[ Target Teleportation Info ]-=-]")]
        public string nameTeleportTarget;
    #if !_iMMO2D
        public Vector3 targetTeleportPosition;
    #else
        public Vector2 vectorTargetPosition;
    #endif
    
    
    #if !_iMMO2D
        public float GetDistance(Vector3 currentPosition)
    #else
        public float GetDistance(Vector2 position)
    #endif
        {
    
    #if !_iMMO2D
            return Vector3.Distance(targetTeleportPosition, currentPosition);
    #else
            return Vector2.Distance(targetPosition, currentPosition);
    #endif
        }
    */
        //private bool onWaitingTeleporting = false;
        // -----------------------------------------------------------------------------------
        // name
        // -----------------------------------------------------------------------------------
        public string name
        {
            get
            {
                if (targetPosition != null)
                    return targetPosition.name;
                else
                    return "";
            }
        }
    
        // -----------------------------------------------------------------------------------
        // getDistance
        // Returns the distance of the stated transform to the target
        // -----------------------------------------------------------------------------------
        public float getDistance(Transform transform)
        {
    #if !_iMMO2D
            return Vector3.Distance(targetPosition.position, transform.position);
    #else
            return Vector2.Distance(targetPosition.position, transform.position);
    #endif
        }
    
        // -----------------------------------------------------------------------------------
        // Valid
        // -----------------------------------------------------------------------------------
        public bool Valid
        {
            get
            {
                return targetPosition != null;
            }
        }
    
        // -----------------------------------------------------------------------------------
        // OnTeleport
        // @Server
        // -----------------------------------------------------------------------------------
        public void OnTeleport(Player player)
        {
            if (!player || !Valid) return;
            //if (sceneTarget != "")
            ///{
                //on passe a waiting T�l�port car on et pas sur la m�me scene
                //onWaitingTeleporting = true;
               // playerTo = player;
                //SceneLoaderAsync.Instance.LoadScene(sceneTarget);      <----------------------------------------------------- � d�coment�
                
            //    player.Tools_Warp(targetPosition.position);
                //if (onWaitingTeleporting)
                //{
                    //Debug.Log("on attend la le chargement complet de la map avant la t�l�portation t�l�portation");
                    /*if (SceneLoaderAsync.Instance.LoadingProgress == 100 && SceneLoaderAsync.Instance.LoadedScene)   < -----------------------------------------------------� d�coment�
                    {
                        Debug.Log("NetworManagerMMOForceDisconnect");
                        
                    }*/
                //}
            //}
            //else
            //{
                player.Tools_Warp(targetPosition.position);
            //}
        }
        // -----------------------------------------------------------------------------------
    
    }
    
}
    #endif
