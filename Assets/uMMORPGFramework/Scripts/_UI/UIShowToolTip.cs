﻿// Instantiates a tooltip while the cursor is over this UI element.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace uMMORPG
{
    public class UIShowToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject tooltipPrefab;
        [TextArea(1, 30)] public string text = "";

        // instantiated tooltip
        GameObject current;

        void CreateToolTip()
        {
            // instantiate
            current = Instantiate(tooltipPrefab, transform.position, Quaternion.identity);

            // put to foreground
            current.transform.SetParent(transform.root, true); // canvas
            current.transform.SetAsLastSibling(); // last one means foreground

            // set text immediately. don't wait for next Update(), otherwise there
            // is 1 frame with a small tooltip without a text which is odd.
            current.GetComponentInChildren<Text>().text = text;
        }

        void ShowToolTip(float delay)
        {
            Invoke(nameof(CreateToolTip), delay);
        }

        // helper function to check if the tooltip is currently shown
        // -> useful to only calculate item/skill/etc. tooltips when really needed
        public bool IsVisible() => current != null;

        void DestroyToolTip()
        {
            // stop any running attempts to show it
            CancelInvoke(nameof(CreateToolTip));

            // destroy it
            Destroy(current);
        }

        public void OnPointerEnter(PointerEventData d)
        {
            ShowToolTip(0.5f);
        }

        public void OnPointerExit(PointerEventData d)
        {
            DestroyToolTip();
        }

        void Update()
        {
            // always copy text to tooltip. it might change dynamically when
            // swapping items etc., so setting it once is not enough.
            if (current) current.GetComponentInChildren<Text>().text = text;
        }

        void OnDisable()
        {
            DestroyToolTip();
        }

        void OnDestroy()
        {
            DestroyToolTip();
        }
    }
}