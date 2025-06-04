using Mirror;

namespace uMMORPG
{
    
    // PLAYER
    public partial class Player
    {
        protected UIMinimap minimap;
    
        // -----------------------------------------------------------------------------------
        // Tools_MinimapSceneText
        // @Server
        // -----------------------------------------------------------------------------------
        [ServerCallback]
        public void Tools_MinimapSceneText(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            Target_Tools_MinimapSceneText(connectionToClient, name);
        }
    
        // -----------------------------------------------------------------------------------
        // Target_Tools_MinimapSceneText
        // @Server -> @Client
        // -----------------------------------------------------------------------------------
        [TargetRpc]
        public void Target_Tools_MinimapSceneText(NetworkConnection target, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
    
            if (minimap == null)
                minimap = FindFirstObjectByType<UIMinimap>();
                //minimap = FindObjectOfType<UIMinimap>();
    
            if (minimap != null && minimap.sceneText != null)
                minimap.sceneText.text = name;
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
