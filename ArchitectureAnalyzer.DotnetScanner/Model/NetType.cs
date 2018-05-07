namespace ArchitectureAnalyzer.DotnetScanner.Model
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
            Enum
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public TypeClass Type { get; set; }

        public string Name { get; set; }

        public string Namespace { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsSealed { get; set; }

        public bool HasAttribute { get; set; }

        [JsonIgnore]
        public NetType BaseType { get; set; }

        [JsonIgnore]
        public IList<NetType> Implements { get; set; }

        [JsonIgnore]
        public IList<NetType> Attributes { get; set; }

        [JsonIgnore]
        public IList<NetMethod> Methods { get; set; }

        public NetType()
        {
            Implements = new List<NetType>();
            Attributes = new List<NetType>();
            Methods = new List<NetMethod>();
        }

        public override string ToString()
        {
            return $"NetType({Namespace}.{Name})";
        }

        public TypeKey GetKey()
        {
            return new TypeKey(Namespace, Name);
        }
    }
}
