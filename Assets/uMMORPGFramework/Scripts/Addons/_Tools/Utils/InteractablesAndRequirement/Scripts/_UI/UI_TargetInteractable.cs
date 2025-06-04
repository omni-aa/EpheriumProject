using Mirror;
using UnityEngine;
using UnityEngine.UI;

#if _iMMOTOOLS

namespace uMMORPG
{
    
    public class UI_TargetInteractable : MonoBehaviour
    {
        [Header("Target")]
        public GameObject panel;
        public Slider healthSlider;
        public Text nameText;
        public Text distanceText;
        public Transform buffsPanel;
        public UIBuffSlot buffSlotPrefab;
        public Button tradeButton, guildInviteButton, partyInviteButton;
    
        [Header("Target of target")]
        public GameObject TargetPanel;
        public Slider targetHealthSlider;
        public Text targetText;
        public Button selectTargetButton;
    
        [Header("[-=-[ Interactable Target ]-=-]")]
        public Text healthText;
        public Text TagetHealthText;
    
        public Text levelText;
        public GameObject challengeObject;
        public GameObject eliteObject;
        public GameObject bossObject;
        public bool nameColoring = false;
    
        private void Update()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                // show nextTarget > target
                Tools_Interactable targetInteractable = player.targetInteractable ?? null;
                //Entity target = player.nextTarget ?? player.target;
                if(targetInteractable != null)
                {
                    Tools_InteractableObject io = (Tools_InteractableObject)targetInteractable;
                    float distance = Tools_ClosestDistance.ClosestDistance(player.collider, io.collider );
    
                    nameText.text = targetInteractable.name;
                }
    
