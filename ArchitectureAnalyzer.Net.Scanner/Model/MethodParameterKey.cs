namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    public struct MethodParameterKey
    {
        public string Type { get; set; }

        public string Method { get; set; }

        public string Name { get; }

        public MethodParameterKey(string type, string method, string name)
        {
            Type = type;
            Method = method;
            Name = name;
        }
    }
}