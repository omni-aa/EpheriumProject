using UnityEngine;

namespace uMMORPG
{
    
    [CreateAssetMenu(fileName = "New Attribute", menuName = "MMO-Indie/Tools/MySQL Information", order = 999)]
    public class Tmpl_DatabaseConfiguration : ScriptableObject
    {
        [Header("")]
        public string databaseHost = "localhost";
        public string databaseUser = "";
        public string databasePassword = "";
        public string databaseName = "";
        public uint databasePort = 3306;
        public string charecteSet = "utf8mb4";
    
    }
    
}
