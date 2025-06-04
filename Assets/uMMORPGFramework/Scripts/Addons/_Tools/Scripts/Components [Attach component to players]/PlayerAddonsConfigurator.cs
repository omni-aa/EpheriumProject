using Mirror;
using UnityEngine;

//using UnityEditor;
namespace uMMORPG
{
    [HideInInspector]
    public partial class PlayerAddonsConfigurator : NetworkBehaviour
    {
#if _iMMOTOOLS

    [Header("Component")]
    public Player player;
    public PlayerInventory inventory;
    public PlayerEquipment playerEquipment;
    public PlayerExperience playerExperience;
    public Health health;
    public Combat combat;

#if _CLIENT
    // -----------------------------------------------------------------------------------
    // OnStartClient
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Client]
    public override void OnStartClient()
    {
        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "OnStartClient_");
    }

    // -----------------------------------------------------------------------------------
    // OnStartLocalPlayer
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Client]
    public override void OnStartLocalPlayer()
    {
        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "OnStartLocalPlayer_");
    }
#endif

    // -----------------------------------------------------------------------------------
    // OnStartServer
    // @Server
    // -----------------------------------------------------------------------------------

#if _SERVER
    [Server]
    public override void OnStartServer()
    {
        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "OnStartServer_");
    }
#endif

    // -----------------------------------------------------------------------------------
    // Start
    // @Client && @Server
    // -----------------------------------------------------------------------------------
    public void Awake()
    {
#if _CLIENT
        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "Awake_Client_");
#endif
#if _SERVER
        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "Awake_Server_");
#endif
    }


    // -----------------------------------------------------------------------------------
    // Start
    // @Client && @Server
    // -----------------------------------------------------------------------------------
    public void Start()
    {
        Utils.InvokeMany(typeof(PlayerAddonsConfigurator), this, "Start_");
    }


    // -----------------------------------------------------------------------------------
    // Start
    // @Client && @Server
    // -----------------------------------------------------------------------------------
    private void Update()
    {
#if _SERVER && _iMMOFRIENDS
        Update_Friendship();
#endif

#if _iMMOBUILDSYSTEM
        Update_RTS_BuildSystem();
#endif
    }

    // -----------------------------------------------------------------------------------
    // Cmd_ShowAnimation
    // @Client && @Server
    // -----------------------------------------------------------------------------------
    private void LateUpdate()
    {
#if _CLIENT && _iMMOCHEST
        LateUpdate_LootCrate();
#endif

#if _CLIENT && _iMMOCHAIRS
        LateUpdate_Chair();
#endif

#if _CLIENT && _iMMODOORS
        LateUpdate_Doors();
#endif

#if _iMMOBUILDSYSTEM
        LateUpdate_RTS_BuildSystem();
#endif

    }

#endif
    }
}