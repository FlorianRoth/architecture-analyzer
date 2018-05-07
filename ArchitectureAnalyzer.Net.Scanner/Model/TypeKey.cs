namespace ArchitectureAnalyzer.Net.Scanner.Model
{
    using System;

    public struct TypeKey
    {
        public static readonly TypeKey Undefined = default(TypeKey);


        public string Namespace { get; }

        public string Name { get; }

        public TypeKey(string @namespace, string name) : this()
        {
            Namespace = @namespace;
            Name = name;
        }
        
        public static TypeKey FromType<T>()
        {
            return FromType(typeof(T));
        }

        public static TypeKey FromType(Type type)
        {
            return new TypeKey(type.Namespace, type.Name);
        }
    }
}