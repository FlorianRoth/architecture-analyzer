namespace ArchitectureAnalyzer.Net.Model
{
    using ArchitectureAnalyzer.Core.Graph;

    using Newtonsoft.Json;

    public class NetMethodParameter : Node
    {
        public string Name { get; set; }

        public int Order { get; set; }

        [JsonIgnore]
        public NetType Type { get; set; }

        [JsonIgnore]
        public NetMethod DeclaringMethod { get; set; }

        public override string ToString()
        {
            return $"NetMethodParameter({Name})";
        }
    }
}