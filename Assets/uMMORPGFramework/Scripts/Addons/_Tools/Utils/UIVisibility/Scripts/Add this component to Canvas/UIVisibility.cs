using System.Collections.Generic;
using UnityEngine;

namespace uMMORPG
{
    
    public class UIVisibility : MonoBehaviour
    {
        [Header("[-=-[ UI Visibility ]-=-]")]
        public KeyCode hotkey = KeyCode.Z;
        public KeyCode modifierKey = KeyCode.LeftAlt;
        public GameObject[] childObjects;
        private List<GameObject> hiddenPanels = new List<GameObject>();
        private bool modifierPressed = false;
        private float updateTime = 0;
    
        private void Update()
        {
            // Check if the modifier key was pressed and the hotkey.
            modifierPressed = Input.GetKey(modifierKey) ? true : false;
            if (modifierPressed && Input.GetKey(hotkey) && updateTime <= Time.time)
            {
                updateTime = Time.time + 0.5f;  // Create a wait TimeLogout before allowing this action again.
                                                // If we don't have hidden parent objects then hide the user interface.
                if (hiddenPanels.Count == 0)
                {
                    for (int i = 0; i < childObjects.Length; i++)
                        if (childObjects[i].activeSelf)
                        {
                            hiddenPanels.Add(childObjects[i]);
                            childObjects[i].SetActive(false);
                        }
                }
                // If we do have hidden parent objects then show them.
                else if (hiddenPanels.Count > 0)
                {
                    for (int x = 0; x < hiddenPanels.Count; x++)
                        hiddenPanels[x].SetActive(true);
    
                    hiddenPanels.Clear();
                }
            }
        }
    }
    
}
