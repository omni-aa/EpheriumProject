#if _MYSQL
using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using MySqlConnector;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Data;

namespace uMMORPG
{
    // Database (mySQL)
    public partial class Database : MonoBehaviour
    {
        public static Database singleton;
        private string connectionString = null;
#if _SERVER || UNITY_EDITOR
        [Tooltip("The first time you launch your server this option must be enabled in order to create the tables on your server, this can remain enabled and this will keep the original behavior which tries to create each table when the server starts, if you uncheck it the attempt to create the tables will not be made the next time the server starts.")]
        public bool createTableOnConnect = true;
        [Tooltip("Be careful if you disable this option, accounts that do not exist will not be able to connect, because they will not be created automatically!")]
        public bool allowAutoRegister = true;
        [Tooltip("Enable create query log for debug (disable this for build!)")]
        public bool enableQueryLog = false;

        private static string _connectionString;
#endif

        [Header("Events")]
        // use onConnected to create an extra table for your addon
        public UnityEvent onConnected;
        public UnityEventPlayerBool onCharacterLoad;
        public UnityEventPlayerBool onCharacterSave;

#if _SERVER
        // M�thode d'initialisation de la cha�ne de connexion
        public void InitializeConnectionString()
        {
            if (connectionString == null)
            {
                MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
                {
                    Server = string.IsNullOrWhiteSpace(databaseConfiguration.databaseHost) ? "127.0.0.1" : databaseConfiguration.databaseHost,
                    Database = string.IsNullOrWhiteSpace(databaseConfiguration.databaseName) ? "database" : databaseConfiguration.databaseName,
                    UserID = string.IsNullOrWhiteSpace(databaseConfiguration.databaseUser) ? "user" : databaseConfiguration.databaseUser,
                    Password = string.IsNullOrWhiteSpace(databaseConfiguration.databasePassword) ? "password" : databaseConfiguration.databasePassword,
                    Port = databaseConfiguration.databasePort,
                    CharacterSet = string.IsNullOrWhiteSpace(databaseConfiguration.charecteSet) ? "utf8mb4" : databaseConfiguration.charecteSet,
                    Pooling = true  // Active explicitement le pooling
                };

                connectionString = connectionStringBuilder.ConnectionString;
                _connectionString = connectionString;
            }
        }

        public List<(string sql, MySqlParameter[] parameters)> batch = new List<(string, MySqlParameter[])>();

        private static MySqlConnection _connection;
        private static readonly object _lock = new object();

        public string GetConnexionString()
        {
            return _connectionString;
        }
        public static void Initialize(string connectionString)
        {
            if (_connection == null)
            {
                lock (_lock)
                {
                    if (_connection == null) // Double v�rification pour �viter les probl�mes en multi-thread
                    {
                        _connection = new MySqlConnection(connectionString);
                        _connection.Open();
                    }
                }
            }
        }

