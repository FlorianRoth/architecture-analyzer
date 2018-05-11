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
        public int Address { get; set; }

        [JsonIgnore]
        public NetType DeclaringType { get; set; }

        [JsonIgnore]
        public NetType ReturnType { get; set; }

        [JsonIgnore]
        public IReadOnlyList<NetMethodParameter> Parameters { get; set; }

        [JsonIgnore]
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
