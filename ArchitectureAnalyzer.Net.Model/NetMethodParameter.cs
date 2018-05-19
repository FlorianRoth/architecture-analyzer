namespace ArchitectureAnalyzer.Net.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Core.Graph;
    
    public class NetMethodParameter : Node
    {
        public string Name { get; set; }

        public int Order { get; set; }

        [Ignore]
        public NetType Type { get; set; }

        [Ignore]
        public NetMethod DeclaringMethod { get; set; }

        public override string ToString()
        {
            return $"NetMethodParameter({Name})";
        }
    }
}