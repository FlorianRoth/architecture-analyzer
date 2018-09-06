namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    public struct PropertyKey
    {
        public string Type { get; }

        public string Name { get; }

        public PropertyKey(string type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}