        public static MySqlConnection GetConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                lock (_lock)
                {
                    if (_connection == null || _connection.State != ConnectionState.Open)
                    {
                        _connection = new MySqlConnection(_connectionString);
                        _connection.Open();
                    }
                }
            }
            return _connection;
        }

        // -----------------------------------------------------------------------------------
        // Awake
        // -----------------------------------------------------------------------------------
        void Awake()
        {
            if (singleton == null) singleton = this;
            InitializeConnectionString();
            Initialize(connectionString);

            Utils.InvokeMany(typeof(Database), this, "Start_Tools_");
        }

        // -----------------------------------------------------------------------------------
        // Connect
        // -----------------------------------------------------------------------------------
        public void Connect()
        {
            if (createTableOnConnect)
            {
                MySqlHelper.ExecuteNonQuery(
                    // accounts
                    @"
                CREATE TABLE IF NOT EXISTS `accounts` (
                      `id` int(11) NOT NULL AUTO_INCREMENT,
                      `name` varchar(32) NOT NULL,
                      `password` varchar(254) NOT NULL,
                      `created` datetime NOT NULL,
                      `lastlogin` datetime NOT NULL,
                      `online` tinyint(1) NOT NULL DEFAULT 0,
                      `banned` tinyint(1) NOT NULL DEFAULT 0,
                      PRIMARY KEY (`id`),
                      KEY `name` (`name`) USING BTREE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                " +

                    // characters
                    @"
                CREATE TABLE IF NOT EXISTS `characters` (
                  `id` int(11) NOT NULL AUTO_INCREMENT,
                  `name` varchar(32) NOT NULL,
                  `account_id` int(11) NOT NULL,
                  `class` varchar(32) NOT NULL,
                  `x` float NOT NULL,
                  `y` float NOT NULL, " +
#if !_iMMO2D
                                      @"
                                  `z` float NOT NULL," +
#endif
                                      @"
                  `level` int(11) NOT NULL DEFAULT 1,
                  `health` int(11) NOT NULL,
                  `mana` int(11) NOT NULL,
                  `stamina` int(11) NOT NULL DEFAULT 0,
                  `strength` int(11) NOT NULL DEFAULT 0,
                  `intelligence` int(11) NOT NULL DEFAULT 0,
                  `experience` bigint(20) NOT NULL DEFAULT 0,
                  `skillExperience` bigint(20) NOT NULL DEFAULT 0,
                  `gold` bigint(20) NOT NULL DEFAULT 0,
                  `coins` bigint(20) NOT NULL DEFAULT 0,
                  `gamemaster` tinyint(1) NOT NULL DEFAULT 0,
                  `online` tinyint(1) NOT NULL,
                  `lastsaved` datetime NOT NULL,
                  `deleted` tinyint(1) NOT NULL,
                  PRIMARY KEY (`id`),
                  UNIQUE KEY `name` (`name`) USING BTREE,
                  KEY `account` (`account_id`),
                  CONSTRAINT FOREIGN KEY (`account_id`) REFERENCES `accounts` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                                " +

                    // character_buffs
                    @"
                CREATE TABLE IF NOT EXISTS `character_buffs` (
                  `character_id` int(11) NOT NULL,
                  `name` varchar(64) NOT NULL,
                  `level` int(11) NOT NULL,
                  `buffTimeEnd` float NOT NULL,
                  PRIMARY KEY (`character_id`,`name`) USING BTREE,
                  KEY `character_id` (`character_id`),
                  CONSTRAINT FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                " +

                    // character_equipment
                    @"
                CREATE TABLE IF NOT EXISTS `character_equipment` (
                  `character_id` int(11) NOT NULL,
                  `slot` int(3) NOT NULL,
                  `name` varchar(64) NOT NULL,
                  `amount` int(11) NOT NULL," +
#if !_iMMO2D
                      @"
                  `durability` int(11) NOT NULL," +
#endif
                      @"
                  `summonedHealth` int(11) NOT NULL,
                  `summonedLevel` int(11) NOT NULL,
                  `summonedExperience` bigint(20) NOT NULL,
                  `equipmentLevel` int(11) NOT NULL,
                  `equipmentGems` varchar(256) DEFAULT NULL,
                  PRIMARY KEY (`character_id`,`slot`) USING BTREE,
                  KEY `character_id` (`character_id`),
                  CONSTRAINT FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                " +

                    // character_inventory
                    @"
                CREATE TABLE IF NOT EXISTS `character_inventory` (
                  `character_id` int(11) NOT NULL,
                  `slot` int(4) NOT NULL,
                  `name` varchar(64) NOT NULL,
                  `amount` int(11) NOT NULL," +
#if !_iMMO2D
                      @"
                  `durability` int(11) NOT NULL," +
#endif
                      @"
                  `summonedHealth` int(11) NOT NULL,
                  `summonedLevel` int(11) NOT NULL,
                  `summonedExperience` bigint(20) NOT NULL,
                  `equipmentLevel` int(11) NOT NULL DEFAULT 0,
                  `equipmentGems` varchar(256) DEFAULT NULL,
                  PRIMARY KEY (`character_id`,`slot`) USING BTREE,
                  KEY `character_id` (`character_id`),
                  CONSTRAINT FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                " +

                    // character_itemcooldowns
                    @"
                CREATE TABLE IF NOT EXISTS `character_itemcooldowns` (
                  `character_id` int(11) NOT NULL,
                  `category` varchar(50) NOT NULL,
                  `cooldownEnd` float NOT NULL,
                  UNIQUE KEY `character` (`character_id`,`category`) USING BTREE,
                  KEY `character_id` (`character_id`),
                  CONSTRAINT FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                " +

                    // character_orders
                    @"
                CREATE TABLE IF NOT EXISTS `character_orders` (
                  `orderid` bigint(20) NOT NULL AUTO_INCREMENT,
                  `character_id` int(11) NOT NULL,
                  `coins` bigint(20) NOT NULL,
                  `processed` bigint(20) NOT NULL,
                  PRIMARY KEY (`orderid`),
                  KEY `character` (`character_id`) USING BTREE,
                  CONSTRAINT FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                " +

                    // character_quests
                    @"
                CREATE TABLE IF NOT EXISTS `character_quests` (
                  `character_id` int(11) NOT NULL,
                  `name` varchar(64) NOT NULL,
                  `progress` int(11) NOT NULL,
                  `completed` tinyint(1) NOT NULL,
                  PRIMARY KEY (`character_id`,`name`) USING BTREE,
                  KEY `character_id` (`character_id`),
                  CONSTRAINT FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                " +

                    // character_skills
                    @"
                CREATE TABLE IF NOT EXISTS `character_skills` (
                  `character_id` int(11) NOT NULL,
                  `name` varchar(50) NOT NULL,
                  `level` int(11) NOT NULL,
                  `castTimeEnd` float NOT NULL,
                  `cooldownEnd` float NOT NULL,
                  PRIMARY KEY (`character_id`,`name`) USING BTREE,
                  KEY `character_id` (`character_id`),
                  CONSTRAINT FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                " +

                    // guild_info
                    @"
                CREATE TABLE IF NOT EXISTS `guild_info` (
                  `id` int(11) NOT NULL AUTO_INCREMENT,
                  `name` varchar(32) NOT NULL,
                  `notice` text NOT NULL,
                  PRIMARY KEY (`id`),
                  UNIQUE KEY `name` (`name`) USING BTREE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
                " +

                    // character_guild
                    @" 
                CREATE TABLE IF NOT EXISTS `character_guild` (
                  `character_id` int(11) NOT NULL,
                  `guild_id` int(11) NOT NULL,
                  `rank` tinyint(4) NOT NULL,
                  PRIMARY KEY (`character_id`) USING BTREE,
                  KEY `guild_id` (`guild_id`),
                  KEY `character_id` (`character_id`),
                  CONSTRAINT FOREIGN KEY (`character_id`) REFERENCES `characters` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
                  CONSTRAINT FOREIGN KEY (`guild_id`) REFERENCES `guild_info` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;");

                // addon system hooks
                onConnected.Invoke();
            }

        }

        // -----------------------------------------------------------------------------------
        // TryLogin
        // -----------------------------------------------------------------------------------
        public bool TryLogin(string account, string password)
        {
            if (!string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(password))
            {
                // demo feature: create account if it doesn't exist yet.
                MySqlParameter accountParameter = new("@name", MySqlDbType.VarChar) { Value = account };
                MySqlParameter passwordParameter = new("@password", MySqlDbType.VarChar) { Value = password };
                MySqlParameter createdParameter = new("@created", MySqlDbType.DateTime) { Value = DateTime.UtcNow };
                MySqlParameter lastloginParameter = new("@lastlogin", MySqlDbType.DateTime) { Value = DateTime.UtcNow };

                // check account name, password, banned status
                //bool valid = ((long)MySqlHelper.ExecuteScalar(connectionString, "SELECT Count(*) FROM accounts WHERE name=@name AND password=@password AND banned=0", accountParameter, passwordParameter)) == 1;
                bool valid = ((long)MySqlHelper.ExecuteScalar("SELECT Count(*) FROM accounts WHERE name=@name AND password=@password AND banned=0", accountParameter, passwordParameter)) == 1;
                if (valid)
                {
                    // save last login time and return true
                    MySqlHelper.ExecuteNonQuery("UPDATE accounts SET lastlogin=@lastlogin WHERE name=@name", accountParameter, lastloginParameter);
                    return true;
                }
                else
                {
                    // Execute query
                    if (allowAutoRegister)
                        MySqlHelper.ExecuteNonQuery("INSERT INTO accounts SET name=@name, password=@password, created=@created, lastlogin=@lastlogin, online=0, banned=0", accountParameter, passwordParameter, createdParameter, lastloginParameter);
                    //MySqlHelper.ExecuteNonQuery("INSERT IGNORE INTO accounts VALUES (@name, @password, @created, @lastlogin, 0, 0)", accountParameter, passwordParameter, createdParameter, lastloginParameter);
                    bool validAutoReg = ((long)MySqlHelper.ExecuteScalar("SELECT Count(*) FROM accounts WHERE name=@name AND password=@password AND banned=0", accountParameter, passwordParameter)) == 1;
                    if (validAutoReg)
                    {
                        // save last login time and return true
                        MySqlHelper.ExecuteNonQuery("UPDATE accounts SET lastlogin=@lastlogin WHERE name=@name", accountParameter, lastloginParameter);
                        return true;
                    }
                }
            }
            return false;
        }
#endif

        public void LogoutAccountUpdate(string account, bool value)
        {
#if _SERVER
            MySqlParameter accountParameter = new("@name", MySqlDbType.VarChar) { Value = account };
            MySqlParameter accountOnline = new("@online", MySqlDbType.Bool) { Value = value };
            // save last login time and return true
            MySqlHelper.ExecuteNonQuery("UPDATE accounts SET online = @online WHERE name=@name", accountParameter, accountOnline);
#endif
        }

        public bool IsLogged(string account)
        {
#if _SERVER
            if (!string.IsNullOrWhiteSpace(account))
            {
                MySqlParameter accountParameter = new("@name", MySqlDbType.VarChar) { Value = account };

                // Return true if Count(*) is 1, else false
                bool t = (long)MySqlHelper.ExecuteScalar("SELECT Count(*) FROM accounts WHERE name=@name AND online=1", accountParameter) == 1;
                return t;
            }
#endif
            return false;
        }


        public bool CreateAccount(string account, string password)
        {
#if _SERVER
            if (!string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(password))
            {
                MySqlParameter accountParameter = new("@name", MySqlDbType.VarChar) { Value = account };
                MySqlParameter passwordParameter = new("@password", MySqlDbType.VarChar) { Value = password };

                bool valid = ((long)MySqlHelper.ExecuteScalar("SELECT Count(*) FROM accounts WHERE name=@name AND password=@password AND banned=0", accountParameter, passwordParameter)) == 1;
                if (!valid)
                {
                    MySqlParameter createdParameter = new("@created", MySqlDbType.DateTime) { Value = DateTime.UtcNow };
                    MySqlParameter lastloginParameter = new("@lastlogin", MySqlDbType.DateTime) { Value = DateTime.UtcNow };

                    // Execute query
                    MySqlHelper.ExecuteNonQuery("INSERT IGNORE INTO accounts VALUES (@name, @password, @created, @lastlogin, 0)", accountParameter, passwordParameter, createdParameter, lastloginParameter);
                    return true;
                }
                else
                {
                    return false;
                }
            }
#endif
            return false;
        }

#if _SERVER
        // -----------------------------------------------------------------------------------
        // CharacterExists
        // -----------------------------------------------------------------------------------
        public bool CharacterExists(string characterName)
        {
            MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = characterName };

            return ((long)MySqlHelper.ExecuteScalar("SELECT Count(*) FROM characters WHERE name=@character", characterNameParameter)) == 1;
        }

        // -----------------------------------------------------------------------------------
        // CharacterDelete
        // -----------------------------------------------------------------------------------
        public void CharacterDelete(string characterName)
        {
            MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = characterName };
            MySqlHelper.ExecuteNonQuery("UPDATE characters SET deleted=1 WHERE name=@character", characterNameParameter);
        }
#endif

        // -----------------------------------------------------------------------------------
        // CharactersForAccount
        // -----------------------------------------------------------------------------------
        public List<string> CharactersForAccount(string account)
        {
            List<String> result = new List<String>();
#if _SERVER
            MySqlParameter accountParameter = new("@account", MySqlDbType.VarChar) { Value = account };
            var table = MySqlHelper.ExecuteReader("SELECT characters.name FROM accounts LEFT JOIN characters ON accounts.id = characters.account_id WHERE accounts.name=@account AND deleted=0; ", accountParameter);
            foreach (var row in table)
                result.Add((string)row[0]);
#endif
            return result;
        }

#if _SERVER
        // -----------------------------------------------------------------------------------
        // LoadInventory Mysql Version
        // -----------------------------------------------------------------------------------
        private void LoadInventory(PlayerInventory inventory)
        {
            for (int i = 0; i < inventory.size; ++i)
                inventory.slots.Add(new ItemSlot());

            MySqlParameter characterIdParameter = new("@character_id", MySqlDbType.Int32) { Value = inventory.player.id };
#if !_iMMO2D
            using (var reader = MySqlHelper.GetReader(connectionString, @"SELECT name, slot, amount,durability, summonedHealth, summonedLevel, summonedExperience, equipmentLevel, equipmentGems FROM character_inventory WHERE `character_id`=@character_id;", characterIdParameter))
#else
        using (var reader = MySqlHelper.GetReader(connectionString, @"SELECT name, slot, amount, summonedHealth, summonedLevel, summonedExperience, equipmentLevel, equipmentGems FROM character_inventory WHERE `character_id`=@character_id;", characterIdParameter))
#endif
            {
                while (reader.Read())
                {
                    string itemName = (string)reader["name"];
                    int slot = (int)reader["slot"];

                    ScriptableItem itemData;
                    if (slot < inventory.size && ScriptableItem.All.TryGetValue(itemName.GetStableHashCode(), out itemData))
                    {
                        Item item = new Item(itemData);
                        int amount = (int)reader["amount"];
#if !_iMMO2D
                        item.durability = (int)reader["durability"];
#endif
                        item.summonedHealth = (int)reader["summonedHealth"];
                        item.summonedLevel = (int)reader["summonedLevel"];
                        item.summonedExperience = (long)reader["summonedExperience"];
                        item.equipmentLevel = (int)reader["equipmentLevel"];
                        item.equipmentGems = (string)reader["equipmentGems"];
                        inventory.slots[slot] = new ItemSlot(item, amount); ;
                    }
                }
            }
        }

        // -----------------------------------------------------------------------------------
        // LoadEquipment
        // -----------------------------------------------------------------------------------
        private void LoadEquipment(PlayerEquipment equipment)
        {
            for (int i = 0; i < equipment.slotInfo.Length; ++i)
                equipment.slots.Add(new ItemSlot());


            MySqlParameter characterIdParameter = new("@character_id", MySqlDbType.Int32) { Value = equipment.player.id };

            using (var reader = MySqlHelper.GetReader(connectionString, @"SELECT * FROM character_equipment WHERE `character_id`=@character_id;", characterIdParameter))
            {
                while (reader.Read())
                {
                    string itemName = (string)reader["name"];
                    int slot = (int)reader["slot"];

                    ScriptableItem itemData;
                    if (slot < equipment.slotInfo.Length && ScriptableItem.All.TryGetValue(itemName.GetStableHashCode(), out itemData))
                    {
                        Item item = new Item(itemData);
                        int amount = (int)reader["amount"];
#if !_iMMO2D
                        item.durability = (int)reader["durability"];
#endif
                        item.summonedHealth = (int)reader["summonedHealth"];
                        item.summonedLevel = (int)reader["summonedLevel"];
                        item.summonedExperience = (long)reader["summonedExperience"];
                        item.equipmentLevel = (int)reader["equipmentLevel"];
                        item.equipmentGems = (string)reader["equipmentGems"];
                        equipment.slots[slot] = new ItemSlot(item, amount);
                    }
                }
            }
        }

        void LoadItemCooldowns(Player player)
        {
            MySqlParameter characterIdParameter = new("@character_id", MySqlDbType.Int32) { Value = player.id };

            using (var reader = MySqlHelper.GetReader(connectionString, "SELECT * FROM character_itemcooldowns WHERE `character_id`=@character_id", characterIdParameter))
            {
                while (reader.Read())
                {
                    string categoryName = (string)reader["category"];
                    float coldownEnd = (float)reader["cooldownEnd"];
                    player.itemCooldowns.Add(categoryName, coldownEnd + NetworkTime.time);
                }
            }
        }

        // -----------------------------------------------------------------------------------
        // LoadSkills
        // -----------------------------------------------------------------------------------
        private void LoadSkills(PlayerSkills skills)
        {
            foreach (ScriptableSkill skillData in skills.skillTemplates)
                skills.skills.Add(new Skill(skillData));

            MySqlParameter characterIdParameter = new("@character_id", MySqlDbType.Int32) { Value = skills.player.id };

            using (var reader = MySqlHelper.GetReader(connectionString, "SELECT name, level, castTimeEnd, cooldownEnd FROM character_skills WHERE `character_id`=@character_id", characterIdParameter))
            {
                while (reader.Read())
                {
                    string skillName = (string)reader["name"];

                    int index = skills.GetSkillIndexByName(skillName);
                    if (index != -1)
                    {
                        Skill skill = skills.skills[index];
                        skill.level = Mathf.Clamp((int)reader["level"], 1, skill.maxLevel);
                        skill.castTimeEnd = (float)reader["castTimeEnd"] + Time.time;
                        skill.cooldownEnd = (float)reader["cooldownEnd"] + Time.time;
                        skills.skills[index] = skill;
                    }
#if _iMMOTRAITS
                else
                {
                    if (ScriptableSkill.All.TryGetValue(skillName.GetStableHashCode(), out ScriptableSkill skillData))
                    {
                        int level = Mathf.Clamp((int)reader["level"], 0, skillData.maxLevel);

                        Skill skill = new Skill(skillData);
                        skill.level = level;
                        skill.castTimeEnd = (float)reader["castTimeEnd"] + Time.time;
                        skill.cooldownEnd = (float)reader["cooldownEnd"] + Time.time;
                        skills.skills.Add(skill);
                    }
                }
#endif
                }
            }
        }

        // -----------------------------------------------------------------------------------
        // LoadBuffs
        // -----------------------------------------------------------------------------------
        private void LoadBuffs(PlayerSkills skills)
        {
            MySqlParameter characterIdParameter = new("@character_id", MySqlDbType.Int32) { Value = skills.player.id };

            using (var reader = MySqlHelper.GetReader(connectionString, "SELECT name, level, buffTimeEnd FROM character_buffs WHERE `character_id` = @character_id", characterIdParameter))
            {
                while (reader.Read())
                {
                    string buffName = (string)reader["name"];
                    if (ScriptableSkill.All.TryGetValue(buffName.GetStableHashCode(), out ScriptableSkill skillData))
                    {
                        int level = Mathf.Clamp((int)reader["level"], 1, skillData.maxLevel);
                        Buff buff = new Buff((BuffSkill)skillData, level);
                        buff.buffTimeEnd = (float)reader["buffTimeEnd"] + Time.time;
                        skills.buffs.Add(buff);
                    }
                }
            }
        }

        // -----------------------------------------------------------------------------------
        // LoadQuests
        // -----------------------------------------------------------------------------------
        private void LoadQuests(PlayerQuests quests)
        {
            MySqlParameter characterIdParameter = new("@character_id", MySqlDbType.Int32) { Value = quests.player.id };

            using var reader = MySqlHelper.GetReader(connectionString, "SELECT name, progress, completed FROM character_quests WHERE `character_id`=@character_id", characterIdParameter);
            while (reader.Read())
            {
                string questName = (string)reader["name"];
                ScriptableQuest questData;
                if (ScriptableQuest.All.TryGetValue(questName.GetStableHashCode(), out questData))
                {
                    Quest quest = new Quest(questData);
                    quest.progress = (int)reader["progress"];
                    quest.completed = (bool)reader["completed"];
                    quests.quests.Add(quest);
                }
                else Debug.LogWarning("MYSQL LoadQuests: skipped quest " + questName + " for " + quests.name + " because it doesn't exist anymore. If it wasn't removed intentionally then make sure it's in the Resources folder.");
            }
        }

        // -----------------------------------------------------------------------------------
        // LoadGuild
        // -----------------------------------------------------------------------------------
        Guild LoadGuild(int guildId)
        {
            Guild guild = new Guild();
            //Debug.LogError(guildId);
            guild.name = "";

            MySqlParameter guildIdParameter = new("@guild_id", MySqlDbType.Int32) { Value = guildId };

            List<List<object>> table = MySqlHelper.ExecuteReader("SELECT name, notice FROM guild_info WHERE id=@guild_id", guildIdParameter);
            if (table.Count == 1)
            {
                List<object> row = table[0];
                guild.name = (string)row[0];
                guild.notice = (string)row[1];
            }

            List<GuildMember> members = new List<GuildMember>();
            Debug.Log("ok aussi");
            using var reader = MySqlHelper.GetReader(connectionString, "SELECT c.id, c.name, cg.rank, c.level, c.online FROM character_guild cg LEFT JOIN characters c ON cg.character_id = c.id WHERE cg.guild_id = @guild_id ", guildIdParameter);
            while (reader.Read())
            {
                GuildMember member = new();
                member.id = (int)reader["id"];
                member.name = reader["name"].ToString();
                member.rank = (GuildRank)Convert.ToInt32(reader["rank"]);
                member.level = reader["level"] != null ? (int)reader["level"] : 1;
                member.online = Convert.ToInt32(reader["online"]) == 1;
                members.Add(member);
            }

            guild.members = members.ToArray();
            return guild;
        }

        // -----------------------------------------------------------------------------------
        // LoadGuildOnDemand
        // -----------------------------------------------------------------------------------
        void LoadGuildOnDemand(PlayerGuild playerGuild)
        {
            MySqlParameter characterIdParameter = new("@character_id", MySqlDbType.Int32) { Value = playerGuild.player.id };
            var loadGuild = MySqlHelper.ExecuteDataRow("SELECT cg.guild_id, gi.name FROM character_guild cg LEFT JOIN guild_info gi ON cg.guild_id = gi.id  WHERE `character_id`=@character_id", characterIdParameter);
            if (loadGuild != null)
            {
                // load guild on demand when the first player of that guild logs in
                // (= if it's not in GuildSystem.guilds yet)
                if (!GuildSystem.guilds.ContainsKey((string)loadGuild["name"]))
                {
                    Guild guild = LoadGuild((int)loadGuild["guild_id"]);
                    GuildSystem.guilds[guild.name] = guild;
                    playerGuild.guild = guild;
                }
                // assign from already loaded guild
                else playerGuild.guild = GuildSystem.guilds[(string)loadGuild["name"]];
            }
        }

#endif

        // -----------------------------------------------------------------------------------
        // CharacterLoad
        // -----------------------------------------------------------------------------------
        public GameObject CharacterLoad(string characterName, List<Player> prefabs, bool isPreview)
        //public GameObject CharacterLoad(int characterId, List<Player> prefabs, bool isPreview)
        {
#if _SERVER
            //var row = MySqlHelper.ExecuteDataRow("SELECT * FROM characters WHERE name=@name AND deleted=0", new MySqlParameter("@name", characterName));

            var row = MySqlHelper.ExecuteDataRow("SELECT characters.*, accounts.name as aname FROM characters LEFT JOIN accounts ON characters.account_id = accounts.id WHERE characters.name=@name AND characters.deleted=0", new MySqlParameter("@name", characterName));
            if (row != null)
            {
                string className = (string)row["class"];
                var prefab = prefabs.Find(p => p.name == className);
                if (prefab != null)
                {
                    GameObject go = GameObject.Instantiate(prefab.gameObject);
                    Player player = go.GetComponent<Player>();
                    player.id = (int)row["id"];
                    player.name = (string)row["name"];
                    player.accountId = (int)row["account_id"];
                    player.account = (string)row["aname"];
                    player.className = (string)row["class"];
                    float x = (float)row["x"];
                    float y = (float)row["y"];
#if !_iMMO2D
                    float z = (float)row["z"];
                    Vector3 position = new Vector3(x, y, z);
#else
                Vector2 position = new Vector2(x, y);
#endif
                    player.level.current = Mathf.Min((int)row["level"], player.level.max);
                    int health = (int)row["health"];
                    int mana = (int)row["mana"];
#if _iMMOSTAMINA
                    int stamina = (int)row["stamina"];
#endif
                    player.strength.value = (int)row["strength"];
                    player.intelligence.value = (int)row["intelligence"];
                    player.experience.current = (long)row["experience"];
                    ((PlayerSkills)player.skills).skillExperience = (long)row["skillExperience"];
                    player.gold = (long)row["gold"];
#if !_iMMO2D
                    player.isGameMaster = (bool)row["gamemaster"];
#endif
                    //player.isGameMaster = row.gamemaster;
                    player.itemMall.coins = (long)row["coins"];

                    if (player.movement.IsValidSpawnPoint(position))
                    {
                        // agent.warp is recommended over transform.position and
                        // avoids all kinds of weird bugs
                        player.movement.Warp(position);
                    }
                    // otherwise warp to start position
                    else
                    {
                        Transform start = NetworkManagerMMO.GetNearestStartPosition(position);
                        player.movement.Warp(start.position);
                        // no need to show the message all the time. it would spam
                        // the server logs too much.
                        //Debug.Log(player.name + " spawn position reset because it's not on a NavMesh anymore. This can happen if the player previously logged out in an instance or if the Terrain was changed.");
                    }

                    LoadEquipment((PlayerEquipment)player.equipment); // Obliger de le charger � la preview

                    if (!isPreview)
                    {
                        LoadInventory(player.inventory);
                        LoadItemCooldowns(player);
                        LoadSkills((PlayerSkills)player.skills);
                        LoadBuffs((PlayerSkills)player.skills);
#if !_iMMOQUESTS
                    LoadQuests(player.quests);
#endif
                        LoadGuildOnDemand(player.guild);
                    }



                    // addon system hooks
                    onCharacterLoad.Invoke(player, isPreview);
                    if (!isPreview)
                        MySqlHelper.ExecuteNonQuery("UPDATE characters SET online=1, lastsaved=@lastsaved WHERE name=@name", new MySqlParameter("@name", characterName), new MySqlParameter("@lastsaved", DateTime.UtcNow));
                    //MySqlHelper.ExecuteNonQuery(connectionString, "UPDATE characters SET online=1, lastsaved=@lastsaved WHERE name=@name", new MySqlParameter("@name", characterName), new MySqlParameter("@lastsaved", DateTime.UtcNow));

                    player.health.current = health;
                    player.mana.current = mana;
#if _iMMOSTAMINA
                    //Debug.Log(" -> "+stamina);
                    player.stamina.current = stamina;
#endif


                    return go;
                }
            }
            else Debug.LogError("no prefab found for class: " + row["class"]);
#endif
            return null;
        }

#if _SERVER
        // -----------------------------------------------------------------------------------
        // SaveInventory
        // -----------------------------------------------------------------------------------
        void SaveInventory(PlayerInventory inventory, bool createCharacter, MySqlCommand command)
        {
            if (enableDirtyFlags && !createCharacter && !inventory.isDirtyInventory) return;

            MySqlParameter characterIdParameter = new("@characterId", MySqlDbType.Int32) { Value = inventory.player.id };

            bool forceThread = !createCharacter && useThread;
            // Supprimer les entr�es existantes pour ce personnage
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_inventory WHERE `character_id`=@characterId", forceThread, characterIdParameter);

            // Construction de la requ�te INSERT group�e
#if !_iMMO2D
            string insertQuery = "INSERT INTO character_inventory (`character_id`, `slot`, `name`, `amount`, `durability`, `summonedHealth`, `summonedLevel`, `summonedExperience`, `equipmentLevel`, `equipmentGems`) VALUES ";
#else
        string insertQuery = "INSERT INTO character_inventory (`character_id`, `slot`, `name`, `amount`, `summonedHealth`, `summonedLevel`, `summonedExperience`, `equipmentLevel`, `equipmentGems`) VALUES ";
#endif

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            for (int i = 0; i < inventory.slots.Count; ++i)
            {
                ItemSlot slot = inventory.slots[i];
                if (slot.amount > 0)
                {
                    if (parameters.Count > 0)
                        insertQuery += ",";
#if !_iMMO2D
                    insertQuery += "(@character_id_" + i + ", @slot_" + i + ", @name_" + i + ", @amount_" + i + ", @durability_" + i + ", @summonedHealth_" + i + ", @summonedLevel_" + i + ", @summonedExperience_" + i + ", @equipmentLevel_" + i + ", @equipmentGems_" + i + ")";
#else
                insertQuery += "(@character_id_" + i + ", @slot_" + i + ", @name_" + i + ", @amount_" + i + ", @summonedHealth_" + i + ", @summonedLevel_" + i + ", @summonedExperience_" + i + ", @equipmentLevel_" + i + ", @equipmentGems_" + i + ")";
#endif

                    parameters.Add(new MySqlParameter("@character_id_" + i, inventory.player.id));
                    parameters.Add(new MySqlParameter("@slot_" + i, i));
                    parameters.Add(new MySqlParameter("@name_" + i, slot.item.name));
                    parameters.Add(new MySqlParameter("@amount_" + i, slot.amount));
#if !_iMMO2D
                    parameters.Add(new MySqlParameter("@durability_" + i, slot.item.durability));
#endif
                    parameters.Add(new MySqlParameter("@summonedHealth_" + i, slot.item.summonedHealth));
                    parameters.Add(new MySqlParameter("@summonedLevel_" + i, slot.item.summonedLevel));
                    parameters.Add(new MySqlParameter("@summonedExperience_" + i, slot.item.summonedExperience));
                    parameters.Add(new MySqlParameter("@equipmentLevel_" + i, slot.item.equipmentLevel));
                    parameters.Add(new MySqlParameter("@equipmentGems_" + i, slot.item.equipmentGems));
                }
            }

            // Ex�cution de la requ�te si des param�tres ont �t� ajout�s
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, forceThread, parameters.ToArray());
            }
            inventory.isDirtyInventory = false;
        }

        void SaveItemCooldowns(Player player, bool createCharacter, MySqlCommand command)
        {
            if (enableDirtyFlags && !createCharacter && !player.isDirtyItemCooldowns) return;
            bool forceThread = !createCharacter && useThread;
            // equipment: remove old entries first, then add all new ones
            // (we could use UPDATE where slot=... but deleting everything makes
            //  sure that there are never any ghosts)

            // Supprimer les entr�es existantes pour ce personnage
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_itemcooldowns WHERE `character_id`=@character_id", forceThread, new MySqlParameter("@character_id", player.id));

            // Construction de la requ�te INSERT group�e
            string insertQuery = "INSERT INTO character_itemcooldowns (`character_id`, `category`, `cooldownEnd`) VALUES ";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            int index = 0;
            foreach (KeyValuePair<string, double> kvp in player.itemCooldowns)
            {
                float cooldown = player.GetItemCooldown(kvp.Key);
                if (cooldown > 0)
                {
                    if (parameters.Count > 0)
                        insertQuery += ",";

                    insertQuery += "(@character_id_" + index + ", @category_" + index + ", @cooldownEnd_" + index + ")";

                    parameters.Add(new MySqlParameter("@character_id_" + index, player.id));
                    parameters.Add(new MySqlParameter("@category_" + index, kvp.Key));
                    parameters.Add(new MySqlParameter("@cooldownEnd_" + index, cooldown));

                    index++;
                }
            }

            // Ex�cution de la requ�te si des param�tres ont �t� ajout�s
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, forceThread, parameters.ToArray());
            }

            player.isDirtyItemCooldowns = false;
        }

        // -----------------------------------------------------------------------------------
        // SaveEquipment
        // -----------------------------------------------------------------------------------
        void SaveEquipment(PlayerEquipment equipment, bool createCharacter, MySqlCommand command)
        {
            if (enableDirtyFlags && !createCharacter && !equipment.isDirtyEquipment) return;
            bool forceThread = !createCharacter && useThread;
            // Supprimer les entr�es existantes pour ce personnage
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_equipment WHERE `character_id`=@character_id", forceThread, new MySqlParameter("@character_id", equipment.player.id));

            // Construction de la requ�te INSERT group�e
#if !_iMMO2D
            string insertQuery = "INSERT INTO character_equipment (`character_id`, `slot`, `name`, `amount`, `durability`, `summonedHealth`, `summonedLevel`, `summonedExperience`, `equipmentLevel`, `equipmentGems`) VALUES ";
#else
        string insertQuery = "INSERT INTO character_equipment (`character_id`, `slot`, `name`, `amount`, `summonedHealth`, `summonedLevel`, `summonedExperience`, `equipmentLevel`, `equipmentGems`) VALUES ";
#endif
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            int index = 0;

            for (int i = 0; i < equipment.slots.Count; ++i)
            {
                ItemSlot slot = equipment.slots[i];
                if (slot.amount > 0)
                {
                    if (index > 0)
                        insertQuery += ",";
#if !_iMMO2D
                    insertQuery += "(@character_id_" + index + ", @slot_" + index + ", @name_" + index + ", @amount_" + index + ", @durability_" + index + ", @summonedHealth_" + index + ", @summonedLevel_" + index + ", @summonedExperience_" + index + ", @equipmentLevel_" + index + ", @equipmentGems_" + index + ")";
#else
                    insertQuery += "(@character_id_" + index + ", @slot_" + index + ", @name_" + index + ", @amount_" + index + ", @summonedHealth_" + index + ", @summonedLevel_" + index + ", @summonedExperience_" + index + ", @equipmentLevel_" + index + ", @equipmentGems_" + index + ")";
#endif

                    parameters.Add(new MySqlParameter("@character_id_" + index, equipment.player.id));
                    parameters.Add(new MySqlParameter("@slot_" + index, i));
                    parameters.Add(new MySqlParameter("@name_" + index, slot.item.name));
                    parameters.Add(new MySqlParameter("@amount_" + index, slot.amount));
#if !_iMMO2D
                    parameters.Add(new MySqlParameter("@durability_" + index, slot.item.durability));
#endif
                    parameters.Add(new MySqlParameter("@summonedHealth_" + index, slot.item.summonedHealth));
                    parameters.Add(new MySqlParameter("@summonedLevel_" + index, slot.item.summonedLevel));
                    parameters.Add(new MySqlParameter("@summonedExperience_" + index, slot.item.summonedExperience));
                    parameters.Add(new MySqlParameter("@equipmentLevel_" + index, slot.item.equipmentLevel));
                    parameters.Add(new MySqlParameter("@equipmentGems_" + index, slot.item.summonedExperience));

                    index++;
                }
            }

            // Ex�cution de la requ�te si des param�tres ont �t� ajout�s
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, forceThread, parameters.ToArray());
            }
            equipment.isDirtyEquipment = false;
        }

        // -----------------------------------------------------------------------------------
        // SaveSkills
        // -----------------------------------------------------------------------------------
        void SaveSkills(PlayerSkills skills, bool createCharacter, MySqlCommand command)
        {
            if (enableDirtyFlags && !createCharacter && !skills.isDirtySkills) return;
            bool forceThread = !createCharacter && useThread;
            // Supprimer les entr�es existantes pour ce personnage
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_skills WHERE `character_id`=@character_id", forceThread, new MySqlParameter("@character_id", skills.player.id));

            // Construction de la requ�te INSERT group�e
            string insertQuery = "INSERT INTO character_skills (`character_id`, `name`, `level`, `castTimeEnd`, `cooldownEnd`) VALUES ";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            int index = 0;

            foreach (Skill skill in skills.skills)
            {
                if (skill.level >= 0)
                {
                    if (index > 0)
                        insertQuery += ",";

                    insertQuery += "(@character_id_" + index + ", @name_" + index + ", @level_" + index + ", @castTimeEnd_" + index + ", @cooldownEnd_" + index + ")";

                    parameters.Add(new MySqlParameter("@character_id_" + index, skills.player.id));
                    parameters.Add(new MySqlParameter("@name_" + index, skill.name));
                    parameters.Add(new MySqlParameter("@level_" + index, skill.level));
                    parameters.Add(new MySqlParameter("@castTimeEnd_" + index, skill.CastTimeRemaining()));
                    parameters.Add(new MySqlParameter("@cooldownEnd_" + index, skill.CooldownRemaining()));

                    index++;
                }
            }

            // Ex�cution de la requ�te si des param�tres ont �t� ajout�s
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, forceThread, parameters.ToArray());
            }
            skills.isDirtySkills = false;
        }

        // -----------------------------------------------------------------------------------
        // SaveBuffs
        // -----------------------------------------------------------------------------------
        void SaveBuffs(PlayerSkills skills, bool createCharacter, MySqlCommand command)
        {
            if (enableDirtyFlags && !createCharacter && !skills.isDirtyBuffs) return; // en fait le dirty flags pour  les buffs et les skills c'est pas fou fou
            bool forceThread = !createCharacter && useThread;
            // Supprimer les entr�es existantes pour ce personnage
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_buffs WHERE `character_id`=@character_id", forceThread, new MySqlParameter("@character_id", skills.player.id));

            // Construction de la requ�te INSERT group�e
            string insertQuery = "INSERT INTO character_buffs (`character_id`, `name`, `level`, `buffTimeEnd`) VALUES ";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            int index = 0;

            foreach (Buff buff in skills.buffs)
            {
                if (buff.level >= 0) // Assurez-vous que le niveau est valide
                {
                    if (index > 0)
                        insertQuery += ",";

                    insertQuery += "(@character_id_" + index + ", @name_" + index + ", @level_" + index + ", @buffTimeEnd_" + index + ")";

                    parameters.Add(new MySqlParameter("@character_id_" + index, skills.player.id));
                    parameters.Add(new MySqlParameter("@name_" + index, buff.name));
                    parameters.Add(new MySqlParameter("@level_" + index, buff.level));
                    parameters.Add(new MySqlParameter("@buffTimeEnd_" + index, buff.BuffTimeRemaining()));

                    index++;
                }
            }

            // Ex�cution de la requ�te si des param�tres ont �t� ajout�s
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, forceThread, parameters.ToArray());
            }
            skills.isDirtyBuffs = false;
        }

        // -----------------------------------------------------------------------------------
        // SaveQuests
        // -----------------------------------------------------------------------------------
        void SaveQuests(PlayerQuests quests, bool createCharacter, MySqlCommand command)
        {
            if (enableDirtyFlags && !createCharacter && !quests.isDirtyQuests) return; // en fait le dirty flags pour  les buffs et les skills c'est pas fou fou
            bool forceThread = !createCharacter && useThread;
            // Supprimer les entr�es existantes pour ce personnage
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_quests WHERE `character_id`=@character_id", forceThread, new MySqlParameter("@character_id", quests.player.id));

            // Construction de la requ�te INSERT group�e
            string insertQuery = "INSERT INTO character_quests (`character_id`, `name`, `progress`, `completed`) VALUES ";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            int index = 0;

            foreach (Quest quest in quests.quests)
            {
                // Assurez-vous que les donn�es sont valides avant de les ajouter
                if (quest.progress >= 0) // Ajustez cette condition selon les besoins
                {
                    if (index > 0)
                        insertQuery += ",";

                    insertQuery += "(@character_id_" + index + ", @name_" + index + ", @progress_" + index + ", @completed_" + index + ")";

                    parameters.Add(new MySqlParameter("@character_id_" + index, quests.player.id));
                    parameters.Add(new MySqlParameter("@name_" + index, quest.name));
                    parameters.Add(new MySqlParameter("@progress_" + index, quest.progress));
                    parameters.Add(new MySqlParameter("@completed_" + index, quest.completed));

                    index++;
                }
            }

            // Ex�cution de la requ�te si des param�tres ont �t� ajout�s
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, forceThread, parameters.ToArray());
            }
            quests.isDirtyQuests = true;
        }

        // -----------------------------------------------------------------------------------
        // CharacterSave
        // -----------------------------------------------------------------------------------
        void CharacterCreate(Player player, bool online, bool createCharacter, MySqlCommand command)
        {
            DateTime? onlineTimestamp = null;
            bool forceThread = !createCharacter && useThread;
            if (!online)
                onlineTimestamp = DateTime.Now;
            int accountId = ((int)MySqlHelper.ExecuteScalar("SELECT id FROM accounts WHERE name=@name", new MySqlParameter("@name", player.account)));
            //Debug.LogError("account id :" +accountId );
#if _iMMO2D
        var query = @"INSERT INTO characters SET name=@name, account_id=@account_id, class = @class, x = @x, y = @y, level = @level, health = @health, mana = @mana, stamina = @stamina, strength = @strength, intelligence = @intelligence, experience = @experience, skillExperience = @skillExperience, gold = @gold, coins = @coins, online = @online, lastsaved = @lastsaved, deleted = 0";
#else
            var query = @"INSERT INTO characters SET name=@name, account_id=@account_id, class = @class, x = @x, y = @y, z = @z, level = @level, health = @health, mana = @mana, stamina = @stamina, strength = @strength, intelligence = @intelligence, experience = @experience, skillExperience = @skillExperience, gold = @gold, coins = @coins, gamemaster = @gamemaster, online = @online, lastsaved = @lastsaved, deleted = 0";
#endif
            Debug.Log("-> " + player.name + " threaded :" + forceThread);
            //ExecuteNonQueryMySql(command, query,
            MySqlHelper.ExecuteNonQuery(connectionString, query,
                forceThread,
                        new MySqlParameter("@name", player.name),
                        new MySqlParameter("@account_id", accountId),
                        new MySqlParameter("@class", player.className),
                        new MySqlParameter("@x", player.transform.position.x),
                        new MySqlParameter("@y", player.transform.position.y),
#if !_iMMO2D
                        new MySqlParameter("@z", player.transform.position.z),
#endif
                        //new MySqlParameter("@ry", player.transform.rotation.x),
                        new MySqlParameter("@level", player.level.current),
                        new MySqlParameter("@health", player.health.current),
                        new MySqlParameter("@mana", player.mana.current),
                        new MySqlParameter("@stamina",
#if _iMMOSTAMINA
                        player.stamina.current
#else
                    "0"
#endif
                        ),
                        new MySqlParameter("@strength", player.strength.value),
                        new MySqlParameter("@intelligence", player.intelligence.value),
                        new MySqlParameter("@experience", player.experience.current),
                        new MySqlParameter("@skillExperience", ((PlayerSkills)player.skills).skillExperience),
                        new MySqlParameter("@gold", player.gold),
                        new MySqlParameter("@coins", player.itemMall.coins),
#if !_iMMO2D
                        new MySqlParameter("@gamemaster", player.isGameMaster),
#endif
                        new MySqlParameter("@online", online ? 1 : 0),
                        new MySqlParameter("@lastsaved", DateTime.UtcNow)
                    );

#if _iMMOLOBBY
        if(!online)
        {
            LogoutAccountUpdate(player.account, online);
        }
#endif
            player.id = (int)MySqlHelper.ExecuteScalar("SELECT id FROM characters WHERE name=@name", new MySqlParameter("@name", player.name));
            Debug.Log("CharacterCreate : " + createCharacter);
            SaveInventory(player.inventory, createCharacter, command);
            SaveEquipment((PlayerEquipment)player.equipment, createCharacter, command);
            SaveItemCooldowns(player, createCharacter, command);
            SaveSkills((PlayerSkills)player.skills, createCharacter, command);
            SaveBuffs((PlayerSkills)player.skills, createCharacter, command);
#if !_iMMOQUESTS
        SaveQuests(player.quests, createCharacter, command);
#endif
            if (player.guild.InGuild())
                SaveGuild(player.guild.guild, false); // TODO only if needs saving? but would be complicated

            // addon system hooks
            onCharacterSave.Invoke(player, createCharacter);

        }

        // -----------------------------------------------------------------------------------
        // CharacterSave
        // -----------------------------------------------------------------------------------
        void CharacterSave(Player player, bool online, bool createCharacter, MySqlCommand command)
        {
            DateTime? onlineTimestamp = null;
            bool forceThread = !createCharacter && useThread;
            if (!online)
                onlineTimestamp = DateTime.Now;
#if _iMMO2D
        var query = @"UPDATE characters SET account_id=@account_id, class = @class, x = @x, y = @y, level = @level, health = @health, mana = @mana, stamina = @stamina, strength = @strength, intelligence = @intelligence, experience = @experience, skillExperience = @skillExperience, gold = @gold, coins = @coins, online = @online, lastsaved = @lastsaved, deleted = 0 where id = @character_id";
#else
            var query = @"UPDATE characters SET account_id=@account_id, name=@name, class = @class, x = @x, y = @y, z = @z, level = @level, health = @health, mana = @mana, stamina = @stamina, strength = @strength, intelligence = @intelligence, experience = @experience, skillExperience = @skillExperience, gold = @gold, coins = @coins, gamemaster = @gamemaster, online = @online, lastsaved = @lastsaved, deleted = 0 where id = @character_id ";
#endif
            Debug.Log("-> " + player.name + " threaded :" + forceThread);

            MySqlHelper.ExecuteNonQuery(connectionString, query,
                forceThread,
                        new MySqlParameter("@name", player.name),
                        new MySqlParameter("@account_id", player.accountId),
                        new MySqlParameter("@class", player.className),
                        new MySqlParameter("@x", player.transform.position.x),
                        new MySqlParameter("@y", player.transform.position.y),
#if !_iMMO2D
                        new MySqlParameter("@z", player.transform.position.z),
#endif
                        new MySqlParameter("@level", player.level.current),
                        new MySqlParameter("@health", player.health.current),
                        new MySqlParameter("@mana", player.mana.current),
                        new MySqlParameter("@stamina",
#if _iMMOSTAMINA
                        player.stamina.current
#else
                    "0"
#endif
                        ),
                        new MySqlParameter("@strength", player.strength.value),
                        new MySqlParameter("@intelligence", player.intelligence.value),
                        new MySqlParameter("@experience", player.experience.current),
                        new MySqlParameter("@skillExperience", ((PlayerSkills)player.skills).skillExperience),
                        new MySqlParameter("@gold", player.gold),
                        new MySqlParameter("@coins", player.itemMall.coins),
#if !_iMMO2D
                        new MySqlParameter("@gamemaster", player.isGameMaster),
#endif
                        new MySqlParameter("@online", online ? 1 : 0),
                        new MySqlParameter("@lastsaved", DateTime.UtcNow),
                        new MySqlParameter("@character_id", player.id)
                    );

#if _iMMOLOBBY
        if(!online)
        {
            LogoutAccountUpdate(player.account, online);
        }
#endif
            SaveInventory(player.inventory, createCharacter, command);
            SaveEquipment((PlayerEquipment)player.equipment, createCharacter, command);
            SaveItemCooldowns(player, createCharacter, command);
            SaveSkills((PlayerSkills)player.skills, createCharacter, command);
            SaveBuffs((PlayerSkills)player.skills, createCharacter, command);
#if !_iMMOQUESTS
        SaveQuests(player.quests, createCharacter, command);
#endif
            if (player.guild.InGuild())
                SaveGuild(player.guild.guild, false); // TODO only if needs saving? but would be complicated

            // addon system hooks
            onCharacterSave.Invoke(player, createCharacter);

        }

        // -----------------------------------------------------------------------------------
        // CharacterSave
        // -----------------------------------------------------------------------------------
        public void CharacterSave(Player player, bool online, bool useTransaction = true)
        {
            MySqlHelper.ExecuteTransaction(connectionString, command =>
            {
                CharacterSave(player, online, useThread, command);
            });
        }

