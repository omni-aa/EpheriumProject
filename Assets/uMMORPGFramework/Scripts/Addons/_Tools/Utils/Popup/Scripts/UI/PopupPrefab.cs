using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace uMMORPG
{
    
    public class PopupPrefab : MonoBehaviour
    {
        public TMP_Text messageText; // Texte du message
        public Image iconSprite;
        public AudioSource audioSource;
    
        public Action OnClose;  // Événement déclenché lors de la fermeture
    
        // Méthode pour définir le message
        public void SetMessage(string message, byte iconId = 0, byte soundId = 0)
        {
    #if _iMMOTOOLS
            Player player = Player.localPlayer;
            if (!player) return;
    
            // Changer le message du popup
            messageText.text = message;
    
            // on vérifie que la configuration des popup est ajouter au joueurs
            if (player.playerAddonsConfigurator.popupConfiguration != null)
            {
                // Charger et appliquer l'icône (à partir d'une liste ou d'un gestionnaire de ressources)
                if (iconId > 0 && iconId <= player.playerAddonsConfigurator.popupConfiguration.availableIcons.Length)
                {
                    iconSprite.sprite = player.playerAddonsConfigurator.popupConfiguration.availableIcons[iconId];
                }
                // Jouer le son (à partir d'une liste ou d'un gestionnaire audio)
                if (soundId > 0 && soundId <= player.playerAddonsConfigurator.popupConfiguration.availableSounds.Length)
                {
                    audioSource.PlayOneShot(player.playerAddonsConfigurator.popupConfiguration.availableSounds[soundId-1]);
                }
            }
            else
            {
                GameLog.LogWarning("PopupConfiguration is not assigned in " + player.className + " prefab");
            }
    #endif
        }
    
        // Méthode appelée au clic pour fermer le popup
        public void OnClick()
        {
            OnClose?.Invoke(); // Notifie le PopupManager
            Destroy(gameObject); // Détruit le popup
        }
    }
    
}
