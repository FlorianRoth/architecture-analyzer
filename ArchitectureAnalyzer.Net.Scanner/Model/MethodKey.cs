namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    public struct MethodKey
    {
        public string Type { get; }

        public string Name { get; }

        public int SignatureHash { get; }

        public MethodKey(string type, string name, int signatureHash)
        {
            Type = type;
            Name = name;
            SignatureHash = signatureHash;
        }
    }
}