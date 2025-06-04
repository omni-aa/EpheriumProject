// Contains all the network messages that we need.
using System.Collections.Generic;
using System.Linq;
using Mirror;


namespace uMMORPG
{

    // client to server ////////////////////////////////////////////////////////////
    public partial struct LoginMsg : NetworkMessage
    {
        public string account;
        public string password;
#if _iMMOREGISTERMOBILE
    public LoginType loginType;
#endif
        public string version;
    }

    public partial struct CharacterCreateMsg : NetworkMessage
    {
        public string name;
        public int classIndex;
#if !_iMMO2D
        public bool gameMaster; // only allowed if host connection!
#endif
    }

    public partial struct CharacterSelectMsg : NetworkMessage
    {
        public int index;
    }

    public partial struct CharacterDeleteMsg : NetworkMessage
    {
        public int index;
    }

    // server to client ////////////////////////////////////////////////////////////
    // we need an error msg packet because we can't use TargetRpc with the Network-
    // Manager, since it's not a MonoBehaviour.
    public partial struct ErrorMsg : NetworkMessage
    {
        public string text;
        public bool causesDisconnect;
    }

    public partial struct LoginSuccessMsg : NetworkMessage
    {
    }

    public partial struct CharactersAvailableMsg : NetworkMessage
    {
        public partial struct CharacterPreview
        {
            public string name;
            public int level;
            public string className; // = the prefab name
#if !_iMMO2D
            public bool isGameMaster; // for nameoverlay prefix in preview!
#endif
#if _iMMOTRAITS
        public string classPlayer;
#endif
            public ItemSlot[] equipment;
        }
        public CharacterPreview[] characters;

        // load method in this class so we can still modify the characters structs
        // in the addon hooks
        public void Load(List<Player> players)
        {
            // we only need name, class, equipment for our UI
            // (avoid Linq because it is HEAVY(!) on GC and performance)
            characters = new CharacterPreview[players.Count];
            for (int i = 0; i < players.Count; ++i)
            {
                Player player = players[i];
                characters[i] = new CharacterPreview
                {
                    name = player.name,
                    level = player.level.current,
                    className = player.className,
#if !_iMMO2D
                    isGameMaster = player.isGameMaster,
#endif
#if _iMMOTRAITS
                classPlayer = (player.playerTraits.Traits.Count > 0) ? player.playerTraits.Traits[0].name : "undefined",
#endif
                    equipment = player.equipment.slots.ToArray()
                };
            }

            // addon system hooks (to initialize extra values like health if necessary)
            Utils.InvokeMany(typeof(CharactersAvailableMsg), this, "Load_", players);
        }
    }
}