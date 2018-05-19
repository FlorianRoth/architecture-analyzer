
namespace ArchitectureAnalyzer.Net.Model
{
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Core.Graph;
    
    public class NetAssembly : Node
    {
        public string Name { get; set; }

        [Ignore]
        public IList<NetAssembly> References { get; set; }

        [Ignore]
        public IList<NetType> DefinedTypes { get; set; }

        public NetAssembly()
        {
            References = new List<NetAssembly>();
            DefinedTypes = new List<NetType>();
        }
        
        public override string ToString()
        {
            return $"NetAssembly({Name})";
        }
    }
}