#endif
        // -----------------------------------------------------------------------------------
        // CharacterSave
        // -----------------------------------------------------------------------------------
        public void CharacterSave(Player player, bool online, bool useTransaction = true, bool createCharacter = false)
        {
#if _SERVER
            MySqlHelper.ExecuteTransaction(connectionString, command =>
            {
                CharacterSave(player, online, createCharacter, command);
            });
#endif
        }

#if _SERVER
        // -----------------------------------------------------------------------------------
        // CharacterCreate
        // -----------------------------------------------------------------------------------
        public void CharacterCreate(Player player, bool online, bool useTransaction = true, bool createCharacter = false)
        {
            MySqlHelper.ExecuteTransaction(connectionString, command =>
            {
                CharacterCreate(player, online, createCharacter, command);
            });
        }

        // -----------------------------------------------------------------------------------
        // CharacterSaveMany
        // -----------------------------------------------------------------------------------

        public void CharacterSaveMany(IEnumerable<Player> players, bool online = true)
        {
            try
            {
                if (useThread)
                    batch.Clear();

                MySqlHelper.ExecuteTransaction(connectionString, command =>
                {
                    foreach (Player player in players)
                    {
                        CharacterSave(player, online, false, command);
                    }
                });

                if (useThread)
                {
                    // Une fois la liste construite, on lance l'ex�cution asynchrone des requ�tes
                    Task.Run(() =>
                    {
                        ExecuteBatch(batch);
                    });
                }
            }
            catch (Exception e)
            {
                Debug.Log("SQL error :" + e);
            }
        }

        public void ExecuteBatch(List<(string sql, MySqlParameter[] parameters)> sqlBatch)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = transaction;

                            foreach (var (sql, parameters) in sqlBatch)
                            {
                                //Debug.Log("Executing: " + sql);

                                // Construire la requête SQL à la volée
                                cmd.CommandText = sql;

                                // Ajouter les paramètres à la commande
                                cmd.Parameters.Clear();  // Réinitialiser les paramètres avant chaque exécution
                                cmd.Parameters.AddRange(parameters);

                                // Exécuter la requête
                                cmd.ExecuteNonQuery();
                            }

                            // Valider la transaction
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Error executing batch: " + ex.Message);
            }
        }

        // -----------------------------------------------------------------------------------
        // SaveGuild
        // -----------------------------------------------------------------------------------
        public void SaveGuild(Guild guild, bool useTransaction = true)
        {
            MySqlHelper.ExecuteTransaction(connectionString, command =>
            {
                int guildId = 0;
                MySqlParameter guildNameParameter = new("@name", MySqlDbType.VarChar) { Value = guild.name };
                var loadGuild = MySqlHelper.ExecuteDataRow("SELECT id FROM guild_info WHERE `name`=@name", guildNameParameter);
                if (loadGuild != null)
                {
                    guildId = (int)loadGuild["id"];
                    var query1 = @"UPDATE guild_info SET `notice` = @notice WHERE id = @guild_id";
                    MySqlHelper.ExecuteNonQuery(query1, new MySqlParameter("@notice", guild.notice), new MySqlParameter("@guild_id", (int)loadGuild["id"]));
                }
                else
                {
                    var query = @"INSERT INTO guild_info SET `name` = @guildName, `notice` = @notice";
                    MySqlHelper.ExecuteNonQuery(query, new MySqlParameter("@guildName", guild.name), new MySqlParameter("@notice", guild.notice));

                    // Récupération de l'ID de la guilde nouvellement créée
                    guildId = Convert.ToInt32(MySqlHelper.ExecuteScalar("SELECT LAST_INSERT_ID()"));
                }

                // Supprimer les entr�es existantes pour cette guilde
                MySqlHelper.ExecuteNonQuery("DELETE FROM character_guild WHERE `guild_id` = @guild_id", new MySqlParameter("@guild_id", guildId));

                // Construction de la requ�te INSERT group�e
                string insertQuery = "INSERT INTO character_guild (`guild_id`, `rank`, `character_id`) VALUES ";

                List<MySqlParameter> parameters = new List<MySqlParameter>();
                int index = 0;

                foreach (GuildMember member in guild.members)
                {
                    if (parameters.Count > 0)
                        insertQuery += ",";

                    insertQuery += "(@guild_id_" + index + ", @rank_" + index + ", @character_id_" + index + ")";

                    parameters.Add(new MySqlParameter("@guild_id_" + index, guildId));
                    parameters.Add(new MySqlParameter("@rank_" + index, member.rank));
                    parameters.Add(new MySqlParameter("@character_id_" + index, member.id));

                    index++;
                }

                // Ex�cution de la requ�te si des param�tres ont �t� ajout�s
                if (parameters.Count > 0)
                {
                    MySqlHelper.ExecuteNonQuery(insertQuery, parameters.ToArray());
                }

            });
        }

