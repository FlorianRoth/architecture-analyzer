namespace ArchitectureAnalyzer.Net.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using ArchitectureAnalyzer.Core.Graph;
    
    public class NetType : Node
    {
        public enum TypeClass
        {
            External,
            Class,
            Interface,
            Enum,
            GenericTypeArg
        }
        
        public TypeClass Type { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Namespace { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsSealed { get; set; }

        public bool IsGeneric => GenericTypeArgs.Any();

        public bool HasAttributes => Attributes.Any();
        
        public Visibility Visibility { get; set; }

        [Ignore]
        public NetType BaseType { get; set; }

        [Ignore]
        public IList<NetType> Implements { get; set; }

        [Ignore]
        public IList<NetType> Attributes { get; set; }

        [Ignore]
        public IList<NetMethod> Methods { get; set; }

        [Ignore]
        public IList<NetProperty> Properties { get; set; }

        [Ignore]
        public IList<NetType> GenericTypeArgs { get; set; }

        public NetType()
        {
            Type = TypeClass.External;
            Implements = new List<NetType>();
            Attributes = new List<NetType>();
            Methods = new List<NetMethod>();
            Properties = new List<NetProperty>();
            GenericTypeArgs = new List<NetType>();
        }
        
        public override string ToString()
        {
            return $"NetType({Namespace}.{Name})";
        }
    }
}
