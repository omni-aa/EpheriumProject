using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR && _iMMOTOOLS

namespace uMMORPG
{
    
    public partial class UICrafting : MonoBehaviour
    {
        // Champ public pour le prefab
        ///public UI_UniversalSlot ingredientSlotPrefab;
    
        private void OnValidate()
        {
            // Vérification automatique dans l'éditeur
            if (ingredientSlotPrefab == null)
            {
                TryAssignDefaultPrefab();
            }
        }
    
        private void TryAssignDefaultPrefab()
        {
    #if UNITY_EDITOR
            string prefabName = "SlotUniversalCraftingIngredient"; // Nom du prefab par défaut
    
            // Recherche le prefab dans les assets
            string[] guids = AssetDatabase.FindAssets(prefabName + " t:Prefab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
    
                if (selectedPrefab != null)
                {
                    // Vérifie si le prefab contient le composant attendu
                    UI_UniversalSlot universalSlot = selectedPrefab.GetComponent<UI_UniversalSlot>();
                    if (universalSlot != null)
                    {
                        ingredientSlotPrefab = universalSlot;
                        EditorUtility.SetDirty(this);
                        Debug.Log("Automatically assigned prefab: " + selectedPrefab.name, this);
                    }
                    else
                    {
                        Debug.LogWarning("The found prefab does not have a UI_UniversalSlot component.", this);
                    }
                }
            }
            else
            {
                Debug.LogWarning("No prefab found with the specified name: " + prefabName, this);
            }
    #endif
        }
    }
    
    [CustomEditor(typeof(UICrafting))]
    public partial class UICraftingEditor : Editor
    {
        private string prefabName = "SlotUniversalCraftingIngredient"; // Nom du prefab par défaut
    
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
    
            UICrafting uiCrafting = (UICrafting)target;
    
            // Vérifier si le champ ingredientSlotPrefab est assigné
            bool isAssigned = uiCrafting.ingredientSlotPrefab != null;
    
            // Définir la couleur et le texte du bouton selon l'état d'assignation
            GUI.backgroundColor = isAssigned ? Color.green : Color.red;
            string buttonText = isAssigned ? "Slot Prefab (Universal) is assigned!" : "Assign Slot Prefab (Universal)";
    
            if (GUILayout.Button(buttonText))
            {
                if (isAssigned)
                {
                    Debug.Log("Slot Prefab (Universal) is already assigned: " + uiCrafting.ingredientSlotPrefab.name);
                }
                else
                {
                    AssignPrefab(uiCrafting);
                }
            }
    
            // Réinitialiser la couleur
            GUI.backgroundColor = Color.white;
        }
    
        private void AssignPrefab(UICrafting uiCrafting)
        {
            // Rechercher le prefab par son nom
            string[] guids = AssetDatabase.FindAssets(prefabName + " t:Prefab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject selectedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
    
                if (selectedPrefab != null)
                {
                    // Vérifier si le prefab contient un composant UI_UniversalSlot
                    UI_UniversalSlot universalSlot = selectedPrefab.GetComponent<UI_UniversalSlot>();
                    if (universalSlot != null)
                    {
                        // Assigner le prefab si le composant est correct
                        uiCrafting.ingredientSlotPrefab = universalSlot;
                        EditorUtility.SetDirty(uiCrafting);
                        Debug.Log("Prefab assigned: " + selectedPrefab.name);
                    }
                    else
                    {
                        Debug.LogWarning("The selected prefab does not have a UI_UniversalSlot component.");
                    }
                }
                else
                {
                    Debug.LogWarning("Prefab not found with the specified name: " + prefabName);
                }
            }
            else
            {
                Debug.LogWarning("No prefab found with the specified name: " + prefabName);
            }
        }
    }
    
}
    #endif
