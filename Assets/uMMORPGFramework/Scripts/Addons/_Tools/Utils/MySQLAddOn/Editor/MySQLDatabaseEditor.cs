#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using MySqlConnector;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace uMMORPG
{
    
    public class MySQLDatabaseEditor : EditorWindow
    {
        public Tmpl_DatabaseConfiguration databaseConfiguration;
        private MySqlConnection connection;
        private List<string> tables = new();
        private int selectedTableIndex = 0;
        private List<Dictionary<string, object>> tableData = new();
        private Vector2 scrollPosTables;
        private Vector2 scrollPosData;
        private string customSqlCommand = "SELECT * FROM characters LIMIT 10";
        private string sqlResult = "";
    
        [MenuItem("MMO-Indie/MySQL Editor (WIP)")]
        public static void ShowWindow()
        {
            GetWindow<MySQLDatabaseEditor>("MySQL DB Editor");
        }
    
        private void OnEnable()
        {
            TryConnectAndLoad();
        }
    
        private void TryConnectAndLoad()
        {
            if (databaseConfiguration == null)
                return;
    
            Connect();
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                LoadTables();
                LoadTableData();
            }
        }
    
        private void Connect()
        {
            try
            {
                string connStr = $"server={databaseConfiguration.databaseHost};user={databaseConfiguration.databaseUser};database={databaseConfiguration.databaseName};password={databaseConfiguration.databasePassword};SslMode=none;";
                connection = new MySqlConnection(connStr);
                connection.Open();
            }
            catch (MySqlException ex)
            {
                Debug.LogError("Erreur de connexion MySQL: " + ex.Message);
            }
        }
    
        private void LoadTables()
        {
            tables.Clear();
            using var cmd = new MySqlCommand("SHOW TABLES", connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                tables.Add(reader.GetString(0));
        }
    
        private void LoadTableData()
        {
            tableData.Clear();
            if (tables.Count == 0) return;
    
            string tableName = tables[selectedTableIndex];
    
            string extraColumns = "";
            string joins = "";
    
            if (TableHasColumn(tableName, "character_id"))
            {
                extraColumns += ", characters.name AS NomduJoueur";
                joins += " LEFT JOIN characters ON characters.id = t.character_id";
            }
    
            if (TableHasColumn(tableName, "account_id"))
            {
                extraColumns += ", accounts.name AS Compte";
                joins += " LEFT JOIN accounts ON accounts.id = t.account_id";
            }
    
            string query = $"SELECT t.*{extraColumns} FROM `{tableName}` t{joins} LIMIT 100";
    
            using var cmd = new MySqlCommand(query, connection);
            using var reader = cmd.ExecuteReader();
    
            while (reader.Read())
            {
                Dictionary<string, object> row = new();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.GetValue(i);
                tableData.Add(row);
            }
        }
    
        private bool TableHasColumn(string tableName, string columnName)
        {
            using var cmd = new MySqlCommand($"SHOW COLUMNS FROM `{tableName}` LIKE '{columnName}'", connection);
            using var reader = cmd.ExecuteReader();
            return reader.HasRows;
        }
    
        private void ExecuteCustomSQL()
        {
            sqlResult = "";
            if (string.IsNullOrWhiteSpace(customSqlCommand)) return;
    
            try
            {
                using var cmd = new MySqlCommand(customSqlCommand, connection);
                using var reader = cmd.ExecuteReader();
    
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                        sqlResult += reader.GetName(i) + ": " + reader.GetValue(i) + " | ";
                    sqlResult += "\n";
                }
            }
            catch (MySqlException ex)
            {
                sqlResult = "Erreur: " + ex.Message;
            }
        }
    
        private void OnGUI()
        {
            databaseConfiguration = (Tmpl_DatabaseConfiguration)EditorGUILayout.ObjectField("Config", databaseConfiguration, typeof(Tmpl_DatabaseConfiguration), false);
    
            if (databaseConfiguration != null && (connection == null || connection.State != System.Data.ConnectionState.Open))
                TryConnectAndLoad();
    
            if (connection == null || connection.State != System.Data.ConnectionState.Open)
            {
                EditorGUILayout.HelpBox("Non connect� � la base MySQL.", MessageType.Error);
                return;
            }
    
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
    
            GUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("Tables", EditorStyles.boldLabel);
            scrollPosTables = EditorGUILayout.BeginScrollView(scrollPosTables, GUILayout.Width(200), GUILayout.ExpandHeight(true));
    
            for (int i = 0; i < tables.Count; i++)
            {
                if (GUILayout.Button(tables[i], (i == selectedTableIndex) ? EditorStyles.toolbarButton : EditorStyles.miniButton))
                {
                    selectedTableIndex = i;
                    LoadTableData();
                }
            }
    
            EditorGUILayout.EndScrollView();
    
            if (GUILayout.Button("Supprimer cette table", GUILayout.Height(25)))
                DeleteCurrentTable();
    
            GUILayout.EndVertical();
    
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label($"Table : {tables[selectedTableIndex]}", EditorStyles.boldLabel);
    
            scrollPosData = EditorGUILayout.BeginScrollView(scrollPosData);
    
            if (tableData.Count == 0)
            {
                EditorGUILayout.HelpBox("Aucune donn�e � afficher.", MessageType.Info);
            }
            else
            {
                foreach (var row in tableData)
                {
                    Rect boxRect = EditorGUILayout.BeginVertical("box");
                    GUILayout.Space(2);
    
                    Rect buttonRect = new Rect(boxRect.xMax - 25, boxRect.yMin + 5, 20, 20);
                    if (GUI.Button(buttonRect, "X", EditorStyles.miniButtonMid))
                    {
                        DeleteRow(row);
                        break;
                    }
    
                    foreach (var kvp in row)
                    {
                        // Ne pas afficher les champs suppl�mentaires ici, ils seront trait�s � c�t� du champ principal
                        if (kvp.Key == "NomduJoueur" || kvp.Key == "Compte")
                            continue;
    
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(kvp.Key, GUILayout.Width(100));
    
                        string input = GUILayout.TextField(kvp.Value?.ToString() ?? "", GUILayout.Width(200));
    
                        if (!Regex.IsMatch(input, "^[a-zA-Z0-9_]*$"))
                            input = Regex.Replace(input, "[^a-zA-Z0-9_]", "");
    
                        // Affichage � c�t� pour les champs li�s
                        if (kvp.Key == "character_id" && row.ContainsKey("NomduJoueur"))
                        {
                            GUILayout.Label($"({row["NomduJoueur"]})", GUILayout.Width(150));
                        }
                        else if (kvp.Key == "account_id" && row.ContainsKey("Compte"))
                        {
                            GUILayout.Label($"({row["Compte"]})", GUILayout.Width(150));
                        }
    
                        EditorGUILayout.EndHorizontal();
                    }
    
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5);
                }
            }
    
            EditorGUILayout.EndScrollView();
    
            GUILayout.Space(20);
            GUILayout.Label("Ex�cuter une commande SQL", EditorStyles.boldLabel);
            customSqlCommand = EditorGUILayout.TextField("Commande SQL:", customSqlCommand);
            if (GUILayout.Button("Ex�cuter"))
                ExecuteCustomSQL();
    
            if (!string.IsNullOrEmpty(sqlResult))
                EditorGUILayout.TextArea(sqlResult, GUILayout.Height(100));
    
            GUILayout.EndVertical();
    
            GUILayout.EndHorizontal();
        }
    
        private void DeleteCurrentTable()
        {
            string tableName = tables[selectedTableIndex];
            if (EditorUtility.DisplayDialog("Supprimer la table", $"Es-tu s�r de vouloir supprimer la table '{tableName}' ?", "Oui", "Non"))
            {
                using var cmd = new MySqlCommand($"DROP TABLE `{tableName}`", connection);
                cmd.ExecuteNonQuery();
                LoadTables();
                selectedTableIndex = 0;
                tableData.Clear();
            }
        }
    
        private void DeleteRow(Dictionary<string, object> row)
        {
            string tableName = tables[selectedTableIndex];
            var primaryKeys = GetPrimaryKeys(tableName);
    
            if (primaryKeys.Count == 0)
            {
                Debug.LogWarning("Impossible de supprimer la ligne : aucune cl� primaire trouv�e.");
                return;
            }
    
            string sql = $"DELETE FROM `{tableName}` WHERE ";
            List<string> conditions = new();
            var cmd = new MySqlCommand();
            cmd.Connection = connection;
    
            for (int i = 0; i < primaryKeys.Count; i++)
            {
                string key = primaryKeys[i];
                object val = row[key];
                string paramName = $"@param{i}";
                conditions.Add($"`{key}` = {paramName}");
                cmd.Parameters.AddWithValue(paramName, val);
            }
    
            sql += string.Join(" AND ", conditions);
            sql += " LIMIT 1";
    
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
    
            LoadTableData();
        }
    
        private List<string> GetPrimaryKeys(string tableName)
        {
            List<string> keys = new();
            using var cmd = new MySqlCommand($"SHOW KEYS FROM `{tableName}` WHERE Key_name = 'PRIMARY'", connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                keys.Add(reader.GetString("Column_name"));
            return keys;
        }
    
        private void OnDisable()
        {
            connection?.Close();
        }
    }
    
}
#endif