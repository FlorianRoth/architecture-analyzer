namespace ArchitectureAnalyzer.Net.Scanner
{
    using System;
    using System.Reflection;
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;
    using ArchitectureAnalyzer.Net.Scanner.Utils;

    using Microsoft.Extensions.Logging;

    internal class PropertyScanner : AbstractScanner
    {
        public PropertyScanner(MetadataReader reader, IModelFactory factory, ILogger logger) : base(reader, factory, logger)
        {
        }

        public NetProperty ScanProperty(PropertyDefinition property, NetType typeModel)
        {
            Logger.LogTrace("    Scanning property '{0}'", property.Name.GetString(Reader));

            if (IncludeProperty(property) == false)
            {
                return null;
            }

            var key = new PropertyKey(
                typeModel.GetKey(),
                property.Name.GetString(Reader));
        
            var propertyModel = Factory.CreatePropertyModel(key);
            propertyModel.DeclaringType = typeModel;

            var signatureTypeProvider = new SignatureTypeProvider(Factory);
            var signature = property.DecodeSignature(signatureTypeProvider, propertyModel);

            propertyModel.Type = signature.ReturnType;

            return propertyModel;
        }

        private bool IncludeProperty(PropertyDefinition property)
        {
            return !property.Attributes.HasFlag(PropertyAttributes.SpecialName);
        }
    }
}