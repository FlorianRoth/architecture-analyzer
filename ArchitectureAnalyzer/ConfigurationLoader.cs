
namespace ArchitectureAnalyzer
{
    using System.IO;

    using Microsoft.Extensions.Configuration;

    internal static class ConfigurationLoader
    {
        public static IConfiguration LoadConfig(string configFile)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFile);

            return builder.Build();
        }
    }
}
