namespace ArchitectureAnalyzer.DotnetScanner.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Core.Graph;

    internal class ModelFactory : IModelFactory
    {
        private readonly ModelMap<NetAssembly> _assemblyMap;

        private readonly ModelMap<NetType> _typeMap;

        private readonly ModelMap<NetMethod> _methodMap;

        public ModelFactory()
        {
            _assemblyMap = new ModelMap<NetAssembly>();
            _typeMap = new ModelMap<NetType>();
            _methodMap = new ModelMap<NetMethod>();
        }

        public NetAssembly CreateAssemblyModel(string id)
        {
            return _assemblyMap.Get(id);
        }

        public NetType CreateTypeModel(string id)
        {
            return _typeMap.Get(id);
        }
        
        public NetMethod CreateMethodModel(string id)
        {
            return _methodMap.Get(id);
        }

        private class ModelMap<TNode> where TNode : Node, new()
        {
            private readonly IDictionary<string, TNode> _nodeMap;

            public ModelMap()
            {
                _nodeMap = new Dictionary<string, TNode>();
            }

            public TNode Get(string id)
            {
                if (_nodeMap.TryGetValue(id, out var node) == false)
                {
                    node = new TNode {Id = id };
                    _nodeMap.Add(id, node);
                }

                return node;
            }
        }
    }
}
