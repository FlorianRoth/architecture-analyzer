namespace ArchitectureAnalyzer.Net.Scanner
{
    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;

    using Microsoft.Extensions.Logging;

    using Mono.Cecil;

    internal class PropertyScanner : AbstractScanner
    {
        public PropertyScanner(ModuleDefinition module, IModelFactory factory, ILogger logger) : base(module, factory, logger)
        {
        }

        public NetProperty ScanProperty(PropertyDefinition property, NetType typeModel)
        {
            Logger.LogTrace("    Scanning property '{0}'", property.Name);
            
            var propertyModel = Factory.CreatePropertyModel(property);
            propertyModel.DeclaringType = typeModel;
            propertyModel.Type = GetTypeFromTypeReference(property.GetMethod.ReturnType);

            return propertyModel;
        }
    }
}