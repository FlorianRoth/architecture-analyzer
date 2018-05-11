namespace ArchitectureAnalyzer.Net.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Core.Graph;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

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

        [JsonConverter(typeof(StringEnumConverter))]
        public TypeClass Type { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Namespace { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsSealed { get; set; }
        
        public bool IsGeneric { get; set; }
        
        public bool HasAttribute { get; set; }

        [JsonIgnore]
        public NetType BaseType { get; set; }

        [JsonIgnore]
        public IList<NetType> Implements { get; set; }

        [JsonIgnore]
        public IList<NetType> Attributes { get; set; }

        [JsonIgnore]
        public IList<NetMethod> Methods { get; set; }

        [JsonIgnore]
        public IList<NetType> GenericTypeArgs { get; set; }

        public NetType()
        {
            Type = TypeClass.External;
            Implements = new List<NetType>();
            Attributes = new List<NetType>();
            Methods = new List<NetMethod>();
            GenericTypeArgs = new List<NetType>();
        }

        public override string ToString()
        {
            return $"NetType({Namespace}.{Name})";
        }
    }
}
