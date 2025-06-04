using System;
using System.IO;
using System.Threading.Tasks;
using MySqlConnector;

#if _SERVER

namespace uMMORPG
{
    
    public static class SqlLogger
    {
        private static readonly string LogFilePath = "queries_log.txt"; // D�finissez ici le chemin du fichier log
    
        public static void LogQuery(string sql, params MySqlParameter[] parameters)
        {
            try
            {
                // Cr�ez une entr�e de log contenant l'heure, la requ�te SQL et les param�tres
                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - SQL: {sql}";
    
                if (parameters != null && parameters.Length > 0)
                {
                    logEntry += " | Params: ";
                    foreach (var param in parameters)
                    {
                        logEntry += $"{param.ParameterName}: {param.Value} ";
                    }
                }
    
                // �crire dans le fichier log
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Si une erreur survient lors de l'�criture dans le fichier, nous affichons l'exception
                Console.WriteLine("Error writing to log: " + ex.Message);
            }
        }
    
        public static async Task LogQueryAsync(string sql, params MySqlParameter[] parameters)
        {
            try
            {
                // Cr�ez une entr�e de log contenant l'heure, la requ�te SQL et les param�tres
                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - (Thread ID :" + System.Threading.Thread.CurrentThread.ManagedThreadId+" )  - SQL: {sql}";
    
                if (parameters != null && parameters.Length > 0)
                {
                    logEntry += " | Params: ";
                    foreach (var param in parameters)
                    {
                        logEntry += $"{param.ParameterName}: {param.Value} ";
                    }
                }
    
                // Utiliser StreamWriter pour �crire de mani�re asynchrone dans le fichier
                using (StreamWriter writer = new StreamWriter(LogFilePath, append: true))
                {
                    await writer.WriteLineAsync(logEntry);
                }
            }
            catch (Exception ex)
            {
                // Si une erreur survient lors de l'�criture dans le fichier, nous affichons l'exception
                Console.WriteLine("Error writing to log: " + ex.Message);
            }
        }
    }
    
}
    #endif
