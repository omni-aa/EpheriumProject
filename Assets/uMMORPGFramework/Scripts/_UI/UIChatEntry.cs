﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace uMMORPG
{
    public class UIChatEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Text text;

        // keep all the message info in case it's needed to reply etc.
        [HideInInspector] public ChatMessage message;
        public FontStyle mouseOverStyle = FontStyle.Italic;
        FontStyle defaultStyle;

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            // can we reply to this message?
            if (!string.IsNullOrWhiteSpace(message.replyPrefix))
            {
                defaultStyle = text.fontStyle;
                text.fontStyle = mouseOverStyle;
            }
        }

        //Detect when Cursor leaves the GameObject
        public void OnPointerExit(PointerEventData pointerEventData)
        {
            text.fontStyle = defaultStyle;
        }

        public void OnPointerClick(PointerEventData data)
        {
            // find the chat component in the parents
            GetComponentInParent<UIChat>().OnEntryClicked(this);
        }
    }
}