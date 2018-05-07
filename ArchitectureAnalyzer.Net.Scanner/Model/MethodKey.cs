namespace ArchitectureAnalyzer.Net.Scanner.Model
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