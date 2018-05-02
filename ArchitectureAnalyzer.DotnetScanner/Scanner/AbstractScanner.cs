namespace ArchitectureAnalyzer.DotnetScanner.Scanner
{
    using System.Linq;
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.DotnetScanner.Model;

    using Microsoft.Extensions.Logging;

    internal abstract class AbstractScanner
    {
        protected MetadataReader Reader { get; }

        protected IModelFactory Factory { get; }

        protected IGraphDatabase Database { get; }

        protected ILogger Logger { get; }

        protected AbstractScanner(MetadataReader reader, IModelFactory factory, IGraphDatabase database, ILogger logger)
        {
            Reader = reader;
            Factory = factory;
            Database = database;
            Logger = logger;
        }

        protected string GetTypeId(TypeDefinition typeDef)
        {
            return CreateId(typeDef.Namespace, typeDef.Name);
        }

        protected string GetTypeId(TypeReference typeRef)
        {
            return CreateId(typeRef.Namespace, typeRef.Name);
        }

        protected string CreateId(params StringHandle[] parts)
        {
            return string.Join(".", parts.Select(GetString).Where(s => !string.IsNullOrEmpty(s)));
        }

        protected string GetString(StringHandle handle)
        {
            return handle.IsNil ? null : Reader.GetString(handle);
        }
    }
}
