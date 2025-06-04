using System.Collections.Generic;
using Mirror;

#if _iMMOTOOLS

namespace uMMORPG
{
    public partial struct CharacterCreateMsg : NetworkMessage
    {
    #if _iMMOUMACHARACTERS
        public string dna;
    #endif
    }
    
    public partial struct CharactersAvailableMsg
    {
    #pragma warning disable CS0282
        public partial struct CharacterPreview
        {
    #if _iMMOUMACHARACTERS
            public string umaDna;
    #endif
    
    #if _iMMOSCENELOADER || _iMMOLOBBY
            public string startingScene;
    #endif
        }
    #pragma warning restore CS0282
    
        private void Load_Tools(List<Player> players)
        {
            for (int i = 0; i < players.Count; ++i)
            {
    #if _iMMOUMACHARACTERS
                if (players[i].playerAddonsConfigurator.tmpl_UMACharacterCreation)
                    characters[i].umaDna = players[i].playerAddonsConfigurator.umaDna;
    #endif
    
    #if _iMMOLOBBY
                if (players[i].playerAddonsConfigurator.sceneList)
                    characters[i].startingScene = (players[i].playerAddonsConfigurator.currentScene.SceneName != null) ? players[i].playerAddonsConfigurator.currentScene.SceneName : players[i].playerAddonsConfigurator.startingScene.SceneName;
                // if (players[i].playerNetworkLobby)
                //    characters[i].startingScene = (players[i].playerNetworkLobby.currentScene.SceneName != null) ? players[i].playerNetworkLobby.currentScene.SceneName : players[i].playerNetworkLobby.startingScene.SceneName;
    #endif
            }
        }
    }
    
}
    #endif
