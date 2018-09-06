namespace ArchitectureAnalyzer.Net.Scanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.Core.Scanner;
    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;

    using Microsoft.Extensions.Logging;

    using Mono.Cecil;

    internal class ReflectionScanner : IScanner
    {
        private readonly IEnumerable<string> _assemblies;

        private readonly IModelFactory _factory;

        private readonly IGraphDatabase _db;

        private readonly ILogger<ReflectionScanner> _logger;

        public ReflectionScanner(
            ReflectionScannerConfiguration config,
            IModelFactory factory,
            IGraphDatabase db,
            ILogger<ReflectionScanner> logger)
        {
            _assemblies = config.Assemblies.ToList();
            _factory = factory;
            _db = db;
            _logger = logger;
        }
        
        public void Scan()
        {
            _logger.LogInformation("Starting scan");

            var scannedAssemblies = ScanAssemblies();

            FillDatabase(scannedAssemblies);
            
            _logger.LogInformation("Scan complete");
        }

        private IReadOnlyList<NetAssembly> ScanAssemblies()
        {
            return _assemblies
                .Select(ScanAssembly)
                .ToList();
        }

        private NetAssembly ScanAssembly(string assemblyPath)
        {
            _logger.LogInformation("Scanning assembly '{0}'", assemblyPath);

            try
            {
                using (var stream = File.OpenRead(assemblyPath))
                using (var moduleDefinition = ModuleDefinition.ReadModule(stream))
                {
                    var scanner = new AssemblyScanner(
                        moduleDefinition,
                        _factory,
                        _logger);
                    
                    return scanner.Scan(moduleDefinition.Assembly);
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Failed to load assembly '{0}'", assemblyPath);
                throw;
            }
            catch (BadImageFormatException ex)
            {
                _logger.LogError(ex, "Failed to load assembly '{0}'", assemblyPath);
                throw;
            }
        }

        private void FillDatabase(IReadOnlyList<NetAssembly> scannedAssemblies)
        {
            using (var tx = _db.BeginTransaction())
            {
                var connector = new GraphBuilder(_factory, tx, _logger);
                connector.Build(scannedAssemblies);

                tx.Commit();
            }
        }
    }
}