                /*if (target != null && target != player)
                {
                    float distance = Utils.ClosestDistance(player, target);
    
                    if (!(target is Player) && target.health.current > 0 && distance < 50) SetupTarget(player, target, distance);
                    else if (target is Player && distance < 50) SetupTarget(player, target, distance);
                    else panel.SetActive(false);
                }*/
                else panel.SetActive(false);
            }
            else panel.SetActive(false);
        }
    
        // Performs all required setup for our target.
        private void SetupTarget(Player player, Entity target, float distance)
        {
            // name and health
            panel.SetActive(true);
            nameText.text = target.name;
            healthSlider.value = target.health.Percent();
            if (target.target == null)
            {
                TargetPanel.SetActive(false);
                /*targetText.enabled = false;
                targetHealthSlider.enabled = false;
                selectTargetButton.enabled = false;
                targetText.text = "";*/
            }
            else
            {
                TargetPanel.SetActive(true);
                /* targetText.enabled = true;
                 targetHealthSlider.enabled = true;
                 selectTargetButton.enabled = true;*/
                //targetText.text = target.target.name.ToString() + " lvl: " + target.target.level.current.ToString();
    
                targetHealthSlider.value = target.target.health.Percent();
                TagetHealthText.text = target.target.health.current.ToString() + " / " + target.target.health.max.ToString();
                targetText.text = target.target.name.ToString();
            }
            healthText.text = target.health.current.ToString() + " / " + target.health.max.ToString();
            levelText.text = target.level.current.ToString();
            distanceText.text = ((int)distance).ToString() + "m";
            //Debug.Log("(Target =" + target.target.ToString());
            // Debug.Log("(level =" +target.level.current.ToString());
            // Debug.Log("(Percent =" + target.health.Percent());
            // Debug.Log("(Vie =" + target.health.current.ToString());
            // Debug.Log("(level =" + target.level.current.ToString());
            /*BuffControl(target);
            ButtonControl(player, target, distance);
            TargetControl(player, target);*/
        }
    
        // Controls all functionality for buffs on target.
        /*private void BuffControl(Entity target)
        {
            // target buffs
            UIUtils.BalancePrefabs(buffSlotPrefab.gameObject, target.skills.buffs.Count, buffsPanel);
            for (int i = 0; i < target.skills.buffs.Count; ++i)
            {
                UIBuffSlot slot = buffsPanel.GetChild(i).GetComponent<UIBuffSlot>();
    
                // refresh
                slot.image.color = Color.white;
                slot.image.sprite = target.skills.buffs[i].image;
                slot.tooltip.text = target.skills.buffs[i].ToolTip();
                slot.slider.maxValue = target.skills.buffs[i].buffTime;
                slot.slider.value = target.skills.buffs[i].BuffTimeRemaining();
            }
        }*/
    
        // Controls all functionality for buttons on target.
        /*private void ButtonControl(Player player, Entity target, float distance)
        {
            // trade button
            if (target is Player
    #if _iMMOPVP
                && ((Player)target).Tools_SameRealm(player)
    #endif
            )
            {
                tradeButton.gameObject.SetActive(true);
                tradeButton.interactable = player.trading.CanStartTradeWith(target);
                tradeButton.onClick.SetListener(() =>
                {
                    player.trading.CmdSendRequest();
                });
            }
            else tradeButton.gameObject.SetActive(false);
    
            // guild invite button
            if (target is Player && player.guild.InGuild()
    #if _iMMOPVP
                && ((Player)target).Tools_SameRealm(player)
    #endif
            )
            {
                guildInviteButton.gameObject.SetActive(true);
                guildInviteButton.interactable = !((Player)target).guild.InGuild() &&
    #if _iMMOGUILDUPGRADES
                                                 player.playerGuildUpgrades.GuildCapacity_CanInvite() &&
    #endif
                                                 player.guild.guild.CanInvite(player.name, target.name) &&
                                                 NetworkTime.time >= player.nextRiskyActionTime &&
                                                 distance <= player.interactionRange;
                guildInviteButton.onClick.SetListener(() =>
                {
                    player.guild.CmdInviteTarget();
                });
            }
            else guildInviteButton.gameObject.SetActive(false);
    
            // party invite button
            if (target is Player
    #if _iMMOPVP
                && ((Player)target).Tools_SameRealm(player)
    #endif
            )
            {
                partyInviteButton.gameObject.SetActive(true);
                partyInviteButton.interactable = (!player.party.InParty() || !player.party.party.IsFull()) &&
                                                 !((Player)target).party.InParty() &&
                                                 NetworkTime.time >= player.nextRiskyActionTime &&
                                                 distance <= player.interactionRange;
                partyInviteButton.onClick.SetListener(() =>
                {
                    player.party.CmdInvite(target.name);
                });
            }
            else partyInviteButton.gameObject.SetActive(false);
    
            selectTargetButton.onClick.SetListener(() =>
            {
                player.CmdSetTarget(target.target.netIdentity);
            });
        }*/
    
        // Controls all functionality for improved target.
        /*private void TargetControl(Player player, Entity target)
        {
            // Setup Elite
            if (target.isElite) eliteObject.SetActive(true);
            else eliteObject.SetActive(false);
    
            // Setup Boss
            if (target.isBoss) bossObject.SetActive(true);
            else bossObject.SetActive(false);
    
            // Setup Level Info
            levelText.gameObject.SetActive(true);
            challengeObject.SetActive(false);
    
            //si le niveau de la cible  et inf�rieur ou �gal  
            int diff = (target.level.current - player.level.current);
    
            if (diff <= -2) { levelText.color = Color.grey; nameText.color = Color.grey; }
            else if (diff == -1) { levelText.color = Color.green; nameText.color = Color.green; }
            else if (diff == 0) { levelText.color = Color.white; nameText.color = Color.white; }
            else if (diff == 1) { levelText.color = Color.blue; nameText.color = Color.blue; }
            else if (diff == 2) { levelText.color = Color.yellow; nameText.color = Color.yellow; }
            else if (diff == 3) { levelText.color = new Color(1.0f, 0.64f, 0.0f); nameText.color = new Color(1.0f, 0.64f, 0.0f); } //orange
            else if (diff >= 4) { levelText.color = Color.red; nameText.color = Color.red; }
            else { Debug.Log("diff color error :" + diff); }
            if (target.isBoss) { levelText.color = new Color(143, 0, 254, 1); nameText.color = new Color(143, 0, 254, 1); } // violet
        }*/
    }
    
}
    #endif
