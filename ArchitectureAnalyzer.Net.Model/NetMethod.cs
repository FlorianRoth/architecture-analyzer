namespace ArchitectureAnalyzer.Net.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Core.Graph;
    
    public class NetMethod : Node, IGenericContext
    {
        public string Name { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsSealed { get; set; }

        public bool IsGeneric { get; set; }
        
        public Visibility Visibility { get; set; }
        
        [Ignore]
        public NetType DeclaringType { get; set; }

        [Ignore]
        public NetType ReturnType { get; set; }

        [Ignore]
        public IReadOnlyList<NetMethodParameter> Parameters { get; set; }

        [Ignore]
        public IReadOnlyList<NetType> GenericParameters { get; set; }
        
        public NetMethod()
        {
            Parameters = new List<NetMethodParameter>();
            GenericParameters = new List<NetType>();
        }
        
        public override string ToString()
        {
            return $"NetMethod({Name})";
        }
    }
}
