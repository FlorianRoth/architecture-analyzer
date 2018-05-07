
namespace ArchitectureAnalyzer
{
    using System;

    using ArchitectureAnalyzer.Core;
    using ArchitectureAnalyzer.DotnetScanner;
    using ArchitectureAnalyzer.Neo4j;
    using ArchitectureAnalyzer.Neo4j.Graph;
    using ArchitectureAnalyzer.Net.Scanner;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;

    internal class ServiceProviderSetup
    {
        private readonly IConfiguration _config;

        public ServiceProviderSetup(IConfiguration config)
        {
            _config = config;
        }

        public IServiceProvider ConfigureServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCoreServices();
            serviceCollection.AddDotnetScanner(ConfigureScanner);
            serviceCollection.AddNeo4J(ConfigureDatabase);
            serviceCollection.AddLogging(ConfigureLogging);

            return serviceCollection.BuildServiceProvider();
        }

        private void ConfigureDatabase(Neo4JConfiguration config)
        {
            _config.GetSection("Neo4j").Bind(config);
        }

        private void ConfigureScanner(ReflectionScannerConfiguration config)
        {
            _config.GetSection("Scanner:Dotnet").Bind(config);
        }

        private static void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddConsole(ConfigureConsoleLogger);
            builder.AddDebug();
        }

        private static void ConfigureConsoleLogger(ConsoleLoggerOptions options)
        {
            options.IncludeScopes = false;
        }

    }
}
