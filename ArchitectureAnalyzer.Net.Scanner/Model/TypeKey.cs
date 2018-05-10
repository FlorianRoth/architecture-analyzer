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

        public static TypeKey FromTypeArgument(Type type, string typeArg)
        {
            return FromTypeArgument(FromType(type), typeArg);
        }

        public static TypeKey FromTypeArgument(TypeKey type, string typeArg)
        {
            return new TypeKey(type.Namespace, type.Name + "<" + typeArg + ">");
        }

        public static TypeKey FromMethodParameter(MethodKey key, string typeArg)
        {
            return new TypeKey(
                key.DeclaringType.Namespace,
                key.DeclaringType.Name + "/" + key.Name + "<" + typeArg + ">");
        }
    }
}