namespace ArchitectureAnalyzer.Net.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Core.Graph;

    using Newtonsoft.Json;

    public class NetMethod : Node
    {
        public string Name { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsSealed { get; set; }

        public bool IsGeneric { get; set; }

        [JsonIgnore]
        public NetType ReturnType { get; set; }

        [JsonIgnore]
        public IReadOnlyList<NetType> ParameterTypes { get; set; }

        [JsonIgnore]
        public IReadOnlyList<NetType> GenericParameters { get; set; }

        public NetMethod()
        {
            GenericParameters = new List<NetType>();
        }

        public override string ToString()
        {
            return $"NetMethod({Name})";
        }
    }
}
