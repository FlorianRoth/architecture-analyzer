
namespace ArchitectureAnalyzer.Net.Scanner
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;

    using Microsoft.Extensions.Logging;

    internal class GraphBuilder
    {
        private readonly IModelFactory _factory;

        private readonly IGraphDatabaseTransaction _tx;

        private readonly ILogger _logger;

        public GraphBuilder(
            IModelFactory factory,
            IGraphDatabaseTransaction tx,
            ILogger logger)
        {
            _factory = factory;
            _tx = tx;
            _logger = logger;
        }

        public void Build(IReadOnlyList<NetAssembly> scannedAssemblies)
        {
            ClearDatabase();

            CreateAllNodes(scannedAssemblies);

            foreach (var assemblyModel in scannedAssemblies)
            {
                ConnectAssemblyReferences(assemblyModel);
            }

            ConnectTypes();
        }

        private void ClearDatabase()
        {
            _logger.LogInformation("Clearing database");

            _tx.Clear();
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
                _tx.CreateNode(model);
            }
        }

        private void ConnectAssemblyReferences(NetAssembly assembly)
        {
            _logger.LogInformation("Connecting assembly references for '{0}'", assembly.Name);

            foreach (var reference in assembly.References)
            {
                _tx.CreateRelationship(assembly, reference, Relationship.DEPENDS_ON);
            }
        }

        private void ConnectTypes()
        {
            _logger.LogInformation("Connecting types");

            foreach (var type in _factory.GetTypeModels())
            {
                ConnectTypeDefinition(type);
                ConnectBaseType(type);
                ConnectInterfaceImplementations(type);
                ConnectMethods(type);
                ConnectProperties(type);
                ConnectAttributes(type);
                ConnectGenericTypeArgs(type);
                ConnectGenericTypeInstantiation(type);
            }
        }

        private void ConnectTypeDefinition(NetType type)
        {
            var assembly = type.Assembly;
            if (assembly == null)
            {
                return;
            }

            _tx.CreateRelationship(assembly, type, Relationship.DEFINES_TYPE);
        }

        private void ConnectBaseType(NetType type)
        {
            if (type.BaseType != null)
            {
                _tx.CreateRelationship(type, type.BaseType, Relationship.EXTENDS);
            }
        }

        private void ConnectInterfaceImplementations(NetType type)
        {
            var baseTypeInterfaces = type.BaseType?.Implements ?? Enumerable.Empty<NetType>();
            var interfacesFromBaseInterfaces = baseTypeInterfaces.Concat(type.Implements.SelectMany(t => t.Implements)).ToList();

            foreach (var interfaceType in type.Implements.Except(interfacesFromBaseInterfaces))
            {
                _tx.CreateRelationship(type, interfaceType, Relationship.IMPLEMENTS);
            }
        }

        private void ConnectAttributes(NetType type)
        {
            foreach (var attributeType in type.Attributes)
            {
                _tx.CreateRelationship(type, attributeType, Relationship.HAS_ATTRIBUTE);
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
            _tx.CreateRelationship(type, method, Relationship.DEFINES_METHOD);
            _tx.CreateRelationship(method, method.ReturnType, Relationship.RETURNS);

            ConnectMethodParameters(method);
            ConnectGenericMethodParameters(method);
        }

        private void ConnectMethodParameters(NetMethod method)
        {
            foreach (var param in method.Parameters)
            {
                _tx.CreateRelationship(
                    method,
                    param,
                    Relationship.DEFINES_PARAMETER);

                _tx.CreateRelationship(
                    param,
                    param.Type,
                    Relationship.HAS_TYPE);
            }
        }

        private void ConnectGenericMethodParameters(NetMethod method)
        {
            foreach (var param in method.GenericParameters)
            {
                _tx.CreateRelationship(method, param, Relationship.DEFINES_GENERIC_METHOD_ARG);
            }
        }

        private void ConnectGenericTypeArgs(NetType type)
        {
            foreach (var arg in type.GenericTypeArgs)
            {
                _tx.CreateRelationship(type, arg, Relationship.DEFINES_GENERIC_TYPE_ARG);
            }
        }

        private void ConnectGenericTypeInstantiation(NetType type)
        {
            if (!type.IsGenericTypeInstantiation)
            {
                return;
            }

            _tx.CreateRelationship(type, type.GenericType, Relationship.INSTANTIATES_GENERIC_TYPE);

            foreach (var arg in type.GenericTypeInstantiationArgs)
            {
                _tx.CreateRelationship(type, arg, Relationship.HAS_TYPE_ARGUMENT);
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
            _tx.CreateRelationship(type, property, Relationship.DEFINES_PROPERTY);
            _tx.CreateRelationship(property, property.Type, Relationship.HAS_TYPE);
        }
    }
}
