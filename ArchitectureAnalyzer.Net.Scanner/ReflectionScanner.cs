namespace ArchitectureAnalyzer.Net.Scanner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.Core.Scanner;
    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;

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

            var scannedAssemblies = ScanAssemblies();

            CreateNodes(scannedAssemblies);

            foreach (var assemblyModel in scannedAssemblies)
            {
                ConnectAssemblyReferences(assemblyModel);
                ConnectTypes(assemblyModel);
            }
            
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
                using (var peFile = new PEReader(stream))
                {
                    var metadataReader = peFile.GetMetadataReader();
                    var scanner = new AssemblyScanner(
                        metadataReader,
                        _factory,
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

        private void CreateNodes(IEnumerable<NetAssembly> scannedAssemblies)
        {
            _logger.LogInformation("Adding nodes to database");

            _logger.LogInformation("  Creating assembly nodes");
            foreach (var model in scannedAssemblies)
            {
                _db.CreateNode(model);
            }

            _logger.LogInformation("  Creating type nodes");
            foreach (var model in _factory.GetTypeModels())
            {
                _db.CreateNode(model);
            }

            _logger.LogInformation("  Creating method nodes");
            foreach (var model in _factory.GetMethodModels())
            {
                _db.CreateNode(model);
            }
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
                ConnectTypeDefinitions(assembly, type);
                ConnectBaseType(type);
                ConnectInterfaceImplementations(type);
                ConnectMethods(type);
                ConnectAttributes(type);
            }
        }

        private void ConnectTypeDefinitions(NetAssembly assembly, NetType type)
        {
            _db.CreateRelationship(assembly, type, Relationship.DEFINES_TYPE);
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

        private void ConnectAttributes(NetType type)
        {
            foreach (var attributeType in type.Attributes)
            {
                _db.CreateRelationship(type, attributeType, Relationship.HAS_ATTRIBUTE);
            }
        }

        private void ConnectMethods(NetType type)
        {
            foreach (var method in type.Methods)
            {
                ConnectMethod(type, method);
            }
        }

        private void ConnectMethod(NetType type, NetMethod method)
        {
            _db.CreateRelationship(type, method, Relationship.DEFINES_METHOD);
            _db.CreateRelationship(method, method.ReturnType, Relationship.RETURNS);

            var order = 0;
            foreach (var param in method.ParameterTypes)
            {
                var rel = new HasParameterRelationship { Order = order++ };
                _db.CreateRelationship(method, param, Relationship.HAS_PARAMETER, rel);
            }
        }
    }
}
