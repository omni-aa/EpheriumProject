namespace uMMORPG
{
    
    using UnityEngine;
    using UnityEngine.EventSystems;
    
    // FIELD TABBING
    
    public class FieldTabbing : MonoBehaviour
    {
        public KeyCode hotkey = KeyCode.Tab;
        public GameObject[] tabObjects;
    
        private int tabCount = 0;
    
        // -----------------------------------------------------------------------------------
        // Start
        // Set our first tabbale object as focus.
        // -----------------------------------------------------------------------------------
        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(tabObjects[tabCount], null);
        }
    
        // -----------------------------------------------------------------------------------
        // Update
        // If tab is hit while active, then move to the next tabbable object.
        // -----------------------------------------------------------------------------------
        private void Update()
        {
            if (Input.GetKeyDown(hotkey))
            {
                tabCount++;
                if (tabCount >= tabObjects.Length) tabCount = 0;
    
                EventSystem.current.SetSelectedGameObject(tabObjects[tabCount], null);
            }
        }
    }
    
}
