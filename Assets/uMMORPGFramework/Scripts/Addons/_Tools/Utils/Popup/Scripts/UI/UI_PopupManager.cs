using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uMMORPG
{
    
    public class UI_PopupManager : MonoBehaviour
    {
        public GameObject popupPrefab; // Référence au prefab du popup
        private Queue<PopupData> popupQueue = new();
        private bool isPopupActive = false;
        public float popupDisplayTime = 2f; // Durée d'affichage par défaut en secondes
    
    
        // Classe pour représenter les données d'un popup
        private class PopupData
        {
            public string message;
            [Range(0, 256)] public byte soundID;
            [Range(0, 256)] public byte iconID;
    
            public PopupData(string message, byte soundID, byte iconID)
            {
                this.message = message;
                this.soundID = soundID;
                this.iconID = iconID;
            }
        }
    
    
        // Méthode pour ajouter un message à la file d'attente
        public void EnqueuePopup(string message, byte soundID, byte iconID)
        //public void EnqueuePopup(PopupData message)
        {
            popupQueue.Enqueue(new PopupData(message, soundID, iconID));
            if (!isPopupActive)
            {
                ShowNextPopup();
            }
        }
    
        // Affiche le prochain popup dans la file
        private void ShowNextPopup()
        {
            if (popupQueue.Count > 0)
            {
                isPopupActive = true;
                PopupData message = popupQueue.Dequeue();
                StartCoroutine(DisplayPopup(message));
            }
            else
            {
                isPopupActive = false;
            }
        }
    
        // Coroutine pour afficher le popup et gérer sa fermeture
        private IEnumerator DisplayPopup(PopupData data)
        {
            GameObject popup = Instantiate(popupPrefab, transform);
            PopupPrefab popupScript = popup.GetComponent<PopupPrefab>();
            popupScript.SetMessage(data.message , data.iconID, data.soundID);
            popupScript.OnClose += PopupClosed; // Écoute la fermeture
    
            // Attendre la durée par défaut ou la fermeture manuelle
            yield return new WaitForSeconds(popupDisplayTime);
    
            // Si le popup n'a pas été fermé manuellement
            if (popup != null)
            {
                Destroy(popup);
                PopupClosed();
            }
        }
    
        // Méthode appelée à la fermeture d'un popup
        private void PopupClosed()
        {
            isPopupActive = false;
            ShowNextPopup();
        }
    }
    
}
