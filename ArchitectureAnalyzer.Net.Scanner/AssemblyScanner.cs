
namespace ArchitectureAnalyzer.Net.Scanner
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;
    using ArchitectureAnalyzer.Net.Scanner.Utils;

    using Microsoft.Extensions.Logging;

    internal class AssemblyScanner : AbstractScanner
    {
        public AssemblyScanner(
            MetadataReader reader,
            IModelFactory factory,
            ILogger logger)
            : base(reader, factory, logger)
        {
        }

        public NetAssembly Scan(AssemblyDefinition assembly)
        {
            var name = assembly.Name.GetString(Reader);

            var assemblyModel = Factory.CreateAssemblyModel(AssemblyKey(name));
            
            assemblyModel.References = CreateAssemblyReferences();
            assemblyModel.DefinedTypes = CreateTypes(assemblyModel);
            
            return assemblyModel;
        }

        private List<NetAssembly> CreateAssemblyReferences()
        {
            return Reader.AssemblyReferences
                .Select(Reader.GetAssemblyReference)
                .Select(CreateAssemblyModel)
                .ToList();
        }

        private NetAssembly CreateAssemblyModel(AssemblyReference assemblyReference)
        {
            var key = AssemblyKey(assemblyReference.Name.GetString(Reader));
            return Factory.CreateAssemblyModel(key);
        }

        private IList<NetType> CreateTypes(NetAssembly assembly)
        {
            var typeScanner = new TypeScanner(Reader, Factory, Logger);

            return Reader.TypeDefinitions
                .Select(Reader.GetTypeDefinition)
                .Where(IncludeType)
                .Select(type => typeScanner.ScanType(type, assembly))
                .ToList();
        }

        private bool IncludeType(TypeDefinition type)
        {
            if (type.Attributes.HasFlag(TypeAttributes.SpecialName))
            {
                return false;
            }

            var name = type.Name.GetString(Reader);
            if (name.StartsWith("<"))
            {
                return false;
            }

            return true;
        }
    }
}
