using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // PLAYER
    [RequireComponent(typeof(PlayerAddonsConfigurator))]
    public partial class Player
    {
    
        protected double Tools_timer;
        [HideInInspector] public bool Tools_timerRunning;
        protected int Tools_activeTasks;
        protected Tools_InfoBox Tools_infobox;
        protected Tools_UI_CastBar Tools_castbar;
        protected Tools_UI_Prompt Tools_popup;
    
        // ================================== AOE RELATED ====================================
    
        // -----------------------------------------------------------------------------------
        // Tools_GetCorrectedTargetsInSphere
        // Returns a list of all legal targets within a sphere around the caster. Comes
        // with several targeting options.
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public override List<Entity> Tools_GetCorrectedTargetsInSphere(Transform origin, float fRadius, bool deadOnly = false, bool affectSelf = false, bool affectOwnParty = false, bool affectOwnGuild = false, bool affectOwnRealm = false, bool reverseTargets = false, bool affectPlayers = false, bool affectNpcs = false, bool affectMonsters = false, bool affectPets = false)
        {
            List<Entity> correctedTargets = new List<Entity>();
    
    #if _iMMO2D
            Collider2D[] colliders = Physics2D.OverlapCircleAll(origin.position, fRadius);
    #else
            Collider[] colliders = Physics.OverlapSphere(origin.position, fRadius);
    #endif
    
    #if _iMMO2D
            foreach (Collider2D co in colliders)
    #else
            foreach (Collider co in colliders)
    #endif
            {
                Entity candidate = co.GetComponentInParent<Entity>();
    
                if (candidate != null && !correctedTargets.Any(x => x == candidate))
                {
                    bool bValidTarget = false;
    
                    if (candidate is Player)
                        bValidTarget = ((Player)this).Tools_SameCheck((Player)candidate, affectSelf, affectPlayers, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargets);
                    // Monster Test
                    else if (affectMonsters && candidate is Monster)
                    {
                        if (affectOwnRealm && reverseTargets)
                            bValidTarget = !Tools_SameRealm(candidate);
                        else if (affectOwnRealm)
                            bValidTarget = Tools_SameRealm(candidate);
                        else if (reverseTargets)
                            bValidTarget = false;
                        else
                            bValidTarget = true;
                    }
                    // Npc Test
                    else if (affectNpcs && candidate is Npc)
                    {
                        if (affectOwnRealm && reverseTargets)
                            bValidTarget = !Tools_SameRealm(candidate);
                        else if (affectOwnRealm)
                            bValidTarget = Tools_SameRealm(candidate);
                        else if (reverseTargets)
                            bValidTarget = false;
                        else
                            bValidTarget = true;
                    }
                    // Pet Test
                    else if (affectPets && candidate is Pet pet)
                    {
                        Entity owner = pet.owner;
                        if (affectOwnRealm && reverseTargets)
                            bValidTarget = !Tools_SameRealm(owner);
                        else if (affectOwnRealm)
                            bValidTarget = Tools_SameRealm(owner);
                        else if (reverseTargets)
                            bValidTarget = false;
                        else
                            bValidTarget = true;
                    }
    
                    if (bValidTarget)
                    {
                        if ((deadOnly && !candidate.isAlive) || (!deadOnly && candidate.isAlive))
                            correctedTargets.Add(candidate);
                    }
                }
            }
    
            return correctedTargets;
        }
    
    
        // -----------------------------------------------------------------------------------
        // Tools_GetCorrectedTargets
        // Returns a booleen if targets is corrects
        // with several targeting options.
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public bool Tools_GetCorrectedTargets(Entity target, bool deadOnly = false, bool affectSelf = false, bool affectOwnParty = false, bool affectOwnGuild = false, bool affectOwnRealm = false, bool reverseTargets = false, bool affectPlayers = false, bool affectNpc = false, bool affectMonsters = false, bool affectPets = false)
        {
            Entity candidate = target.GetComponentInParent<Entity>();
            bool bValidTarget = false;
            if (candidate != null)
            {
                // Player Test
                if (candidate is Player)
                    bValidTarget = Tools_SameCheck((Player)candidate, affectSelf, affectPlayers, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargets);
    
                // Monster Test
                else if (affectMonsters && candidate is Monster)
                {
                    if (affectOwnRealm && reverseTargets)
                        bValidTarget = !Tools_SameRealm(candidate);
                    else if (affectOwnRealm)
                        bValidTarget = Tools_SameRealm(candidate);
                    else if (reverseTargets)
                        bValidTarget = false;
                    else
                        bValidTarget = true;
                }
    
                // Npc Test
                else if (affectNpc && candidate is Npc)
                {
                    if (affectOwnRealm && reverseTargets)
                    {
                        bValidTarget = !Tools_SameRealm(candidate);
                    }
                    else if (affectOwnRealm)
                    {
                        bValidTarget = Tools_SameRealm(candidate);
                    }
                    else if (reverseTargets)
                    {
                        bValidTarget = false;
                    }
                    else
                        bValidTarget = true;
                }
    
                // Pet Test
                else if (affectPets && candidate is Pet pet)
                {
                    Entity owner = pet.owner;
                    if (affectOwnRealm && reverseTargets)
                    {
                        bValidTarget = !Tools_SameRealm(owner);
                    }
                    else if (affectOwnRealm)
                    {
                        bValidTarget = Tools_SameRealm(owner);
                    }
                    else if (reverseTargets)
                    {
                        bValidTarget = false;
                    }
                    else
                        bValidTarget = true;
                }
            }
            else
            {
                bValidTarget = false;
            }
    
            return bValidTarget;
        }
    
    
        public void clearindicator()
        {
    
                if (localPlayer && localPlayer.target == null)
                {
                    localPlayer.indicator.Clear();
                }
    
        }
       
        // ========================== Tools SELECTION HANDLING ===============================
        // Custom selection handling script that is used in combination with interactable
        // objects. Allows to break from the default selection handling that only allows to
        // interact with Entities.
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // Cmd_Tools_OnInteractServer
        // @Client -> @Server
        // -----------------------------------------------------------------------------------
        [Command]
        public void Cmd_Tools_OnInteractServer(GameObject go)
        {
    #if _SERVER
            if (!go.TryGetComponent<Tools_Interactable>(out var interactable))
                return;
    
            interactable.interactionRequirements.requierementCost.PayCost(this);
            interactable.interactionRequirements.GrantRewards(this);
    
            if (go.TryGetComponent<Tools_InteractableObject>(out var interactableObject))
                interactableObject.OnUnlock();
    
            interactable.OnInteractServer(this);
    #endif
        }
    
    
        // ============================ SELECTION HANDLING ===================================
        // Replaces the selection handling check of the core asset with custom functions
        // because it is possible to add more checks here later, but not to the core asset
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // Tools_SelectionHandling_Npc
        // Replaces the built-in selection handling for NPCs
        // -----------------------------------------------------------------------------------
        public bool Tools_SelectionHandling_Npc(Entity entity)
        {
            bool valid = true;
    
            valid = entity is Npc && entity.isAlive;
    
    #if _iMMONPCRESTRICTIONS
            if (entity is Npc npc)
                valid = npc.npcRestrictions.ValidateNpcRestrictions(this) && valid;
    #endif
    
            return valid;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_SelectionHandling_DeadMonster
        // Replaces the built-in selection handling for Dead Monsters
        // -----------------------------------------------------------------------------------
        public bool Tools_SelectionHandling_DeadMonster(Entity entity)
        {
            bool valid = true;
    
            valid = entity is Monster && !entity.isAlive;
    
    #if _iMMOLOOTRULES
            if (entity is Monster monster && monster.monsterLootRules)
                valid = monster.monsterLootRules.ValidateTaggedLooting(this) && valid;
    #endif
    
            return valid;
        }
    
        // ================================ GROUP CHECKS =====================================
        // Group targeting check that allows to detect if the target is of the same Party,
        // Guild or Realm as the caster. Can also reverse the targeting process to only target
        // members that are NOT of those groups.
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // Tools_SameCheck
        // Checks if the target is considered to be of the same group as the caster
        // -----------------------------------------------------------------------------------
        public bool Tools_SameCheck(Player player, bool bSelf, bool bPlayers, bool bParty, bool bGuild, bool bRealm, bool bReverse = false)
        {
            if (!bReverse)
            {
                // -- we want to include all stated target groups
    
                if (bSelf && (player == this)) return true;
                if (bPlayers && player is Player) return true;
                if (bParty && Tools_SameParty(player)) return true;
                if (bGuild && Tools_SameGuild(player)) return true;
                if (bRealm && Tools_SameRealm(player)) return true;
                return false;
            }
            else
            {
                // -- we want to exclude all stated target groups
    
                if (bSelf && (player == this)) return false;
                if (bPlayers && player is Player) return false;
                if (bParty && Tools_SameParty(player)) return false;
                if (bGuild && Tools_SameGuild(player)) return false;
                if (bRealm && Tools_SameRealm(player)) return false;
                return true;
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_SameParty
        // Checks if the target is in the same party as the caster
        // -----------------------------------------------------------------------------------
        public bool Tools_SameParty(Player player)
        {
            return (this.party.InParty() &&
                    player.party.InParty() &&
                    this.party.party.members.Contains(player.name) &&
                    player != this
                    );
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_SameGuild
        // Checks if the target is in the same guild as the caster
        // -----------------------------------------------------------------------------------
        public bool Tools_SameGuild(Player player)
        {
            return (this.guild.InGuild() &&
                    player.guild.InGuild() &&
                    guild.name == player.guild.name &&
                    player != this
                    );
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_SameRealm
        // Checks if the target is the same realmID/alliedRealmID as the caster
        // -----------------------------------------------------------------------------------
        public bool Tools_SameRealm(Player player)
        {
    #if _iMMOPVP
            return GetAlliedRealms(player);
    #else
            return false;
    #endif
        }
    
        // ================================ TELEPORTATION ====================================
        // Custom warp function because I plan to add visual effects to the client side a
        // bit later on. This will require a TargetRPC etc.
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // Cmd_NpcWarp
        // @Client -> @Server
        // -----------------------------------------------------------------------------------
    #if _iMMOTELEPORTER
    	[Command]
    	public void Cmd_NpcWarp(int index)
    	{
    		if (target != null && target is Npc npc && Utils.ClosestDistance(this, target) <= interactionRange && npc.npcTeleporter.teleportationDestinations.Length > 0 && npc.npcTeleporter.teleportationDestinations.Length >= index)
            {
        		npc.npcTeleporter.teleportationDestinations[index].teleportationTarget.OnTeleport(this);
        	}
    	}
    #endif
    
        // -----------------------------------------------------------------------------------
        // Cmd_Tools_Warp
        // @Client -> @Server
        // -----------------------------------------------------------------------------------
        [Command]
        public void Cmd_Tools_Warp(Vector3 pos)
        {
            movement.Warp(pos);
            target = null;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_Warp
        // -----------------------------------------------------------------------------------
        [ServerCallback]
        public void Tools_Warp(Vector3 pos)
        {
            movement.Warp(pos);
            target = null;
        }
    
        // ================================== COMMON UI ======================================
        // Some common UI functions (all Client side) that include a Popup, a universal
        // CastBar and a small player log (InfoBox). Just make sure to call the functions
        // only from the Client. Some functions also have a server variant that allows you
        // to call the function from the server and it is then executed at the connected
        // client.
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // Tools_PopupShow
        // @Client
        // -----------------------------------------------------------------------------------
        public void Tools_PopupShow(string message)
        {
            if (message == "") return;
    
            Player player = Player.localPlayer;
            if (!player) return;
    
            if (Tools_popup == null)
                Tools_popup = FindFirstObjectByType<Tools_UI_Prompt>();
                //Tools_popup = FindObjectOfType<Tools_UI_Prompt>();
    
            if (Tools_popup != null && !Tools_popup.forceUseChat)
                Tools_popup.Show(message);
    
            Tools_AddMessage(message, 0, false);                                      // todo: add editable color
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CastbarShow
        // @Client
        // -----------------------------------------------------------------------------------
        public void Tools_CastbarShow(string message, float duration)
        {
            if (duration <= 0) return;
    
            Player player = Player.localPlayer;
            if (!player) return;
    
            if (Tools_castbar == null)
                Tools_castbar = FindFirstObjectByType<Tools_UI_CastBar>();
                //Tools_castbar = FindObjectOfType<Tools_UI_CastBar>();
    
            if (Tools_castbar != null)
                Tools_castbar.Show(message, duration);
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CastbarHide
        // @Client
        // -----------------------------------------------------------------------------------
        public void Tools_CastbarHide()
        {
            if (Tools_castbar == null)
                Tools_castbar = FindFirstObjectByType<Tools_UI_CastBar>();
                //Tools_castbar = FindObjectOfType<Tools_UI_CastBar>();
    
            if (Tools_castbar != null)
                Tools_castbar.Hide();
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_TargetAddMessage
        // @Server
        // -----------------------------------------------------------------------------------
        [ServerCallback]
        public override void Tools_TargetAddMessage(string message, byte color = 0, bool show = true)
        {
            if (message == "") return;
    
            if (this == Player.localPlayer)
            {
                Tools_AddMessage(message, color, show);
                return;
            }
    
            if (Tools_infobox == null)
                Tools_infobox = GetComponent<Tools_InfoBox>();
    
            if (Tools_infobox)
            {
                Tools_infobox.TargetAddMessage(connectionToClient, message, color, show);
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_AddMessage
        // @Client
        // -----------------------------------------------------------------------------------
        [ClientCallback]
        public void Tools_AddMessage(string message, byte color = 0, bool show = true)
        {
            if (message == "") return;
    
            if (Tools_infobox == null)
                Tools_infobox = GetComponent<Tools_InfoBox>();
    
            if (Tools_infobox)
            {
                Tools_infobox.AddMessage(message, color, show);
            }
        }
    
        // ================================= BUFF RELATED ====================================
        // All buff related functions should only be called server side as only the server
        // can manipulate the syncList. The "get" functions are useable on client as well.
        //
        // Buff = a status effect that has a positive effect on the player (e.g. Blessing)
        // Nerf = a status effect thats has a negative effect on the player (e.g. Poison)
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // Tools_CleanupBuffs
        // Removes one or more buffs from the target. Comes with options.
        // @Server
        // -----------------------------------------------------------------------------------
        public override void Tools_CleanupStatusBuffs(float successChance = 1f, float modifier = 0f, int limit = 0)
        {
            int limited = 0;
            if (limit > 0)
                limited = limit;
    
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (!skills.buffs[i].data.disadvantageous && skills.buffs[i].data != offenderBuff && skills.buffs[i].data != murdererBuff && Tools_CheckChance(successChance, modifier))
                {
                    skills.buffs.RemoveAt(i);
                    i--;
    
                    if (limited > 0)
                    {
                        limit--;
                        if (limit <= 0) return;
                    }
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CleanupStatusNerfs
        // Removes one or more nerfs from the target. Comes with options.
        // @Server
        // -----------------------------------------------------------------------------------
        public override void Tools_CleanupStatusNerfs(float successChance = 1f, float modifier = 0f, int limit = 0)
        {
            int limited = 0;
            if (limit > 0)
                limited = limit;
    
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (skills.buffs[i].data.disadvantageous && skills.buffs[i].data != offenderBuff && skills.buffs[i].data != murdererBuff && Tools_CheckChance(successChance, modifier))
                {
                    skills.buffs.RemoveAt(i);
                    i--;
    
                    if (limited > 0)
                    {
                        limit--;
                        if (limit <= 0) return;
                    }
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_getBuffCount
        // Returns the number of skills.buffs (positive status effect) currently active on the player
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public int Tools_getBuffCount()
        {
            int count = 0;
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (!skills.buffs[i].data.disadvantageous && skills.buffs[i].BuffTimeRemaining() > 0 && skills.buffs[i].data != offenderBuff && skills.buffs[i].data != murdererBuff)
                    count++;
            }
            return count;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_getNerfCount
        // Returns the number of nerfs (negative status effect) currently active on the player
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public int Tools_getNerfCount()
        {
            int count = 0;
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (skills.buffs[i].data.disadvantageous && skills.buffs[i].BuffTimeRemaining() > 0 && skills.buffs[i].data != offenderBuff && skills.buffs[i].data != murdererBuff)
                    count++;
            }
            return count;
        }
    
        // ================================= MISC FUNCS ======================================
        // A bunch of very common utility functions that are missing on the core asset
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // Tools_CanAttack
        // Replaces the built-in CanAttack check. This one can be expanded, the built-in one not.
        // -----------------------------------------------------------------------------------
        public override bool Tools_CanAttack(Entity entity)
        { // il faut ajouter les npc attackable ici ?
            return
                base.Tools_CanAttack(entity) &&
                (
                    ((entity is Pet && entity != petControl.activePet) || (entity is Mount && entity != mountControl.activeMount))
    #if _iMMOMOUNTS
                    || (entity is MountExtended && entity != playerExtentedMounts.unmountedMount)
    #endif
                    || (entity is Player && entity.isAlive) || (entity is Monster && entity.isAlive) || (entity is Npc && entity.isAlive)
                )
    #if _iMMOUSAGEREQUIREMENTS
                && ( Tools_GetWeapon() == null || (Tools_GetWeapon() != null && Tools_GetWeapon().Item_CanUse(this)) )
    #endif
    #if _iMMOPVP
                && GetAttackAllowance(entity)
    #endif
                ;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_checkHasClass
        // Checks if the player prefab is of one of the provided array of classes.
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public bool Tools_checkHasClass(Player[] classes)
        {
            if (classes == null || classes.Length == 0) return true;
            foreach (Player _class in classes)
            {
                if (className == _class.name) return true;
            }
            return false;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_checkHasPrestigeClass
        // Checks if the player has one or more prestige classes
        // [Requires: PRESTIGE CLASSES AddOn]
        // @Client or @Server
        // -----------------------------------------------------------------------------------
    #if _iMMOPRESTIGECLASSES
    
        public bool Tools_checkHasPrestigeClass(Tmpl_PrestigeClass[] prestigeClasses)
        {
            if (prestigeClasses == null || prestigeClasses.Length == 0) return true;
            return (prestigeClasses.Any(x => x == playerAddonsConfigurator.prestigeClass)) ? true : false;
        }
    
    #endif
    
        // -----------------------------------------------------------------------------------
        // Tools_getSkillLevel
        // Simple wrapper to return the current level of a skill on the player
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public int Tools_getSkillLevel(ScriptableSkill skill)
        {
            return skills.skills.FirstOrDefault(s => s.name == skill.name).level;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_checkHasSkill
        // Checks the existence of one skill and its level
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public bool Tools_checkHasSkill(ScriptableSkill skill, int level)
        {
            if (skill == null || level <= 0) return true;
            return skills.skills.Any(s => s.name == skill.name && s.level >= level);
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_checkHasSkills
        // Checks the existence of one or more skills and their skill level
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public bool Tools_checkHasSkills(Tools_SkillRequirement[] skills, bool requiresAll = false)
        {
            if (skills == null || skills.Length == 0) return true;
    
            bool valid = false;
    
            foreach (Tools_SkillRequirement skill in skills)
            {
                if (Tools_checkHasSkill(skill.skill, skill.level))
                {
                    valid = true;
                    if (!requiresAll) return valid;
                }
                else
                {
                    valid = false;
                }
            }
    
            return valid;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_removeItems
        // Removes items from the inventory without checking. Removes either the first item
        // provided as a array or all items in that array of items.
        // @Server
        // -----------------------------------------------------------------------------------
        public void Tools_removeItems(Player player, Tools_ItemRequirement[] items, bool removeAll = false)
        {
            if (items.Length == 0) return;
    
            foreach (Tools_ItemRequirement item in items)
            {
                player.inventory.Remove(new Item(item.item), item.amount);
                if (!removeAll) return;
            }
        }
        // -----------------------------------------------------------------------------------
        // Tools_AddItems
        // Removes items from the inventory without checking. Removes either the first item
        // provided as a array or all items in that array of items.
        // @Server
        // -----------------------------------------------------------------------------------
        public void Tools_AddItems(Player player, Tools_ItemRequirement[] items, bool removeAll = false)
        {
            if (items.Length == 0) return;
    
            foreach (Tools_ItemRequirement item in items)
            {
                player.inventory.Add(new Item(item.item), item.amount);
                if (!removeAll) return;
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_GetWeapon
        // Returns the WeaponItem template that the player currently has equipped.
        // -----------------------------------------------------------------------------------
        public WeaponItem Tools_GetWeapon()
        {
            int idx = equipment.slots.FindIndex(slot => slot.amount > 0 &&
                slot.item.data is WeaponItem &&
                ((WeaponItem)slot.item.data).category.StartsWith("Weapon"));
    
            if (idx != -1)
                return (WeaponItem)equipment.slots[idx].item.data;
            else
                return null;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_inventorySlotCount
        // Returns the amount of inventory slots that have an item on them.
        // -----------------------------------------------------------------------------------
        public int Tools_inventorySlotCount(Player player)
        {
            int amnt = 0;
    
            for (int i = 0; i < player.inventory.slots.Count; ++i)
            {
                if (player.inventory.slots[i].amount > 0)
                    amnt++;
            }
    
            return amnt;
        }
    
        // ==================================== TIMER ========================================
        // A very simple Tools_timer that keeps track of the duration that passed since it was
        // started. Each player can only have one active Tools_timer. The Tools_timer can be set, checked
        // and stopped if required. It's not synched and can be used either on the server or
        // on the client for low security things.
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // Tools_setTimer
        // Sets or resets the Tools_timer
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public void Tools_setTimer(float duration) //? Require server authoritative ?
        {
            if (duration > 0)
            {
                Tools_timer = NetworkTime.time + duration;
                Tools_timerRunning = true;
            }
            else
            {
                Tools_stopTimer();
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_checkTimer
        // Checks if the Tools_timer is finished
        // @Client or @Server
        // -----------------------------------------------------------------------------------
       
        public bool Tools_checkTimer() //! Require server authoritative !
        {
            return (Tools_timerRunning && NetworkTime.time > Tools_timer);
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_stopTimer
        // Simply stops the Tools_timer
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public void Tools_stopTimer() //? Require server authoritative ?
        {
            Tools_timer = 0;
            Tools_timerRunning = false;
        }
    
        // ==================================== TASKS ========================================
        // Very simple task system that uses a counter to keep track of the things a player
        // is currently doing. Typically a player can have just one task but the system
        // supports any number. Tasks control if the castBar is shown and you can check if
        // a user is busy or not.
        //
        // Used mostly by Lootcrate, Crafting, Harvesting and a few others as all their
        // activities are considered to be a task.
        //
        // Tasks are kept track client side (they work on server side as well) without a
        // syncVar because they are considered a very low security risk. Nothing depends
        // on them.
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // Tools_isBusy
        // Simply checks if the task counter is greater than 0.
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public bool Tools_isBusy()
        {
            return Tools_activeTasks > 0;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_addTask
        // Simply increases the task counter
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public void Tools_addTask()
        {
            Tools_activeTasks++;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_removeTask
        // Reduces the task counter by one for this player, hides the Tools_castbar if this was the
        // last task removed.
        // @Client or @Server
        // -----------------------------------------------------------------------------------
        public void Tools_removeTask()
        {
            if (Tools_activeTasks > 0)
                Tools_activeTasks--;
            if (Tools_activeTasks < 1)
            {
                Tools_timer = 0;
                if (Tools_castbar == null)
                    Tools_castbar = FindFirstObjectByType<Tools_UI_CastBar>();
                    //Tools_castbar = FindObjectOfType<Tools_UI_CastBar>();
                if (Tools_castbar != null)
                    Tools_castbar.Hide();
            }
        }
    
        
        // ==================================== EVENTS =======================================
    
    
        // ==================================== ADDON SPECIFIC ===============================
        // We need a few AddOn specific functions right here in the Tools, as several AddOns
        // depend on it. This way we don't have to create multiple, redundant functions in
        // each one of the individual AddOns.
        // ===================================================================================
    
        // -----------------------------------------------------------------------------------
        // UnequipCursedEquipment
        // @Server
        // -----------------------------------------------------------------------------------
    #if _iMMOCURSEDEQUIPMENT && _iMMOTOOLS
    
        public void UnequipCursedEquipment(int maxCurseLevel)
        {
            for (int i = 0; i < equipment.slots.Count; ++i)
            {
                int index = i;
                ItemSlot slot = equipment.slots[index];
    
                if (
                    slot.amount > 0 &&
                    inventory.CanAdd(new Item(slot.item.data), slot.amount) &&
                    ((EquipmentItem)slot.item.data).cursedLevel > 0 &&
                    ((EquipmentItem)slot.item.data).cursedLevel <= maxCurseLevel
    #if _iMMOEQUIPABLEBAG
                    && ((EquipmentItem)slot.item.data).canUnequipBag(this)
    #endif
                    )
                {
                    inventory.Add(slot.item, 1);
                    slot.DecreaseAmount(1);
                    equipment.slots[index] = slot;
                }
            }
        }
    
    #endif
    
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
