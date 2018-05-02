namespace ArchitectureAnalyzer.DotnetScanner.Model
{
    using ArchitectureAnalyzer.Core.Graph;

    public class NetMethod : Node
    {
        public string Name { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsSealed { get; set; }

        public NetMethod(string id)
            : base(id)
        {
        }
    }
}
