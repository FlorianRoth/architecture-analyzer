
namespace ArchitectureAnalyzer.DotnetScanner.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using ArchitectureAnalyzer.Net.Model;

    public static class TypeKeyExtensions
    {
        public static TypeKey GetKey(this NetType type)
        {
            return new TypeKey(type.Namespace, type.Name);
        }

        public static TypeKey ToArrayType(this TypeKey key)
        {
            return new TypeKey(key.Namespace, key.Name + "[]");
        }

        public static TypeKey ToPointerType(this TypeKey key)
        {
            return new TypeKey(key.Namespace, key.Name + "*");
        }

        public static TypeKey ToReferenceType(this TypeKey key)
        {
            return new TypeKey(key.Namespace, key.Name + "&");
        }

        public static TypeKey ToPinnedType(this TypeKey key)
        {
            return new TypeKey(key.Namespace, key.Name + " pinned");
        }
        
        public static TypeKey ToModifiedType(this TypeKey key, TypeKey modifier)
        {
            return new TypeKey(key.Namespace, $"{modifier.Namespace}.{modifier.Name} {key.Name}");
        }

        public static TypeKey ToGenericType(this TypeKey key, IEnumerable<TypeKey> typeArguments)
        {
            var name = key.Name + "<" + string.Join(",", typeArguments.Select(a => a.Namespace + "." + a.Name)) + ">";

            return new TypeKey(key.Namespace, name);
        }
    }
}
