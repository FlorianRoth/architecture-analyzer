
namespace ArchitectureAnalyzer.DotnetScanner.Scanner
{
    using System.Linq;
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.DotnetScanner.Model;

    using Microsoft.Extensions.Logging;

    internal class AssemblyScanner : AbstractScanner
    {
        public AssemblyScanner(
            MetadataReader reader,
            IModelFactory factory,
            IGraphDatabase database,
            ILogger logger)
            : base(reader, factory, database, logger)
        {
        }

        public NetAssembly Scan(AssemblyDefinition assembly)
        {
            var name = GetString(assembly.Name);

            var model = Factory.CreateAssemblyModel(name);
            model.Name = name;

            Database.CreateNode(model);

            model.References = Reader.AssemblyReferences
                .Select(Reader.GetAssemblyReference)
                .Select(CreateAssemblyModel)
                .ToList();
            
            var typeScanner = new TypeScanner(Reader, Factory, Database, Logger);

            foreach (var type in Reader.TypeDefinitions.Select(Reader.GetTypeDefinition))
            {
                var typeModel = typeScanner.ScanType(type);
                if (typeModel == null)
                {
                    continue;
                }
                
                model.DefinedTypes.Add(typeModel);
                Database.CreateRelationship(model, typeModel, Relationship.DEFINES_TYPE);
            }

            return model;
        }

        private NetAssembly CreateAssemblyModel(AssemblyReference assemblyReference)
        {
            return Factory.CreateAssemblyModel(GetString(assemblyReference.Name));
        }
    }
}
