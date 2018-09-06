namespace ArchitectureAnalyzer.Net.Scanner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;
    using ArchitectureAnalyzer.Net.Scanner.Utils;

    using Microsoft.Extensions.Logging;

    using Mono.Cecil;

    internal class TypeScanner : AbstractScanner
    {
        public TypeScanner(ModuleDefinition module, IModelFactory factory, ILogger logger)
            : base(module, factory, logger)
        {
        }

        public NetType ScanType(TypeDefinition type, NetAssembly assembly)
        {
            Logger.LogTrace("  Scanning type '{0}'", type.Name);
            
            var typeModel = Factory.CreateTypeModel(type);
            typeModel.Type = GetTypeClass(type);
            typeModel.Assembly = assembly;
            typeModel.Visibility = GetVisibility(type);
            typeModel.IsStatic = IsStatic(type);
            typeModel.IsAbstract = IsAbstract(type);
            typeModel.IsSealed = IsSealed(type);
            typeModel.GenericTypeArgs = CreateGenericTypeArgs(type);
            typeModel.Methods = CreateMethods(type, typeModel);
            typeModel.Properties = CreateProperties(type, typeModel);
            typeModel.DisplayName = GetDisplayName(typeModel);
            typeModel.Implements = GetImplementedInterfaces(type);
            typeModel.Attributes = GetAttributes(type);
            typeModel.BaseType = GetBaseType(type);
            
            return typeModel;
        }

        private string GetDisplayName(NetType typeModel)
        {
            var displayName = typeModel.Name;

            if (typeModel.IsGeneric)
            {
                displayName = displayName.Substring(0, displayName.LastIndexOf('`'));
            }

            return displayName;
        }

        private NetType.TypeClass GetTypeClass(TypeDefinition type)
        {
            var atts = type.Attributes;

            if (atts.HasFlag(TypeAttributes.Interface))
            {
                return NetType.TypeClass.Interface;
            }
            
            if (Equals(type.BaseType?.FullName, typeof(Enum).FullName))
            {
                return NetType.TypeClass.Enum;
            }

            return NetType.TypeClass.Class;
        }
        
        private Visibility GetVisibility(TypeDefinition type)
        {
            return type.Attributes.ToVisibility();
        }

        private static bool IsSealed(TypeDefinition type)
        {
            var atts = type.Attributes;
            return atts.HasFlag(TypeAttributes.Sealed) && !atts.HasFlag(TypeAttributes.Abstract);
        }
        
        private static bool IsStatic(TypeDefinition type)
        {
            var atts = type.Attributes;
            return atts.HasFlag(TypeAttributes.Sealed) && atts.HasFlag(TypeAttributes.Abstract);
        }

        private static bool IsAbstract(TypeDefinition type)
        {
            var atts = type.Attributes;
            return !atts.HasFlag(TypeAttributes.Sealed) && atts.HasFlag(TypeAttributes.Abstract);
        }

        private NetType GetBaseType(TypeDefinition type)
        {
            var baseType = type.BaseType;

            return baseType == null ? null : GetTypeFromTypeReference(baseType);
        }
        
        private IList<NetType> CreateGenericTypeArgs(TypeDefinition type)
        {
            return type.GenericParameters
                .Select(CreateGenericTypeArg)
                .ToList();
        }

        private NetType CreateGenericTypeArg(GenericParameter genericParameter)
        {
            return Factory.CreateGenericTypeArg(genericParameter);
        }

        private IList<NetMethod> CreateMethods(TypeDefinition type, NetType typeModel)
        {
            var methodScanner = new MethodScanner(Module, Factory, Logger);

            return type.Methods
                .Where(IncludeMethod)
                .Select(method => methodScanner.ScanMethod(method, typeModel))
                .ToList();
        }

        private bool IncludeMethod(MethodDefinition method)
        {
            var name = method.Name;
            if (ScannerConstants.MethodSpecialNames.Contains(name))
            {
                return true;
            }

            if (method.Attributes.HasFlag(MethodAttributes.SpecialName))
            {
                return false;
            }

            return true;
        }

        private IList<NetProperty> CreateProperties(TypeDefinition type, NetType typeModel)
        {
            var propertyScanner = new PropertyScanner(Module, Factory, Logger);

            return type.Properties
                .Where(IncludeProperty)
                .Select(property => propertyScanner.ScanProperty(property, typeModel))
                .ToList();
        }

        private static bool IncludeProperty(PropertyDefinition property)
        {
            return !property.Attributes.HasFlag(PropertyAttributes.SpecialName);
        }
        
        private IList<NetType> GetImplementedInterfaces(TypeDefinition type)
        {
            return type.Interfaces
                .Select(interfaceImpl => GetTypeFromTypeReference(interfaceImpl.InterfaceType))
                .Where(i => i != null)
                .ToList();
        }

        private IList<NetType> GetAttributes(TypeDefinition type)
        {
            return type.CustomAttributes
                .Select(GetTypeFromCustomAttribute)
                .Where(t => t != null)
                .ToList();
        }
    }
}
