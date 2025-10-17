using System;
using System.Configuration;

namespace OrgnTransplant
{
    /// <summary>
    /// Helper class for reading application configuration
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Get database connection string from App.config
        /// </summary>
        public static string GetConnectionString()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["OrganTransplantDB"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ConfigurationErrorsException("Connection string 'OrganTransplantDB' not found in App.config");
                }

                return connectionString;
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to read connection string", ex);
                throw;
            }
        }

        /// <summary>
        /// Get app setting by key
        /// </summary>
        public static string GetAppSetting(string key, string defaultValue = "")
        {
            try
            {
                return ConfigurationManager.AppSettings[key] ?? defaultValue;
            }
            catch (Exception ex)
            {
                Logger.LogWarning($"Failed to read app setting '{key}', using default value", ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Get app setting as boolean
        /// </summary>
        public static bool GetAppSettingBool(string key, bool defaultValue = false)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                if (bool.TryParse(value, out bool result))
                    return result;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get app setting as integer
        /// </summary>
        public static int GetAppSettingInt(string key, int defaultValue = 0)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                if (int.TryParse(value, out int result))
                    return result;

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
