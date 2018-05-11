namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    public struct MethodKey
    {
        public TypeKey DeclaringType { get; }

        public string Name { get; }

        public int SignatureHash { get; }

        public MethodKey(TypeKey declaringType, string name, int signatureHash)
        {
            DeclaringType = declaringType;
            Name = name;
            SignatureHash = signatureHash;
        }
    }
}