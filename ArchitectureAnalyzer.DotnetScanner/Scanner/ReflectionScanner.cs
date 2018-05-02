namespace ArchitectureAnalyzer.DotnetScanner.Scanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.Core.Scanner;
    using ArchitectureAnalyzer.DotnetScanner.Model;

    using Microsoft.Extensions.Logging;

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

            var allAssemblies = _assemblies.Select(ScanAssembly).ToList();

            foreach (var assembly in allAssemblies)
            {
                ConnectAssemblyReferences(assembly);
                ConnectTypes(assembly);
            }
            
            _logger.LogInformation("Scan complete");
        }

        private void ConnectAssemblyReferences(NetAssembly assembly)
        {
            _logger.LogInformation("Connecting assembly references for '{0}'", assembly.Name);

            foreach (var reference in assembly.References)
            {
                _db.CreateRelationship(assembly, reference, Relationship.DEPENDS_ON);
            }
        }

        private void ConnectTypes(NetAssembly assembly)
        {
            _logger.LogInformation("Connecting types for assembly '{0}'", assembly.Name);

            foreach (var type in assembly.DefinedTypes)
            {
                ConnectBaseType(type);
                ConnectInterfaceImplementations(type);
            }
        }
        
        private void ConnectBaseType(NetType type)
        {
            if (type.BaseType != null)
            {
                _db.CreateRelationship(type, type.BaseType, Relationship.EXTENDS);
            }
        }

        private void ConnectInterfaceImplementations(NetType type)
        {
            var baseTypeInterfaces = type.BaseType?.Implements ?? Enumerable.Empty<NetType>();
            var interfacesFromBaseInterfaces = baseTypeInterfaces.Concat(type.Implements.SelectMany(t => t.Implements)).ToList();

            foreach (var interfaceType in type.Implements.Except(interfacesFromBaseInterfaces))
            {
                _db.CreateRelationship(type, interfaceType, Relationship.IMPLEMENTS);
            }
        }

        private NetAssembly ScanAssembly(string assemblyPath)
        {
            _logger.LogInformation("Scanning assembly '{0}'", assemblyPath);

            try
            {
                using (var stream = File.OpenRead(assemblyPath))
                using (var peFile = new PEReader(stream))
                {
                    var metadataReader = peFile.GetMetadataReader();
                    var scanner = new AssemblyScanner(
                        metadataReader,
                        _factory,
                        _db,
                        _logger);

                    var assembly = metadataReader.GetAssemblyDefinition();
                    return scanner.Scan(assembly);
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
    }
}
