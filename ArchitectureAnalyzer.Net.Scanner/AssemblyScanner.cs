
namespace ArchitectureAnalyzer.Net.Scanner
{
    using System.Collections.Generic;
    using System.Linq;

    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;

    using Microsoft.Extensions.Logging;

    using Mono.Cecil;
    
    internal class AssemblyScanner : AbstractScanner
    {
        public AssemblyScanner(
            ModuleDefinition module,
            IModelFactory factory,
            ILogger logger)
            : base(module, factory, logger)
        {
        }

        public NetAssembly Scan(AssemblyDefinition assembly)
        {
            var assemblyModel = Factory.CreateAssemblyModel(assembly);
            assemblyModel.References = CreateAssemblyReferences();
            assemblyModel.DefinedTypes = CreateTypes(assemblyModel);
            
            return assemblyModel;
        }

        private List<NetAssembly> CreateAssemblyReferences()
        {
            return Module.AssemblyReferences
                .Select(CreateAssemblyModel)
                .ToList();
        }

        private NetAssembly CreateAssemblyModel(AssemblyNameReference assemblyReference)
        {
            return Factory.CreateAssemblyModel(assemblyReference);
        }

        private IList<NetType> CreateTypes(NetAssembly assembly)
        {
            var typeScanner = new TypeScanner(Module, Factory, Logger);

            return Module.Types
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

            return !type.Name.StartsWith("<");
        }
    }
}
