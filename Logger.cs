using System;
using System.IO;

namespace OrgnTransplant
{
    /// <summary>
    /// Simple logging system for the application
    /// </summary>
    public static class Logger
    {
        private static readonly object lockObj = new object();
        private static string logFilePath;
        private static bool isEnabled;

        static Logger()
        {
            try
            {
                // Initialize from configuration
                isEnabled = ConfigurationHelper.GetAppSettingBool("EnableLogging", true);
                logFilePath = ConfigurationHelper.GetAppSetting("LogFilePath", "Logs\\OrganTransplant.log");

                // Ensure log directory exists
                string logDirectory = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Create full path
                logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFilePath);
            }
            catch
            {
                // Fallback to default
                isEnabled = true;
                logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "OrganTransplant.log");
            }
        }

        /// <summary>
        /// Log an information message
        /// </summary>
        public static void LogInfo(string message)
        {
            Log("INFO", message, null);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        public static void LogWarning(string message, Exception ex = null)
        {
            Log("WARNING", message, ex);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        public static void LogError(string message, Exception ex = null)
        {
            Log("ERROR", message, ex);
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        public static void LogDebug(string message)
        {
            Log("DEBUG", message, null);
        }

        /// <summary>
        /// Internal log method
        /// </summary>
        private static void Log(string level, string message, Exception ex)
        {
            if (!isEnabled)
                return;

            try
            {
                lock (lockObj)
                {
                    string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

                    if (ex != null)
                    {
                        logMessage += $"\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";
                    }

                    // Write to file
                    File.AppendAllText(logFilePath, logMessage + Environment.NewLine);

                    // Also write to debug output
                    System.Diagnostics.Debug.WriteLine(logMessage);
                }
            }
            catch
            {
                // Silent fail - don't let logging crash the app
            }
        }

        /// <summary>
        /// Clear old log files
        /// </summary>
        public static void ClearOldLogs(int daysToKeep = 30)
        {
            try
            {
                string logDirectory = Path.GetDirectoryName(logFilePath);
                if (Directory.Exists(logDirectory))
                {
                    var files = Directory.GetFiles(logDirectory, "*.log");
                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.LastWriteTime < DateTime.Now.AddDays(-daysToKeep))
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("Failed to clear old logs", ex);
            }
        }
    }
}
