using UnityEngine;
using Mirror;

namespace uMMORPG
{
    [RequireComponent(typeof(Level))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(PlayerParty))]
    [DisallowMultipleComponent]
    public partial class PlayerSkills : Skills
    {
        [Header("Components")]
        public Player player;
        public Level level;
        public Movement movement;
        public PlayerParty party;

        [Header("Skill Experience")]
        [SyncVar] public long skillExperience = 0;
#if _iMMO2D
        // always store lookDirection at the time of casting.
        // this is the only 100% accurate way since player movement is only synced
        // in intervals and look direction from velocity is never 100% accurate.
        // fixes https://github.com/vis2k/uMMORPG2D/issues/19
        // => only necessary for players. all other entities are server controlled.
        Vector2 _currentSkillDirection = Vector2.down;
        protected override Vector2 currentSkillDirection => _currentSkillDirection;
#endif
#if _SERVER
        [HideInInspector] public bool isDirtySkills = false;
        [HideInInspector] public bool isDirtyBuffs = false;
#endif

        void Start()
        {
            // do nothing if not spawned (=for character selection previews)
            if (!isServer && !isClient) return;

            // spawn effects for any buffs that might still be active after loading
            // (OnStartServer is too early)
            // note: no need to do that in Entity.Start because we don't load them
            //       with previously casted skills
            if (isServer)
                for (int i = 0; i < buffs.Count; ++i)
                    if (buffs[i].BuffTimeRemaining() > 0)
                        buffs[i].data.SpawnEffect(entity, entity);
        }

        [Command]
#if _iMMO2D
        public void CmdUse(int skillIndex, Vector2 direction)
#else
        public void CmdUse(int skillIndex)
#endif
        {
#if _SERVER
            // validate
            if ((entity.state == "IDLE" || entity.state == "MOVING" || entity.state == "CASTING") &&
                0 <= skillIndex && skillIndex < skills.Count)
            {
                // skill learned and can be casted?
                if (skills[skillIndex].level > 0 && skills[skillIndex].IsReady())
                {
                    currentSkill = skillIndex;

#if _iMMO2D
                    // set look direction to use when the cast starts.
                    // DO NOT set entity.lookDirection instead. it would be over-
                    // written by Entity.Update before the actual cast starts!
                    // fixes https://github.com/vis2k/uMMORPG2D/issues/19
                    _currentSkillDirection = direction;

                    // let's set it anyway for visuals.
                    // even if it might be overwritten.
                    entity.lookDirection = direction;
#endif
                    //Debug.Log("skill used");
                    isDirtySkills = true;
                    isDirtyBuffs = true;
                }
            }
#endif
        }

        // helper function: try to use a skill and walk into range if necessary
        [Client]
        public void TryUse(int skillIndex, bool ignoreState = false)
        {
            // only if not casting already
            // (might need to ignore that when coming from pending skill where
            //  CASTING is still true)
            if (entity.state != "CASTING" || ignoreState)
            {
                Skill skill = skills[skillIndex];
                // fix skill auto-recasts:
                // Server calls Skills::RpcCastFinished when castTimeRemaining==0.
                // Rpc may arrive before the SyncList or NetworkTime updates,
                // so when trying to auto re-cast, CastTimeRemaining is still a bit >0.
                // => RpcCastFinished is only ever called exactly when castRemaining==0,
                //    so let's simply ignore the ready check here by passing 'ignoreState',
                //    which is 'true' when auto recasting the next skill after one was finished.
                bool checkSelf = CastCheckSelf(skill, !ignoreState);
                bool checkTarget = CastCheckTarget(skill);
                if (checkSelf && checkTarget)
                {
                    // check distance between self and target
#if _iMMO2D
                    Vector2 destination;
                    if (CastCheckDistance(skill, out destination))
                    {
                        // cast
                        CmdUse(skillIndex, ((Player)entity).lookDirection);
                    }
#else
                    Vector3 destination;
                    if (CastCheckDistance(skill, out destination))
                    {
                        // cast
                        CmdUse(skillIndex);
                    }
#endif
                    else
                    {
                        // move to the target first
                        // (use collider point(s) to also work with big entities)
                        float stoppingDistance = skill.castRange * ((Player)entity).attackToMoveRangeRatio;
                        movement.Navigate(destination, stoppingDistance);

                        // use skill when there
                        ((Player)entity).useSkillWhenCloser = skillIndex;
                    }
                }
            }
            else
            {
                ((Player)entity).pendingSkill = skillIndex;
            }
        }

        public bool HasLearned(string skillName)
        {
            // has this skill with at least level 1 (=learned)?
            return HasLearnedWithLevel(skillName, 1);
        }

        public bool HasLearnedWithLevel(string skillName, int skillLevel)
        {
            // (avoid Linq because it is HEAVY(!) on GC and performance)
            foreach (Skill skill in skills)
                if (skill.level >= skillLevel && skill.name == skillName)
                    return true;
            return false;
        }

        // helper function for command and UI
        // -> this is for learning and upgrading!
        public bool CanUpgrade(Skill skill)
        {
            return
#if _iMMOPRESTIGECLASSES
            ((Player)entity).playerAddonsConfigurator.PrestigeClasses_CanUpgradeSkill(skill) &&
#endif
                   skill.level < skill.maxLevel &&
                   level.current >= skill.upgradeRequiredLevel &&
                   skillExperience >= skill.upgradeRequiredSkillExperience &&
                   (skill.predecessor == null || (HasLearnedWithLevel(skill.predecessor.name, skill.predecessorLevel)));
        }

        // -> this is for learning and upgrading!
        [Command]
        public void CmdUpgrade(int skillIndex)
        {
#if _SERVER
            // validate
            if ((entity.state == "IDLE" || entity.state == "MOVING" || entity.state == "CASTING") &&
                0 <= skillIndex && skillIndex < skills.Count)
            {
                // can be upgraded?
                Skill skill = skills[skillIndex];
                if (CanUpgrade(skill))
                {
                    // decrease skill experience
                    skillExperience -= skill.upgradeRequiredSkillExperience;

                    // upgrade
                    ++skill.level;
                    skills[skillIndex] = skill;
                    isDirtySkills = true;
                }
            }
#endif
        }

        // events //////////////////////////////////////////////////////////////////
        [Server]
        public void OnKilledEnemy(Entity victim)
        {
            // killed a monster
            if (victim is Monster monster)
            {
                // gain exp if not in a party or if in a party without exp share
                if (!party.InParty() || !party.party.shareExperience)
                    skillExperience += Experience.BalanceExperienceReward(monster.rewardSkillExperience, level.current, monster.level.current);
            }
        }
    }
}