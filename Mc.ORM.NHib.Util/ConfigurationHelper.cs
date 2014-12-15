using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mc.ORM.NHib.Util
{
    public class ConfigurationHelper
    {
        /// <summary>
        /// Get Connection String
        /// </summary>
        /// <param name="connectionStringKey">Connection string Key </param>
        /// <param name="workingdirectory">Working directory</param>
        /// <param name="configFileName">Config file path</param>
        /// <param name="configSource">ConfigSourceEnum.FromAppSttings|ConfigSourceEnum.FromConnectionStringSection</param>
        /// <returns></returns>
        public static string GetConnectionString(string connectionStringKey, string workingdirectory = "", string configFileName = "", ConfigSourceEnum configSource = ConfigSourceEnum.FromAppSttings)
        {
            configFileName = GetFileConfigPath(connectionStringKey, workingdirectory, configFileName, ref configSource);

            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFileName };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            switch (configSource)
            {
                case ConfigSourceEnum.FromAppSttings:
                    return config.ConnectionStrings.ConnectionStrings[config.AppSettings.Settings[connectionStringKey].Value].ConnectionString;
                case ConfigSourceEnum.FromConnectionStringSection:
                    return config.ConnectionStrings.ConnectionStrings[connectionStringKey].ConnectionString;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get file config path
        /// </summary>
        /// <param name="connectionStringKey"></param>
        /// <param name="workingdirectory"></param>
        /// <param name="configFileName"></param>
        /// <param name="configSource"></param>
        /// <returns></returns>
        private static string GetFileConfigPath(string connectionStringKey, string workingdirectory, string configFileName, ref ConfigSourceEnum configSource)
        {
            if (string.IsNullOrEmpty(connectionStringKey))
                throw new ArgumentNullException("connectionStringKey");
            if (string.IsNullOrEmpty(workingdirectory))
                workingdirectory = Environment.CurrentDirectory;
            if (string.IsNullOrEmpty(configFileName) && configSource == ConfigSourceEnum.FromAppSttings)
            {
                configFileName = Path.Combine(workingdirectory, "Web.config");
                if (!File.Exists(configFileName))
                    throw new ApplicationException("Config File not found");
            }
            else
            {
                if (!File.Exists(configFileName))
                {
                    configFileName = Path.Combine(workingdirectory, "ConnectionStrings.config");
                    configSource = ConfigSourceEnum.FromConnectionStringSection;
                }
                if (!File.Exists(configFileName))
                    throw new ApplicationException("Config File not found");
            }
            return configFileName;
        }

        /// <summary>
        /// ConfigSource Enum
        /// </summary>
        public enum ConfigSourceEnum
        {
            FromConnectionStringSection,
            FromAppSttings
        }
    }
}
