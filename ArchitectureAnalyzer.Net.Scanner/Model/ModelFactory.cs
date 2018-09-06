namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.Net.Model;

    using Mono.Cecil;

    internal class ModelFactory : IModelFactory
    {
        private readonly ModelMap<string, NetAssembly> _assemblyMap;

        private readonly ModelMap<string, NetType> _typeMap;

        private readonly ModelMap<MethodKey, NetMethod> _methodMap;

        private readonly ModelMap<MethodParameterKey, NetMethodParameter> _methodParameterMap;

        private readonly ModelMap<PropertyKey, NetProperty> _propertyMap;

        public ModelFactory()
        {
            _assemblyMap = new ModelMap<string, NetAssembly>();
            _typeMap = new ModelMap<string, NetType>();
            _methodMap = new ModelMap<MethodKey, NetMethod>();
            _methodParameterMap = new ModelMap<MethodParameterKey, NetMethodParameter>();
            _propertyMap = new ModelMap<PropertyKey, NetProperty>();
        }

        public NetAssembly CreateAssemblyModel(AssemblyDefinition assemblyDefinition)
        {
            var model =  _assemblyMap[assemblyDefinition.Name.Name];
            model.Name = assemblyDefinition.Name.Name;

            return model;
        }

        public NetAssembly CreateAssemblyModel(AssemblyNameReference key)
        {
            var model = _assemblyMap[key.Name];
            model.Name = key.Name;

            return model;
        }
        
        public NetType CreateTypeModel(TypeReference typeReference)
        {
            var model = _typeMap[typeReference.FullName];
            model.FullName = typeReference.FullName;
            model.Name = typeReference.Name;
            model.DisplayName = model.Name;
            model.Namespace = typeReference.Namespace;

            if (typeReference is GenericInstanceType genericInstanceType)
            {
                model.IsGenericTypeInstantiation = true;
                model.GenericType = CreateTypeModel(genericInstanceType.ElementType);
                model.GenericTypeInstantiationArgs = genericInstanceType.GenericArguments.Select(CreateTypeModel).ToList();
            }

            return model;
        }

        public NetMethod CreateMethodModel(MethodDefinition methodDefinition)
        {
            var key = new MethodKey(methodDefinition.DeclaringType.FullName, methodDefinition.Name, 0);

            var model = _methodMap[key];
            model.Name = methodDefinition.Name;

            return model;
        }

        public NetMethodParameter CreateMethodParameter(MethodDefinition method, ParameterDefinition param)
        {
            var key = new MethodParameterKey(method.DeclaringType.FullName, method.Name, param.Name);

            var model = _methodParameterMap[key];
            model.Name = param.Name;

            return model;
        }

        public NetType CreateGenericTypeArg(GenericParameter parameter)
        {
            var typeName = parameter.DeclaringType?.FullName ?? parameter.DeclaringMethod.DeclaringType.Name;

            var key = $"{typeName}/{parameter.DeclaringMethod?.Name}/{parameter.Name}";

            var model = _typeMap[key];
            model.Name = parameter.Name;
            model.Namespace = string.Empty;
            model.FullName = key;
            model.DisplayName = parameter.Name;
            model.Type = NetType.TypeClass.GenericTypeArg;

            return model;
        }

        public NetProperty CreatePropertyModel(PropertyDefinition propertyDefinition)
        {
            var key = new PropertyKey(propertyDefinition.DeclaringType.FullName, propertyDefinition.Name);

            var model = _propertyMap[key];
            model.Name = propertyDefinition.Name;

            return model;
        }

        public IEnumerable<NetAssembly> GetAssemblyModels()
        {
            return _assemblyMap.Models;
        }

        public IEnumerable<NetType> GetTypeModels()
        {
            return _typeMap.Models;
        }

        public IEnumerable<NetMethod> GetMethodModels()
        {
            return _methodMap.Models;
        }

        public IEnumerable<NetMethodParameter> GetMethodParameterModels()
        {
            return _methodParameterMap.Models;
        }

        public IEnumerable<NetProperty> GetPropertyModels()
        {
            return _propertyMap.Models;
        }

        private class ModelMap<TKey, TNode> where TNode : Node, new()
        {
            // ReSharper disable once StaticMemberInGenericType
            private static long _nextId;

            private readonly IDictionary<TKey, TNode> _nodeMap;

            public IEnumerable<TNode> Models => _nodeMap.Values;

            public TNode this[TKey key]
            {
                get
                {
                    if (_nodeMap.TryGetValue(key, out var node) == false)
                    {
                        node = new TNode { Id = _nextId++ };
                        _nodeMap.Add(key, node);
                    }

                    return node;
                }
            }

            public ModelMap()
            {
                _nodeMap = new Dictionary<TKey, TNode>();
            }
        }
    }
}
