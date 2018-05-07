
namespace ArchitectureAnalyzer.DotnetScanner.Utils
{
    using System.Linq;
    using System.Reflection.Metadata;

    internal static class MetadataExtensions
    {
        public static string GetTypeId(this TypeDefinitionHandle typeDef, MetadataReader reader)
        {
            if (typeDef.IsNil)
            {
                return null;
            }
            
            return GetTypeId(reader.GetTypeDefinition(typeDef), reader);
        }

        public static string GetTypeId(this TypeDefinition typeDef, MetadataReader reader)
        {
            return CreateId(reader, typeDef.Namespace, typeDef.Name);
        }

        public static string GetTypeId(this TypeReferenceHandle typeRef, MetadataReader reader)
        {
            if (typeRef.IsNil)
            {
                return null;
            }

            return GetTypeId(reader.GetTypeReference(typeRef), reader);
        }

        public static string GetTypeId(this TypeReference typeRef, MetadataReader reader)
        {
            return CreateId(reader, typeRef.Namespace, typeRef.Name);
        }

        private static string CreateId(MetadataReader reader, params StringHandle[] parts)
        {
            return string.Join(".", parts.Select(h => GetString(h, reader)).Where(s => !string.IsNullOrEmpty(s)));
        }

        public static string GetString(this StringHandle handle, MetadataReader reader)
        {
            return handle.IsNil ? null : reader.GetString(handle);
        }
    }
}
