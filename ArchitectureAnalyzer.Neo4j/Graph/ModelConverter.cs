
namespace ArchitectureAnalyzer.Neo4j.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using ArchitectureAnalyzer.Core.Graph;

    internal static class ModelConverter
    {
        public static IDictionary<string, object> Convert(object model)
        {
            var result = new Dictionary<string, object>();

            var type = model.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                if (IsIgnored(property))
                {
                    continue;
                }

                ConvertProperty(property, model, result);
            }
            
            return result;
        }

        private static void ConvertProperty(PropertyInfo property, object model, IDictionary<string, object> result)
        {
            var type = property.PropertyType;
            var name = property.Name;
            var value = property.GetValue(model);

            if (type.IsEnum)
            {
                value = Enum.Format(type, value, "G");
            }
            
            result[name] = value;
        }

        private static bool IsIgnored(PropertyInfo property)
        {
            return property.GetCustomAttribute<IgnoreAttribute>() != null;
        }
    }
}
