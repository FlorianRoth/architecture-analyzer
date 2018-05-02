namespace ArchitectureAnalyzer
{
    using System;
    using System.Reflection;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.Core.Scanner;

    using Microsoft.Extensions.CommandLineUtils;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class Program
    {
        private const string HELP = "-?|-h|--help";

        private const int EXIT_OK = 0;

        private const int EXIT_ERROR = 1;

        public static void Main(string[] args)
        {
            try
            {
                var app = new CommandLineApplication();
                app.Name = Assembly.GetEntryAssembly().GetName().Name;
                app.Description = "Architecture Analyzer 0.1 alpha";
                app.HelpOption(HELP);

                app.OnExecute(() => 0);

                var configFileOption = app.Option(
                    "-c|--config <file>",
                    "Configuration file",
                    CommandOptionType.SingleValue);

                app.Command(
                    "scan",
                    command =>
                        {
                            command.Description = "Run the scanner to fill the database.";
                            command.OnExecute(() => Scan(configFileOption));
                        });

                var exitCode = app.Execute(args);

                Environment.ExitCode = exitCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.ExitCode = EXIT_ERROR;
            }
        }

        private static IServiceProvider CreateServiceProvider(CommandOption configFileOption)
        {
            var configFile = configFileOption.HasValue() ? configFileOption.Value() : "appconfig.json";

            var config = ConfigurationLoader.LoadConfig(configFile);
            var serviceProvider = new ServiceProviderSetup(config).ConfigureServiceProvider();

            return serviceProvider;
        }

        private static int Scan(CommandOption configFileOption)
        {
            var serviceProvider = CreateServiceProvider(configFileOption);

            var database = serviceProvider.GetService<IGraphDatabase>();
            var scanner = serviceProvider.GetService<IScanner>();

            try
            {
                database.Connect();
                database.Clear();

                scanner.Scan();

                return EXIT_OK;
            }
            catch (Exception ex)
            {
                serviceProvider.GetService<ILogger>().LogError(ex, ex.Message);
                return EXIT_ERROR;
            }
            finally
            {
                database.Disconnect();
            }
        }
    }
}