#endif
        // -----------------------------------------------------------------------------------
        // GuildExists
        // -----------------------------------------------------------------------------------
        public bool GuildExists(string guild)
        {
#if _SERVER
            return ((long)MySqlHelper.ExecuteScalar("SELECT Count(*) FROM guild_info WHERE `name`=@name", new MySqlParameter("@name", guild))) == 1;
#else
        return false;
#endif
        }

        // -----------------------------------------------------------------------------------
        // RemoveGuild
        // -----------------------------------------------------------------------------------
        public void RemoveGuild(string guild)
        {
#if _SERVER
            MySqlHelper.ExecuteNonQuery("DELETE FROM guild_info WHERE `name`=@name", new MySqlParameter("@name", guild));
#endif
        }

#if _SERVER
        // -----------------------------------------------------------------------------------
        // GrabCharacterOrders
        // -----------------------------------------------------------------------------------
        public List<long> GrabCharacterOrders(int characterId)
        {
            var result = new List<long>();

            var table = MySqlHelper.ExecuteReader("SELECT orderid, coins FROM character_orders WHERE `character_id`=@character_id AND processed=0", new MySqlParameter("@character_id", characterId));

            // Liste pour stocker les param�tres
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            string updateQuery = "UPDATE character_orders SET processed=1 WHERE orderid IN (";

            // Construction de la requ�te UPDATE group�e
            for (int i = 0; i < table.Count; ++i)
            {
                if (i > 0)
                    updateQuery += ",";

                updateQuery += "@orderid_" + i;
                parameters.Add(new MySqlParameter("@orderid_" + i, (long)table[i][0]));

                // Ajouter l'ordre dans la liste des r�sultats
                result.Add((long)table[i][1]);
            }

            updateQuery += ")";

            // Ex�cution de la requ�te si des param�tres ont �t� ajout�s
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(updateQuery, parameters.ToArray());
            }

            return result;
        }
#endif

        // -----------------------------------------------------------------------------------
    }
}
#endif