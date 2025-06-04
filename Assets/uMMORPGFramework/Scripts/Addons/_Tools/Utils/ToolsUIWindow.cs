using UnityEngine;
using UnityEngine.EventSystems;

namespace uMMORPG
{
    
    public enum ToolsCloseOption
    {
        DoNothing,
        DeactivateWindow,
        DestroyWindow
    }
    
    public class ToolsUIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ToolsCloseOption onClose = ToolsCloseOption.DeactivateWindow;
        public static ToolsUIWindow currentlyDragged;
    
        private RectTransform rectTransform;
        private Vector2 offset;
    
        void Awake()
        {
            rectTransform = transform.parent.GetComponent<RectTransform>();
        }
    
        public void OnBeginDrag(PointerEventData d)
        {
            currentlyDragged = this;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, d.position, d.pressEventCamera, out offset);
            offset = rectTransform.anchoredPosition - offset;
        }
    
        public void OnDrag(PointerEventData d)
        {
            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, d.position, d.pressEventCamera, out localPointerPosition);
            rectTransform.anchoredPosition = localPointerPosition + offset;
            KeepInScreen();
        }
    
        public void OnEndDrag(PointerEventData d)
        {
            currentlyDragged = null;
        }
    
        public void OnClose()
        {
            rectTransform.SendMessage("OnWindowClose", SendMessageOptions.DontRequireReceiver);
    
            if (onClose == ToolsCloseOption.DeactivateWindow)
                rectTransform.gameObject.SetActive(false);
    
            if (onClose == ToolsCloseOption.DestroyWindow)
                Destroy(rectTransform.gameObject);
        }
    
        private void KeepInScreen()
        {
            Vector2 pos = rectTransform.anchoredPosition;
            Vector2 parentSize = (rectTransform.parent as RectTransform).rect.size;
            Vector2 rectSize = rectTransform.rect.size;
    
            pos.x = Mathf.Clamp(pos.x, rectSize.x * rectTransform.pivot.x, parentSize.x - rectSize.x * (1 - rectTransform.pivot.x));
            pos.y = Mathf.Clamp(pos.y, rectSize.y * rectTransform.pivot.y, parentSize.y - rectSize.y * (1 - rectTransform.pivot.y));
    
            rectTransform.anchoredPosition = pos;
        }
    }
    
}
