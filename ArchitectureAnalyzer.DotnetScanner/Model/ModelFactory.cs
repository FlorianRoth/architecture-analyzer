namespace ArchitectureAnalyzer.DotnetScanner.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.Net.Model;

    internal class ModelFactory : IModelFactory
    {
        private readonly ModelMap<AssemblyKey, NetAssembly> _assemblyMap;

        private readonly ModelMap<TypeKey, NetType> _typeMap;

        private readonly ModelMap<MethodKey, NetMethod> _methodMap;

        public ModelFactory()
        {
            _assemblyMap = new ModelMap<AssemblyKey, NetAssembly>();
            _typeMap = new ModelMap<TypeKey, NetType>();
            _methodMap = new ModelMap<MethodKey, NetMethod>();
        }

        public NetAssembly CreateAssemblyModel(AssemblyKey key)
        {
            var model =  _assemblyMap[key];
            model.Name = key.Name;

            return model;
        }

        public NetType CreateTypeModel(TypeKey key)
        {
            var model = _typeMap[key];
            model.Name = key.Name;
            model.Namespace = key.Namespace;

            return model;
        }
        
        public NetMethod CreateMethodModel(MethodKey key)
        {
            var model = _methodMap[key];
            model.Name = key.Name;

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
