using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    // ENTITY
    
    public partial class Entity
    {
        [SyncVar] protected GameObject _lastAggressor;
        [SyncVar, HideInInspector] public bool _cannotCast;
    
        public float cacheTimerInterval = 1.0f;
        protected float _cacheTimer;
        [SyncVar] protected float _Tools_modifierSpeed;
        protected const float minBuffChance = 0.01f;
        protected const float maxBuffChance = 0.99f;
    
    #if !_iMMO2D
        public GameObject headPosition;
    #endif
    
        public void OnSkillBuffChanged(SyncList<Buff>.Operation op, int index, Buff oldData, Buff newData)
        {
            Debug.Log("OnSkillBuffChanged Changed update required");
            //titleUpdate.TriggerEvent();
            // onTitleLearnedChanged.Invoke();
        }
        // ================================== AGGRESSOR RELATED ==============================
    
        // -----------------------------------------------------------------------------------
        // lastAggressor
        // -----------------------------------------------------------------------------------
        public Entity lastAggressor
        {
            get { return _lastAggressor != null ? _lastAggressor.GetComponent<Entity>() : null; }
            set { _lastAggressor = value != null ? value.gameObject : null; }
        }
        public void PartialOnAggro(Entity entity)
        {
            target = entity;
            OnAggro(target);
        }
        // ================================== ANIMATION RELATED ==============================
    
        // -----------------------------------------------------------------------------------
        // StartAnimation
        // @Client
        // -----------------------------------------------------------------------------------
        [ClientCallback]
        public void StartAnimation(string animationName, AudioClip soundEffect = null)
        {
            if (string.IsNullOrWhiteSpace(animationName)) return;
    
            foreach (Animator anim in GetComponentsInChildren<Animator>())
            {
                if (anim.parameters.Any(x => x.name == (animationName)))
                    anim.SetBool(animationName, true);
            }
    
            if (soundEffect != null && audioSource != null)
            {
                audioSource.PlayOneShot(soundEffect);
            }
        }
    
        // -----------------------------------------------------------------------------------
        // StopAnimation
        // @Client
        // -----------------------------------------------------------------------------------
        [ClientCallback]
        public void StopAnimation(string animationName, AudioClip soundEffect = null)
        {
            if (string.IsNullOrWhiteSpace(animationName)) return;
    
            foreach (Animator anim in GetComponentsInChildren<Animator>())
            {
                if (anim.parameters.Any(x => x.name == (animationName)))
                    anim.SetBool(animationName, false);
            }
    
            if (soundEffect != null && audioSource != null)
            {
                audioSource.PlayOneShot(soundEffect);
            }
        }
    
        // ================================= FUNCTIONS =======================================
    
        // -----------------------------------------------------------------------------------
        // isAlive
        // -----------------------------------------------------------------------------------
        public bool isAlive
        {
            get
            {
                return health.current > 0;
            }
        }
    
        // -----------------------------------------------------------------------------------
        // canInteract
        // -----------------------------------------------------------------------------------
        public bool canInteract
        {
            get
            {
                return
                        isAlive &&
                        (
                        state == "IDLE" ||
                           state == "MOVING" ||
                           state == "CASTING" ||
                           state == "TRADING"
                           );
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CanAttack
        // Replaces the built-in CanAttack check. This one can be expanded, the built-in one not.
        // -----------------------------------------------------------------------------------
        public virtual bool Tools_CanAttack(Entity entity)
        {
    #if !_iMMO2D
            Vector3 localPosHead = (headPosition != null) ? headPosition.transform.position : transform.position;
            Vector3 targetPosHead = (entity == null) ? Vector3.zero : (entity.headPosition != null) ? entity.headPosition.transform.position : entity.transform.position;
    
    #if UNITY_EDITOR
            if (Physics.Linecast(localPosHead, targetPosHead, out RaycastHit hitb, LayerMask.GetMask("Default")))
                Debug.DrawLine(localPosHead, hitb.point, Color.red);
            else
                Debug.DrawLine(localPosHead, targetPosHead, Color.green);
    #endif
    #endif
            return
                (!_cannotCast &&
                isAlive &&
                entity != null &&
                entity.isAlive &&
                entity != this &&
                !inSafeZone &&
                !entity.inSafeZone &&
    #if _iMMO2D
                !NavMesh2D.Raycast(transform.position, entity.transform.position, out NavMeshHit2D hit, NavMesh2D.AllAreas) &&
    #else
                !(Physics.Linecast(localPosHead, targetPosHead, out RaycastHit hit, LayerMask.GetMask("Default"))) &&
    #endif
               (entity is Player p 
    #if !_iMMO2D
               && (!p.isGameMaster || (p.isGameMaster && !p.combat.invincible)) 
    #endif
               ||
                entity is Monster ||
                entity is Pet ||
                entity is Npc ||
    #if _iMMOMOUNTS
                entity is MountExtended ||
    #endif
                entity is Mount
                ));
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_ToggleVisibility
        // -----------------------------------------------------------------------------------
        public void Tools_ToggleVisibility(bool visible)
        {
            Renderer[] renderer = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderer)
                rend.enabled = visible;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_OverrideState
        // Force overrides the Entity state, as the variable is protected its not possible
        // otherwise. State overwriting is required in certain places because otherwise the
        // state automatically switches back to IDLE or DEAD (for example) and we would not
        // be able to do anything about it.
        // -----------------------------------------------------------------------------------
        public void Tools_OverrideState(string newState)
        {
            if (newState != "")
                _state = newState;
        }
    
        // -----------------------------------------------------------------------------------
        // DealDamageAt_Tools
        // Custom hook and splits up the DealDamageAt hook into class based ones. I could not
        // get the built-in DealDamageAt hook to work with "Player" for example, it only and
        // ever triggered at the "Entity". Same for Monster and Pet. So it always triggers on
        // the base class, not on the derived classes.
        //
        // TODO PlayerTools (Permettrait d'enregistr� tous les truc du genre)
        // -----------------------------------------------------------------------------------
    
        private void DealDamageAt_Tools(Entity entity, int amount, int damageDealt, DamageType damageType)
        {
            Debug.Log("DealDamageAt_Tools(Entity entity)");
            if (entity == null || amount <= 0 || !entity.isAlive) return;
    
            //entity.lastAggressor = this;
    
            if (entity is Player)
            {
                Utils.InvokeMany(typeof(Player), entity, "OnDamageDealt_");
            }
    
            if (entity is Monster)
            {
                Utils.InvokeMany(typeof(Monster), entity, "OnDamageDealt_", amount); //only monster has amount parameter!
            }
    
            if (entity is Pet)
            {
                Utils.InvokeMany(typeof(Pet), entity, "OnDamageDealt_");
            }
            if (entity is Npc)
            {
                Utils.InvokeMany(typeof(Npc), entity, "OnDamageDealt_");
            }
    
        }
        
        [Server]
        public void onKilledEnemyIdentity(Entity entity)
        {
            entity.lastAggressor = (this as Summonable) ? ((Summonable)this).owner : entity;
        }
    
    
        // -----------------------------------------------------------------------------------
        // OnDeath_Tools
        // -----------------------------------------------------------------------------------
        // TODO : vais devoir relocalis� cette fonction certainement car on supprime la methode avec reflexion!!!!
        //protected virtual void OnDeath_Tools_Entity()
        public virtual void OnDeath_Tools_Entity()
        {
            target = null;
            _cannotCast = false;
            //_lastAggressor = null; // we cannot reset that here, would have to go into "on respawn" instead
            Tools_resetModifySpeed();
        }
    
        // -----------------------------------------------------------------------------------
        // OnAggro
        // Custom onAggro function that has a % chance to trigger (instead of automatically)
        // -----------------------------------------------------------------------------------
        public void Tools_OnAggro(Entity source, float fChance = 1f)
        {
            if (fChance <= 0 || source.skills.skills.Count <= 0 || !isAlive || !Tools_CanAttack(source)) return;
    
            if (fChance > 0 && UnityEngine.Random.value <= fChance)
            {
                target = source;
                OnAggro(target);
            }
        }
    
        // =================================== HELPERS =======================================
    
        // -----------------------------------------------------------------------------------
        // Tools_SpawnEffect
        // Same as SpawnEffect that is found in skill effects of the core asset. It has been
        // put here because its required for almost every skill. Prevents duplicate code.
        // -----------------------------------------------------------------------------------
        public void Tools_SpawnEffect(Entity caster, BuffSkill buff)
        {
            if (buff.effect != null)
            {
                GameObject go = Instantiate(buff.effect.gameObject, transform.position, Quaternion.identity);
                go.GetComponent<BuffSkillEffect>().caster = caster;
                go.GetComponent<BuffSkillEffect>().target = this;
                go.GetComponent<BuffSkillEffect>().buffName = buff.name;
                NetworkServer.Spawn(go);
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_Recoil
        // Pushes the target X unity units away from the caster. In case "distance" is negative
        // it instead pulls the target X unity units closer to the caster. The push itself it
        // not smoothed (due to complicated networking) but uses a Warp instead.
        // -----------------------------------------------------------------------------------
        public void Tools_Recoil(Entity caster, float distance)
        {
            // If the distance is positive we want to push the target away.
            if (distance > 0)
            {
                Vector3 newPosition = transform.position - transform.forward * distance;
                movement.Warp(newPosition);
            }
            // If the distance is negative we want to pull the target, but not if they're too close.
            else if (distance < 0 && Vector3.Distance(transform.position, caster.transform.position) > 5)
            {
                Vector3 newPosition = transform.position + transform.forward * distance;
                movement.Warp(newPosition);
            }
        }
    
        // ================================== AOE RELATED ====================================
    
        // -----------------------------------------------------------------------------------
        // Tools_GetCorrectedTargetsInSphere
        // Retrieves all legal targets within a sphere around the caster. This version is for
        // Entities only (Monsters, Pets etc.), players have a unique one with more options.
        // -----------------------------------------------------------------------------------
        public virtual List<Entity> Tools_GetCorrectedTargetsInSphere(Transform origin, float fRadius, bool deadOnly = false, bool affectSelf = false, bool affectOwnParty = false, bool affectOwnGuild = false, bool affectOwnRealm = false, bool reverseTargets = false, bool affectPlayers = false, bool affectNpcs = false, bool affectMonsters = false, bool affectPets = false)
        {
            List<Entity> correctedTargets = new List<Entity>();
    
            int layerMask = ~(1 << 2); //2= ignore raycast
    #if _iMMO2D
            Collider2D[] colliders = Physics2D.OverlapCircleAll(origin.position, fRadius, layerMask);
    #else
            Collider[] colliders = Physics.OverlapSphere(origin.position, fRadius, layerMask);
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
                    if ((deadOnly && !candidate.isAlive) || (!deadOnly && candidate.isAlive))
                    {
                        if (Tools_SameCheck(candidate, affectSelf, affectPlayers, affectNpcs, affectMonsters, affectOwnParty, affectOwnGuild, affectOwnRealm, reverseTargets))
                        {
                            correctedTargets.Add(candidate);
                        }
                    }
                }
            }
    
            return correctedTargets;
        }
    
        // ================================ GROUP CHECKS =====================================
    
        // -----------------------------------------------------------------------------------
        // Tools_SameCheck
        // Checks if the target is considered to be of the same group as the caster, a group
        // can be a party, guild or realm
        // -----------------------------------------------------------------------------------
        public bool Tools_SameCheck(Entity entity, bool bSelf, bool bPlayers, bool bNpcs, bool bMonsters, bool bPets, bool bParty, bool bGuild, bool bRealm, bool bReverse = false)
        {
            // -- we want to include all stated target groups
            if (!bReverse)
            {
                if (bSelf && (entity == this)) return true;
                if (bPlayers && entity is Player) return true;
                if (bNpcs && entity is Npc) return true;
                if (bMonsters && entity is Monster) return true;
                if (bPets && entity is Pet) return true;
                if (bRealm && Tools_SameRealm(entity)) return true;
                return false;
            }
            // -- we want to exclude all stated target groups
            else
            {
                if (bSelf && (entity == this)) return false;
                if (bPlayers && entity is Player) return false;
                if (bNpcs && entity is Npc) return false;
                if (bMonsters && entity is Monster) return false;
                if (bPets && entity is Pet) return false;
                if (bRealm && Tools_SameRealm(entity)) return false;
                return true;
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_SameRealm
        // Checks if the target is the same realmID/alliedRealmID as the caster
        // -----------------------------------------------------------------------------------
        public bool Tools_SameRealm(Entity entity)
        {
    #if _iMMOPVP
            return GetAlliedRealms(entity);
    #else
            return false;
    #endif
        }
    
        // ================================= BUFF RELATED ====================================
    
        // -----------------------------------------------------------------------------------
        // Tools_ApplyBuff
        // -----------------------------------------------------------------------------------
        public virtual void Tools_ApplyBuff(BuffSkill buff, int level = 1, float successChance = 1f, float modifier = 0f)
        {
            if (buff == null || successChance <= 0) return;
    
            // -- check for buff/nerf blocking
            if ( (buff.disadvantageous && skills.buffs.Any(x => x.blockNerfs)) || (!buff.disadvantageous && skills.buffs.Any(x => x.blockBuffs)) )
                return;
    
            // -- check apply chance and apply
            if (Tools_CheckChance(successChance, modifier))
            {
                skills.AddOrRefreshBuff(new Buff(buff, level));
                Tools_SpawnEffect(this, buff);
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_ApplyBuffs
        // -----------------------------------------------------------------------------------
        public virtual void Tools_ApplyBuffs(BuffSkill[] buffs, int level = 1, float successChance = 1f, float modifier = 0f, int limit = 0)
        {
            //if (skills.buffs.Count == 0 || successChance <= 0) return;
            if (buffs.Count() == 0 || successChance <= 0) return;
    
            foreach (BuffSkill buff in buffs)
            {
                if (buff == null || successChance <= 0) continue;
    
                // -- sanity check on the level
                level = Mathf.Clamp(level, 1, buff.maxLevel);
    
                // -- check for buff/nerf blocking
                if (
                    (buff.disadvantageous && skills.buffs.Any(x => x.blockNerfs)) ||
                    (!buff.disadvantageous && skills.buffs.Any(x => x.blockBuffs))
                    )
                    continue;
    
                // -- check apply chance and apply
                if (Tools_CheckChance(successChance, modifier))
                {
                    skills.AddOrRefreshBuff(new Buff(buff, level));
    
                    Tools_SpawnEffect(this, buff);
    
                    if (limit > 0)
                    {
                        limit--;
                        if (limit <= 0) return;
                    }
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_RemoveBuff
        // -----------------------------------------------------------------------------------
        public virtual void Tools_RemoveBuff(BuffSkill buff)
        {
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (skills.buffs[i].data == buff)
                {
                    if (skills.buffs[i].data.cannotRemove)
                    {
                        skills.buffs.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_RemoveBuff
        // -----------------------------------------------------------------------------------
        public virtual void Tools_RemoveBuff(BuffSkill buff, float successChance = 1f, float modifier = 0f)
        {
            if (buff == null || successChance <= 0) return;
    
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (skills.buffs[i].data == buff && Tools_CheckChance(successChance, modifier))
                {
                    if (!skills.buffs[i].data.cannotRemove)
                    {
                        skills.buffs.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CleanupStatusBuffs
        // -----------------------------------------------------------------------------------
        public virtual void Tools_CleanupStatusBuffs(float successChance = 1f, float modifier = 0f, int limit = 0)
        {
            int limited = 0;
            if (limit > 0)
                limited = limit;
    
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (!skills.buffs[i].data.disadvantageous && Tools_CheckChance(successChance, modifier))
                {
                    if (!skills.buffs[i].data.cannotRemove)
                    {
                        skills.buffs.RemoveAt(i);
                        --i;
                        if (limit > 0)
                        {
                            limited--;
                            if (limited <= 0) return;
                        }
                    }
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CleanupStatusNerfs
        // -----------------------------------------------------------------------------------
        public virtual void Tools_CleanupStatusNerfs(float successChance = 1f, float modifier = 0f, int limit = 0)
        {
            int limited = 0;
            if (limit > 0)
                limited = limit;
    
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (skills.buffs[i].data.disadvantageous && Tools_CheckChance(successChance, modifier))
                {
                    if (!skills.buffs[i].data.cannotRemove)
                    {
                        skills.buffs.RemoveAt(i);
                        --i;
                        if (limit > 0)
                        {
                            limited--;
                            if (limited <= 0) return;
                        }
                    }
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CleanupStatusAny
        // -----------------------------------------------------------------------------------
        public virtual void Tools_CleanupStatusAny(float successChance = 1f, float modifier = 0f, int limit = 0)
        {
            int limited = 0;
            if (limit > 0)
                limited = limit;
    
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (Tools_CheckChance(successChance, modifier))
                {
                    if (!skills.buffs[i].data.cannotRemove)
                    {
                        skills.buffs.RemoveAt(i);
                        --i;
                        if (limit > 0)
                        {
                            limited--;
                            if (limited <= 0) return;
                        }
                    }
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CleanupStatusAll
        // -----------------------------------------------------------------------------------
        public void Tools_CleanupStatusAll()
        {
            for (int i = 0; i < skills.buffs.Count; ++i)
            {
                if (!skills.buffs[i].data.cannotRemove)
                {
                    skills.buffs.RemoveAt(i);
                    --i;
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_checkHasBuff
        // -----------------------------------------------------------------------------------
        public bool Tools_checkHasBuff(BuffSkill buff)
        {
            return skills.buffs.Any(x => x.data == buff);
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_CheckChance
        // -----------------------------------------------------------------------------------
        protected bool Tools_CheckChance(float baseChance, float modifier = 0f)
        {
            if (baseChance >= 1 && modifier == 0) return true;
    
    #if _iMMOATTRIBUTES
            baseChance += modifier;
            baseChance -= combat.resistance;
            baseChance = Mathf.Clamp(baseChance, minBuffChance, maxBuffChance);
    #endif
    
            return UnityEngine.Random.value <= baseChance;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_HarmonizeChance
        // -----------------------------------------------------------------------------------
        public float Tools_HarmonizeChance(float baseChance, float modifier)
        {
            baseChance += modifier;
    
    #if _iMMOATTRIBUTES
            baseChance -= combat.resistance;
            baseChance = Mathf.Clamp(baseChance, minBuffChance, maxBuffChance);
    #endif
    
            return baseChance;
        }
    
        // ================================= ITEM RELATED ====================================
    
        // -----------------------------------------------------------------------------------
        // Tools_checkHasEquipment
        // -----------------------------------------------------------------------------------
        public bool Tools_checkHasEquipment(ScriptableItem item, int amount = 1)
        {
            if (item == null) return true;
    
            foreach (ItemSlot slot in equipment.slots)
                if (slot.amount > 0 && slot.amount >= amount && slot.item.data == item) return true;
    
            return false;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_checkDepletableEquipment
        // -----------------------------------------------------------------------------------
        public bool Tools_checkDepletableEquipment(ScriptableItem item, int amount = 1)
        {
            if (item == null) return true;
    
            foreach (ItemSlot slot in equipment.slots)
                if (slot.amount > 0 && slot.item.data.maxStack > 1 && slot.amount >= amount && slot.item.data == item) return true;
    
            return false;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_removeEquipment
        // Delete a equipped item, just like a inventory item can be deleted by default
        // -----------------------------------------------------------------------------------
        public void Tools_removeEquipment(ScriptableItem item)
        {
            for (int i = 0; i < equipment.slots.Count; ++i)
            {
                ItemSlot slot = equipment.slots[i];
    
                if (slot.amount > 0 && slot.item.data == item)
                {
                    slot.amount--;
                    equipment.slots[i] = slot;
                    return;
                }
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_checkHasEquipment
        // -----------------------------------------------------------------------------------
        public bool Tools_checkHasEquipment(ScriptableItem[] items, bool requiresAll = false)
        {
            if (items == null || items.Length <= 0) return true;
    
            bool valid = false;
    
            foreach (ScriptableItem item in items)
            {
                if (Tools_checkHasEquipment(item))
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
        // Tools_checkHasItem
        // -----------------------------------------------------------------------------------
        public bool Tools_checkHasItem(Player player, ScriptableItem item, int amount = 1)
        {
            if (item == null) return true;
            return player.inventory.Count(new Item(item)) >= amount;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_checkHasItems
        // -----------------------------------------------------------------------------------
        public bool Tools_checkHasItems(Player player, Tools_ItemRequirement[] items, bool requiresAll = false)
        {
            if (items == null || items.Length == 0) return true;
    
            bool valid = false;
    
            foreach (Tools_ItemRequirement item in items)
            {
                if (player.inventory.Count(new Item(item.item)) >= item.amount)
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
        // Tools_getTotalItemCount
        // -----------------------------------------------------------------------------------
        public int Tools_getTotalItemCount(Player player, ScriptableItem itemT)
        {
            int totalCount = 0;
    
            for (int i = 0; i < player.inventory.slots.Count; ++i)
            {
                if (player.inventory.slots[i].amount > 0 && player.inventory.slots[i].item.data == itemT)
                {
                    totalCount += player.inventory.slots[i].amount;
                }
            }
            return totalCount;
        }
    
        // ================================= SPEED RELATED ===================================
    
        // -----------------------------------------------------------------------------------
        // Tools_speed
        // -----------------------------------------------------------------------------------
        public virtual float Tools_speed
        {
            get
            {
                return speed + _Tools_modifierSpeed;
            }
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_OverwriteSpeed
        // -----------------------------------------------------------------------------------
        public void Tools_OverwriteSpeed()
        {
            movement.SetSpeed(Tools_speed);
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_resetSpeed
        // -----------------------------------------------------------------------------------
        public void Tools_resetModifySpeed()
        {
            _Tools_modifierSpeed = 0;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_setSpeed
        // -----------------------------------------------------------------------------------
        public void Tools_setSpeedModifier(float newSpeedModifier)
        {
            _Tools_modifierSpeed = newSpeedModifier;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_ModifySpeedPercentage
        // -----------------------------------------------------------------------------------
        public void Tools_ModifySpeedPercentage(float newSpeed)
        {
            Tools_setSpeedModifier(Tools_speed + (Tools_speed * newSpeed));
        }
    
        // ============================== STATE MACHINE RELATED ==============================
    
        // -----------------------------------------------------------------------------------
        // Tools_Stunned
        // -----------------------------------------------------------------------------------
        public bool Tools_Stunned()
        {
            return NetworkTime.time <= stunTimeEnd;
        }
    
        // -----------------------------------------------------------------------------------
        // Tools_SetStun
        // -----------------------------------------------------------------------------------
        public void Tools_SetStun(float stunModifier)
        {
            stunTimeEnd += stunModifier;
        }
    
        // ================================= COMMON UI =======================================
    
        // -----------------------------------------------------------------------------------
        // Tools_TargetAddMessage
        // @Server
        // -----------------------------------------------------------------------------------
        [ServerCallback]
        public virtual void Tools_TargetAddMessage(string message, byte color = 0, bool show = true) { }
    
        // -----------------------------------------------------------------------------------
    }
    
}
    #endif
