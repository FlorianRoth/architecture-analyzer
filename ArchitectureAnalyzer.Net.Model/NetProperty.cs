
namespace ArchitectureAnalyzer.Net.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Core.Graph;

    using Newtonsoft.Json;

    public class NetProperty : Node, IGenericContext
    {
        private static readonly IReadOnlyList<NetType> NoGenericParameters = new NetType[0];

        public string Name { get; set; }

        [JsonIgnore]
        public NetType Type { get; set; }

        [JsonIgnore]
        public NetType DeclaringType { get; set; }

        [JsonIgnore]
        public IReadOnlyList<NetType> GenericParameters => NoGenericParameters;
    }
}
