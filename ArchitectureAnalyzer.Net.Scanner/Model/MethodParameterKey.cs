namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    public struct MethodParameterKey
    {
        public long MethodId { get; set; }

        public string Name { get; }

        public MethodParameterKey(long methodId, string name)
        {
            MethodId = methodId;
            Name = name;
        }
    }
}