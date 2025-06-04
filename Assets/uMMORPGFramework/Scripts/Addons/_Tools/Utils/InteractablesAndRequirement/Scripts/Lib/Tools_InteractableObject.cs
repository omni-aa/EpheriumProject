using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if _iMMOTOOLS

namespace uMMORPG
{
    // ===================================================================================
    // INTERACTABLE OBJECT CLASS
    // ===================================================================================
    public partial class Tools_InteractableObject : Tools_Interactable
    {
    
        public enum InteractionRangeType { Any, Quadruple, Double, Normal, Half, Quarter }
    
        [Tooltip("[Optional] Interaction range limit?")]
        public InteractionRangeType interactionRangeType;
    
        [Tooltip("[Optional] Once a player accessed it, everybody can interact with it?")]
        public bool unlockPermanently = false;
    
        [Header("[COMPONENTS]")]
        //public new Collider collider;
    #pragma warning disable
    #if _iMMO2D
        public new Collider2D collider;
    #else
        public new Collider collider;
    #endif
    #pragma
        //public NetworkProximityGridChecker proxchecker;
        public Transform effectMount;
        public AudioSource audioSource;
    
        [Header("Events")]
        public UnityEventEntity onSelect; // called when clicking it the first TimeLogout
    
        // -----------------------------------------------------------------------------------
        // Start
        // -----------------------------------------------------------------------------------
        void OnMouseDown()
        {
            // joined world yet? (not character selection?)
            // not over UI? (avoid targeting through windows)
            // and in a state where local player can select things?
            if (Player.localPlayer != null && !Utils.IsCursorOverUserInterface() && (Player.localPlayer.state == "IDLE" || Player.localPlayer.state == "MOVING" || Player.localPlayer.state == "CASTING" || Player.localPlayer.state == "STUNNED"))
            {
                // clear requested skill in any case because if we clicked
                // somewhere else then we don't care about it anymore
                Player.localPlayer.useSkillWhenCloser = -1;
    
                // set indicator in any case
                // (not just the first TimeLogout, because we might have clicked on the
                //  ground in the mean TimeLogout. always set it when selecting.)
                Player.localPlayer.indicator.SetViaParent(transform);
    
                // clicked for the first TimeLogout: SELECT
                if (Player.localPlayer.target != this)
                {
    
                    Player.localPlayer.movement.Reset();
                    if (checkInteractionRange(Player.localPlayer))
                    {
                        Debug.Log("Selected " + this.name);
                    }
                    // target it
                    Player.localPlayer.CmdSetTarget(netIdentity);
    
                    // call OnSelect + hook
                    OnSelect(Player.localPlayer);
                    onSelect.Invoke(Player.localPlayer);
                }
            }
        }
    
        protected virtual void OnSelect(Player player) {
            if (isClient)
            {
                OnUpdateClient();
            }
            if (isServer)
            {
                OnUpdateServer();
            }
        }
        // -----------------------------------------------------------------------------------
        // OnUpdateClient
        // @Client
        // -----------------------------------------------------------------------------------
        [ClientCallback]
        public virtual void OnUpdateClient()
        {
            Player player = Player.localPlayer;
            if (!player) return;
    
            // -- check for interaction Distance
    #if !_iMMO2D
            this.GetComponentInChildren<SpriteRenderer>().enabled = UMMO_Tools.Tools_CheckSelectionHandling(this.gameObject);
    #endif
    
            // -- check for click
            if (UMMO_Tools.Tools_SelectionHandling(this.gameObject))
            {
                if (checkInteractionRange(player) && ((!interactionRequirements.hasRequirements() && !interactionRequirements.requierementCost.HasCosts()) || automaticActivation))
                {
                    // -- when no requirements & no costs: automatically
                    ConfirmAccess();
                }
                else if (checkInteractionRange(player) && interactionRequirements.checkState(player))
                {
                    // -- in any other case: show confirmation UI
                    Debug.Log("humm");
                    ShowAccessRequirementsUI();
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // OnUpdateServer
        // @Server
        // -----------------------------------------------------------------------------------
        [ServerCallback]
        public virtual void OnUpdateServer() { }
    
        // -----------------------------------------------------------------------------------
        // OnUnlock
        // @Server
        // -----------------------------------------------------------------------------------
        public void OnUnlock()
        {
            unlocked = unlockPermanently;
        }
    
        // -----------------------------------------------------------------------------------
        // OnLock
        // @Server
        // -----------------------------------------------------------------------------------
        public void OnLock()
        {
            unlocked = false;
        }
    
        // -----------------------------------------------------------------------------------
        // IsUnlocked
        // -----------------------------------------------------------------------------------
        public override bool IsUnlocked()
        {
            return unlocked;
        }
    
        // -----------------------------------------------------------------------------------
        // Show
        // -----------------------------------------------------------------------------------
        [Server]
        public void Show()
        {
            collider.enabled = true;
            netIdentity.enabled = false;
        }
    
        // -----------------------------------------------------------------------------------
        // Hide
        // -----------------------------------------------------------------------------------
        [Server]
        public void Hide()
        {
            collider.enabled = false;
            netIdentity.enabled = true;
        }
    
        // -----------------------------------------------------------------------------------
        // IsHidden
        // -----------------------------------------------------------------------------------
        public bool IsHidden()
        {
            return netIdentity.enabled;
        }
    
        // -----------------------------------------------------------------------------------
        // checkInteractionRange
        // -----------------------------------------------------------------------------------
        public bool checkInteractionRange(Player player)
        {
            if (interactionRangeType == InteractionRangeType.Any || collider == null) return true;
    
            float fInteractionRange = player.interactionRange;
    
            if (interactionRangeType == InteractionRangeType.Quadruple)
            {
                fInteractionRange *= 4;
            }
            else if (interactionRangeType == InteractionRangeType.Double)
            {
                fInteractionRange *= 2;
            }
            else if (interactionRangeType == InteractionRangeType.Half)
            {
                fInteractionRange *= 0.5f;
            }
            else if (interactionRangeType == InteractionRangeType.Quarter)
            {
                fInteractionRange *= 0.25f;
            }
            return Tools_ClosestDistance.ClosestDistance(player.collider, collider) <= fInteractionRange;
        }
    
        // =================================== HELPERS =======================================
    
        // -----------------------------------------------------------------------------------
        // SpawnEffect
        // Same as SpawnEffect that is found in skill effects of the core asset. It has been
        // put here because its required for almost every skill. Prevents duplicate code.
        // -----------------------------------------------------------------------------------
        public void SpawnEffect(GameObject effectObject, AudioClip effectSound = null)
        {
            if (effectObject != null)
            {
                GameObject go = Instantiate(effectObject, effectMount.position, Quaternion.identity);
                NetworkServer.Spawn(go);
            }
    
            if (effectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(effectSound);
            }
        }
    
        // -----------------------------------------------------------------------------------
        // PlaySound
        // -----------------------------------------------------------------------------------
        [ClientCallback]
        public void PlaySound(AudioClip effectSound = null)
        {
            if (effectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(effectSound);
            }
        }
    
        // -----------------------------------------------------------------------------------
        // IsWorthUpdating
        // -----------------------------------------------------------------------------------
        /*public override bool IsWorthUpdating()
        {
            return netIdentity.observers == null || netIdentity.observers.Count > 0 || IsHidden();
        }*/
    
        // -----------------------------------------------------------------------------------
    
    
}}
    #endif
