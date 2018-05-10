namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    public struct MethodKey
    {
        public TypeKey DeclaringType { get; }

        public string Name { get; }

        public MethodKey(TypeKey declaringType, string name)
        {
            DeclaringType = declaringType;
            Name = name;
        }
    }
}