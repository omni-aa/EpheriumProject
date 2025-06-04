using UnityEngine;
using UnityEngine.UI;

namespace uMMORPG
{
    
    public class UI_Button_event : MonoBehaviour
    {
        // Start is called before the first frame update
        public Button button;
        public GameObject panelTarget;
    #pragma warning disable CS0649
        [SerializeField] private bool gameMaster;
    #pragma warning restore CS0649
        private void Start()
        {
            button.onClick.AddListener(ShowHide);
        }
        public void ShowHide()
        {
    #if _CLIENT
            panelTarget.SetActive((!panelTarget.activeSelf));
    #endif
        }
    #if !_iMMO2D && _iMMOTOOLS
        private void OnEnable()
        {
            if (gameMaster)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    button.gameObject.SetActive(player.isGameMaster);
                }
            }
        }
    #endif
    }
    
}
