
namespace ArchitectureAnalyzer.Net.Scanner.Utils
{
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.Net.Scanner.Model;

    internal static class MetadataExtensions
    {
        public static TypeKey GetTypeKey(this TypeDefinitionHandle typeDef, MetadataReader reader)
        {
            if (typeDef.IsNil)
            {
                return TypeKey.Undefined;
            }
            
            return GetTypeKey(reader.GetTypeDefinition(typeDef), reader);
        }

        public static TypeKey GetTypeKey(this TypeDefinition typeDef, MetadataReader reader)
        {
            return new TypeKey(typeDef.Namespace.GetString(reader), typeDef.Name.GetString(reader));
        }

        public static TypeKey GetTypeKey(this TypeReferenceHandle typeRef, MetadataReader reader)
        {
            if (typeRef.IsNil)
            {
                return TypeKey.Undefined;
            }

            return GetTypeKey(reader.GetTypeReference(typeRef), reader);
        }

        public static TypeKey GetTypeKey(this TypeReference typeRef, MetadataReader reader)
        {
            return new TypeKey(typeRef.Namespace.GetString(reader), typeRef.Name.GetString(reader));
        }
        
        public static string GetString(this StringHandle handle, MetadataReader reader)
        {
            return handle.IsNil ? null : reader.GetString(handle);
        }
    }
}
