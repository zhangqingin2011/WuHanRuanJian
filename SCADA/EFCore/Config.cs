using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace SCADA
{
    /// <summary>
    /// Config-Customize
    /// </summary>
    public static class Config
    {
        private static IConfigurationRoot GetRoot(string fileName = "appsettings.json") => new ConfigurationBuilder().Add(new JsonConfigurationSource { Path = fileName, ReloadOnChange = true }).Build();

        public static void SetConfigFile(string fileName) => Configuration = GetRoot(fileName);

        public static IConfiguration Configuration { get; private set; } = GetRoot();

        public static string GetConnectionString(string name) => Configuration.GetConnectionString(name);

        /// <summary>
        /// Server端服务URL
        /// </summary>
        public static string ServerURL => Configuration[nameof(ServerURL)];
    }

}
