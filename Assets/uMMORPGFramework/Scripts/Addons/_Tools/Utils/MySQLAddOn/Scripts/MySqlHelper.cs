using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MySqlConnector;
using UnityEngine;

#if _SERVER && _MYSQL

namespace uMMORPG
{
    
    public static class MySqlHelper
    {
        public static void ExecuteTransaction(string connectionString, Action<MySqlCommand> action)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var command = new MySqlCommand
                        {
                            Connection = conn,
                            Transaction = transaction
                        };
    
                        action(command);
                        // Log de la transaction ici (en consid�rant qu'il peut y avoir plusieurs requ�tes dans la transaction)
                        if (Database.singleton.enableQueryLog)
                            SqlLogger.LogQuery(command.CommandText, command.Parameters.Cast<MySqlParameter>().ToArray());
    
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    
    
        /*public static object ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            using (var cmd = new MySqlCommand(sql, Database.GetConnection())) // Utilise la connexion partag�e
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                // Log de la requ�te
                if (Database.singleton.enableQueryLog)
                    SqlLogger.LogQuery("(Same connection)" + sql, parameters);
                return cmd.ExecuteScalar();
            }
            
        }*/
        public static object ExecuteScalar(string sql, params MySqlParameter[] parameters)
        {
            using var cmd = new MySqlCommand(sql, Database.GetConnection()); // Utilise la connexion partag�e
    
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
    
            // Log de la requ�te
            if (Database.singleton.enableQueryLog)
                SqlLogger.LogQuery("(Same connection) " + sql, parameters);
    
            using var reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            return reader.Read() ? reader[0] : null;
        }
    
        public static object ExecuteScalar(string connectionString, string sql, params MySqlParameter[] parameters)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    // Log de la requ�te
                    if (Database.singleton.enableQueryLog)
                        SqlLogger.LogQuery(sql, parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }
        public static void ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using var cmd = new MySqlCommand(sql, Database.GetConnection()); // Utilise la connexion partag�e
    
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
    
            // Log de la requ�te
            if (Database.singleton.enableQueryLog)
                SqlLogger.LogQuery("(Same connection)" + sql, parameters);
    
            // Ex�cution de la commande avec CommandBehavior.CloseConnection pour fermer la connexion
            cmd.ExecuteNonQuery();
        }
    
        public static void ExecuteNonQuery(string connectionString, string sql, params MySqlParameter[] parameters)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    // Log de la requ�te
                    if (Database.singleton.enableQueryLog)
                        SqlLogger.LogQuery(sql, parameters);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static void ExecuteNonQuery(string connectionString, string sql, bool useThread, params MySqlParameter[] parameters)
        {
            if (useThread)
            {
                //Database.singleton.batch.Add(sql, new List<MySqlParameter[]> { parameters });
                Database.singleton.batch.Add((sql, parameters ));
                SqlLogger.LogQuery("(Threaded) " + sql, parameters);
            }
            else
            {
                using (var cmd = new MySqlCommand(sql, Database.GetConnection())) // Utilise la connexion partag�e
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
    
                    // Log de la requ�te
                    if (Database.singleton.enableQueryLog)
                        SqlLogger.LogQuery("(Same connection)" + sql, parameters);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch(Exception e)
                    {
                        Debug.Log("Error executing query: " + e.Message);
                    }
                }
                /*using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        if (Database.singleton.enableQueryLog)
                            SqlLogger.LogQuery(sql, parameters);
                        cmd.ExecuteNonQuery();
                    }
                }*/
            }
        }
    
        public static DataRow ExecuteDataRow(string sql, params MySqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (var cmd = new MySqlCommand(sql, Database.GetConnection())) // Utilise la connexion partag�e
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
    
                // Log de la requ�te
                if (Database.singleton.enableQueryLog)
                    SqlLogger.LogQuery("(Same connection)" + sql, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0];
                    }
                    return null;
                }
    
            }
        }
        public static DataRow ExecuteDataRow(string connectionString, string sql, params MySqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
    
                    // Log de la requ�te
                    if (Database.singleton.enableQueryLog)
                        SqlLogger.LogQuery(sql, parameters);
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                        if (dt.Rows.Count > 0)
                        {
                            return dt.Rows[0];
                        }
                        return null;
                    }
                }
            }
        }
    
    
        public static DataSet ExecuteDataSet(string connectionString, string sql, params MySqlParameter[] parameters)
        {
            var ds = new DataSet();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
    
                    // Log de la requ�te
                    if (Database.singleton.enableQueryLog)
                        SqlLogger.LogQuery(sql, parameters);
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(ds);
                    }
                }
            }
            return ds;
        }
    
        public static List<List<object>> ExecuteReader(string connectionString, string sql, params MySqlParameter[] parameters)
        {
            var result = new List<List<object>>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
    
                    // Log de la requ�te
                    if (Database.singleton.enableQueryLog)
                        SqlLogger.LogQuery(sql, parameters);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new List<object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetValue(i));
                            }
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }
    
        public static List<List<object>> ExecuteReader(string sql, params MySqlParameter[] parameters)
        {
            var result = new List<List<object>>();
            using (var cmd = new MySqlCommand(sql, Database.GetConnection())) // Utilise la connexion partag�e
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
    
                // Log de la requ�te
                if (Database.singleton.enableQueryLog)
                    SqlLogger.LogQuery("(Same connection)" + sql, parameters);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new List<object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetValue(i));
                        }
                        result.Add(row);
                    }
                }
            }
            
            return result;
        }
    
        /**
         * Using a new connection for the reader is a better practice, not reuse the same connection
         */
        /*public static MySqlDataReader GetReader(string sql, params MySqlParameter[] parameters)
        {
            var cmd = new MySqlCommand(sql, Database.GetConnection()); // Pas de using ici !
    
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
    
            // Log de la requ�te
            if (Database.singleton.enableQueryLog)
                SqlLogger.LogQuery("(Same connection) " + sql, parameters);
    
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }*/
    
        /**
         * now use pooling connection, new mysqConnection reuse connexion if available and force close it when reader is closed
         */
        public static MySqlDataReader GetReader(string connectionString, string sql, params MySqlParameter[] parameters)
        {
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            using (var cmd = new MySqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
    
                // Log de la requ�te
                if (Database.singleton.enableQueryLog)
                    SqlLogger.LogQuery(sql, parameters);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }
    
    }
    
    
}
    #endif
