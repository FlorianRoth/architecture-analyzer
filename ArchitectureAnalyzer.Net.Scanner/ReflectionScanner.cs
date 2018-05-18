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

            CreateAllNodes(scannedAssemblies);

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

        private void CreateAllNodes(IEnumerable<NetAssembly> scannedAssemblies)
        {
            _logger.LogInformation("Adding nodes to database");

            CreateNodes(scannedAssemblies);
            CreateNodes(_factory.GetTypeModels());
            CreateNodes(_factory.GetMethodModels());
            CreateNodes(_factory.GetMethodParameterModels());
            CreateNodes(_factory.GetPropertyModels());
        }

        private void CreateNodes<TNode>(IEnumerable<TNode> nodes)
            where TNode : Node
        {
            _logger.LogInformation("  Creating {0} nodes", typeof(TNode).Name);
            foreach (var model in nodes)
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
                ConnectProperties(type);
                ConnectAttributes(type);
                ConnectGenericTypeArgs(type);
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

            ConnectMethodParameters(method);
            ConnectGenericMethodParameters(method);
        }
        
        private void ConnectMethodParameters(NetMethod method)
        {
            foreach (var param in method.Parameters)
            {
                _db.CreateRelationship(
                    method,
                    param,
                    Relationship.DEFINES_PARAMETER);

                _db.CreateRelationship(
                    param,
                    param.Type,
                    Relationship.HAS_TYPE);
            }
        }

        private void ConnectGenericMethodParameters(NetMethod method)
        {
            foreach (var param in method.GenericParameters)
            {
                _db.CreateRelationship(method, param, Relationship.DEFINES_GENERIC_METHOD_ARG);
            }
        }

        private void ConnectGenericTypeArgs(NetType type)
        {
            foreach (var arg in type.GenericTypeArgs)
            {
                _db.CreateRelationship(type, arg, Relationship.DEFINES_GENERIC_TYPE_ARG);
            }
        }
        
        private void ConnectProperties(NetType type)
        {
            foreach (var property in type.Properties)
            {
                ConnectProperty(type, property);
            }
        }

        private void ConnectProperty(NetType type, NetProperty property)
        {
            _db.CreateRelationship(type, property, Relationship.DEFINES_PROPERTY);
            _db.CreateRelationship(property, property.Type, Relationship.HAS_TYPE);
        }
    }
}
