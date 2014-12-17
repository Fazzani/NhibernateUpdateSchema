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
        /// <param name="options"> </param>
        /// <param name="configSource">ConfigSourceEnum.FromAppSttings|ConfigSourceEnum.FromConnectionStringSection</param>
        /// <returns></returns>
        public static string GetConnectionString(SchemaManagerOptions options, ConfigSourceEnum configSource = ConfigSourceEnum.FromAppSttings)
        {
            options.ConfigName = GetFileConfigPath(options.ConfigDirectory, options.ConfigName, ref configSource);

            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = options.ConfigName };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            switch (configSource)
            {
                case ConfigSourceEnum.FromAppSttings:
                    if (!string.IsNullOrEmpty(options.ConnectionStringKeyValue))
                        return config.ConnectionStrings.ConnectionStrings[options.ConnectionStringKeyValue].ConnectionString;
                    return config.ConnectionStrings.ConnectionStrings[config.AppSettings.Settings[options.ConnectionStringKey].Value].ConnectionString;
                case ConfigSourceEnum.FromConnectionStringSection:
                    if (!string.IsNullOrEmpty(options.ConnectionStringKeyValue))
                        return config.ConnectionStrings.ConnectionStrings[options.ConnectionStringKeyValue].ConnectionString;
                    return config.ConnectionStrings.ConnectionStrings[options.ConnectionStringKey].ConnectionString;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get file config path
        /// </summary>
        /// <param name="workingdirectory"></param>
        /// <param name="configFileName"></param>
        /// <param name="configSource"></param>
        /// <returns></returns>
        private static string GetFileConfigPath(string workingdirectory, string configFileName, ref ConfigSourceEnum configSource)
        {
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
