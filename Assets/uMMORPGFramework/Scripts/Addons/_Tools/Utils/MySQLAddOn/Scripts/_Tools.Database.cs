using SQLite;
using UnityEngine;

#if _iMMOTOOLS
#if _SQLITE
#endif

namespace uMMORPG
{
     
    // DATABASE CLASSES
    public partial class Database
    {
    #if _MYSQL
        [Header("[-=-[ Use Thread For save ]-=-]")]
        public bool useThread;
        [Tooltip("Enable dirty flags to save only modified data (improves performance).")]
        public bool enableDirtyFlags = false;
    #endif
        public enum DatabaseType { SQLite, mySQL }
        // -----------------------------------------------------------------------------------
        // Tools_connection
        // @ workaround because uMMORPGs default database connection is private.
        // -----------------------------------------------------------------------------------
    #if _SQLITE && _SERVER
        public SQLiteConnection Tools_connection {
            get {
    
                return connection;
            }
        }
    #endif
        [Header("Database Type")]
        public DatabaseType databaseType = DatabaseType.SQLite;
    #if _SERVER || UNITY_EDITOR
        [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
        public Tmpl_DatabaseConfiguration databaseConfiguration;
    
    
        [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
        public string dbHost = "localhost";
        [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
        public string dbName = "dbName";
        [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
        public string dbUser = "dbUser";
        [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
        public string dbPassword = "dbPassword";
        [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
        public uint dbPort = 3306;
        [StringShowConditional(conditionFieldName: "databaseType", conditionValue: "mySQL")]
        public string dbCharacterSet = "utf8mb4";
    #endif
        protected const string DB_SQLITE = "_SQLITE";
        protected const string DB_MYSQL = "_MYSQL";
    
        // -----------------------------------------------------------------------------------
        // OnValidate
        // -----------------------------------------------------------------------------------
    #if UNITY_EDITOR
        private void OnValidate()
        {
            if (databaseType == Database.DatabaseType.SQLite)
            {
                DefineSymbols.RemoveScriptingDefine(DB_MYSQL);
                DefineSymbols.AddScriptingDefine(DB_SQLITE);
            }
            else if (databaseType == Database.DatabaseType.mySQL)
            {
                DefineSymbols.RemoveScriptingDefine(DB_SQLITE);
                DefineSymbols.AddScriptingDefine(DB_MYSQL);
            }
        }
    #endif
    
    #if _SERVER
        public void Start()
        {
    #if _SQLITE
            Utils.InvokeMany(typeof(Database), this, "Start_Tools_");
    #endif
        }
    #endif
        }
    
}
    #endif
