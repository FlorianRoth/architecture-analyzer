namespace ArchitectureAnalyzer.Net.Scanner
{
    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;

    using Microsoft.Extensions.Logging;

    using Mono.Cecil;

    internal abstract class AbstractScanner
    {
        protected ModuleDefinition Module { get; }

        protected IModelFactory Factory { get; }

        protected ILogger Logger { get; }

        protected AbstractScanner(ModuleDefinition module, IModelFactory factory, ILogger logger)
        {
            Module = module;
            Factory = factory;
            Logger = logger;
        }

        protected NetType GetTypeFromCustomAttribute(CustomAttribute customAttribute)
        {
            return GetTypeFromTypeReference(customAttribute.AttributeType);
        }

        protected NetType GetTypeFromTypeReference(TypeReference typeReference)
        {
            return Factory.CreateTypeModel(typeReference);
        }
    }
}
