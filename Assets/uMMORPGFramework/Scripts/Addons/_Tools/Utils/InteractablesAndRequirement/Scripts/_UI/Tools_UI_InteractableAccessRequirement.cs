using UnityEngine;
using UnityEngine.UI;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // ===================================================================================
    // UI INTERACTABLE ACCESS REQUIREMENT
    // ===================================================================================
    public partial class Tools_UI_InteractableAccessRequirement : Tools_UI_Requirement
    {
        [Header("[COSTS]")]
        public string labelGoldCost = " - Gold cost per use: ";
        public string labelCoinCost = " - Coins cost per use: ";
    #if _iMMOHONORSHOP
        public string labelRequiredHonorCurrency 		= " - Honor Currency cost: ";
    #endif
    
        protected Tools_Interactable interactable;
    
        // -----------------------------------------------------------------------------------
        // Show
        // -----------------------------------------------------------------------------------
        public void Show(Tools_Interactable _interactable)
        {
            Player player = Player.localPlayer;
            if (!player) return;
    
            interactable = _interactable;
            requirements = interactable.interactionRequirements;
    
            for (int i = 0; i < content.childCount; ++i)
            {
                Destroy(content.GetChild(i).gameObject);
            }
    
            UpdateTextbox();
    
            interactButton.interactable = interactable.interactionRequirements.checkRequirements(player) || interactable.IsUnlocked();
    
            if (interactable.interactionText != "")
                interactButton.GetComponentInChildren<Text>().text = interactable.interactionText;
    
            interactButton.onClick.SetListener(() =>
            {
                interactable.ConfirmAccess();
                Hide();
            });
    
            cancelButton.onClick.SetListener(() =>
            {
                Hide();
            });
    
            panel.SetActive(true);
        }
    
        // -----------------------------------------------------------------------------------
        // Update
        // -----------------------------------------------------------------------------------
        protected void Update()
        {
            if (!panel.activeSelf) return;
    
            Player player = Player.localPlayer;
            if (!player) return;
    
            if (!UMMO_Tools.Tools_CheckSelectionHandling(interactable.gameObject))
            {
                Hide();
            }
        }
    
        // -----------------------------------------------------------------------------------
        // UpdateTextbox
        // -----------------------------------------------------------------------------------
        protected override void UpdateTextbox()
        {
            Player player = Player.localPlayer;
            if (!player) return;
    
            base.UpdateTextbox();
    
            // ------------ Costs
    
            Tools_InteractionRequirements ir = (Tools_InteractionRequirements)requirements;
    
            if (ir.requierementCost.goldCost > 0)
                AddMessage(labelGoldCost + ir.requierementCost.goldCost.ToString(), player.gold >= ir.requierementCost.goldCost ? textColor : errorColor);
    
            if (ir.requierementCost.coinCost > 0)
                AddMessage(labelCoinCost + ir.requierementCost.coinCost.ToString(), player.itemMall.coins >= ir.requierementCost.coinCost ? textColor : errorColor);
    
    #if _iMMOHONORSHOP
    		if (ir.requierementCost.honorCurrencyCost.Length > 0)
    		{
    			AddMessage(labelRequiredHonorCurrency, textColor);
    			foreach (HonorShopCurrencyCost currency in ir.requierementCost.honorCurrencyCost)
    			{
    				if (player.playerHonorShop.GetHonorCurrency(currency.honorCurrency) < currency.amount)
    				{
    					AddMessage(currency.honorCurrency.name + " x" + currency.amount.ToString(), errorColor);
    				}
    				else
    				{
    					AddMessage(currency.honorCurrency.name + " x" + currency.amount.ToString(), textColor);
    				}
    			}
    		}
    #endif
        }
    
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
