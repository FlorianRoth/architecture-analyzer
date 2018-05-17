namespace ArchitectureAnalyzer.Net.Scanner
{
    using ArchitectureAnalyzer.Net.Scanner.Model;

    public struct PropertyKey
    {
        public TypeKey Type { get; }

        public string Name { get; }

        public PropertyKey(TypeKey type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}