namespace ArchitectureAnalyzer.DotnetScanner.Model
{
    public struct MethodKey
    {
        public string Name { get; }

        public MethodKey(string name)
        {
            Name = name;
        }
    }
}