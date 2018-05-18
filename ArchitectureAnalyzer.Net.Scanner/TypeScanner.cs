namespace ArchitectureAnalyzer.Net.Scanner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;
    using ArchitectureAnalyzer.Net.Scanner.Utils;

    using Microsoft.Extensions.Logging;

    internal class TypeScanner : AbstractScanner
    {
        private static readonly TypeKey EnumKey = new TypeKey(typeof(Enum).Namespace, typeof(Enum).Name);

        public TypeScanner(MetadataReader reader, IModelFactory factory, ILogger logger)
            : base(reader, factory, logger)
        {
        }

        public NetType ScanType(TypeDefinition type)
        {
            Logger.LogTrace("  Scanning type '{0}'", type.Name.GetString(Reader));

            var typeKey = type.GetTypeKey(Reader);

            var typeModel = Factory.CreateTypeModel(typeKey);
            typeModel.Type = GetTypeClass(type);
            typeModel.Visibility = GetVisibility(type);
            typeModel.IsStatic = IsStatic(type);
            typeModel.IsAbstract = IsAbstract(type);
            typeModel.IsSealed = IsSealed(type);
            typeModel.GenericTypeArgs = CreateGenericTypeArgs(type, typeKey);
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

            var baseType = GetTypeFromEntityHandle(type.BaseType);
            if (Equals(baseType?.GetKey(), EnumKey))
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
            return GetTypeFromEntityHandle(type.BaseType);
        }
        
        private IList<NetType> CreateGenericTypeArgs(TypeDefinition type, TypeKey typeKey)
        {
            return type.GetGenericParameters()
                .Select(Reader.GetGenericParameter)
                .Select(arg => CreateGenericTypeArg(arg, typeKey))
                .ToList();
        }

        private NetType CreateGenericTypeArg(GenericParameter genericParameter, TypeKey typeKey)
        {
            var name = genericParameter.Name.GetString(Reader);

            return Factory.CreateGenericTypeArg(typeKey, name);
        }

        private IList<NetMethod> CreateMethods(TypeDefinition type, NetType typeModel)
        {
            var methodScanner = new MethodScanner(Reader, Factory, Logger);

            return type.GetMethods()
                .Select(Reader.GetMethodDefinition)
                .Where(IncludeMethod)
                .Select(method => methodScanner.ScanMethod(method, typeModel))
                .ToList();
        }

        private bool IncludeMethod(MethodDefinition method)
        {
            var name = method.Name.GetString(Reader);
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
            var propertyScanner = new PropertyScanner(Reader, Factory, Logger);

            return type.GetProperties()
                .Select(Reader.GetPropertyDefinition)
                .Where(IncludeProperty)
                .Select(property => propertyScanner.ScanProperty(property, typeModel))
                .ToList();
        }

        private bool IncludeProperty(PropertyDefinition property)
        {
            return !property.Attributes.HasFlag(PropertyAttributes.SpecialName);
        }
        
        private IList<NetType> GetImplementedInterfaces(TypeDefinition type)
        {
            return type.GetInterfaceImplementations()
                .Select(Reader.GetInterfaceImplementation)
                .Select(interfaceImpl => GetTypeFromEntityHandle(interfaceImpl.Interface))
                .Where(i => i != null)
                .ToList();
        }

        private IList<NetType> GetAttributes(TypeDefinition type)
        {
            return type.GetCustomAttributes()
                .Select(GetTypeFromCustomAttributeHandle)
                .Where(t => t != null)
                .ToList();
        }

        private NetType GetTypeFromEntityHandle(EntityHandle handle)
        {
            if (handle.IsNil)
            {
                return null;
            }

            switch (handle.Kind)
            {
                case HandleKind.TypeDefinition:
                    return GetTypeFromTypeDefinitonHandle((TypeDefinitionHandle)handle);

                case HandleKind.TypeReference:
                    return GetTypeFromTypeReferenceHandle((TypeReferenceHandle)handle);

                case HandleKind.CustomAttribute:
                    return GetTypeFromCustomAttributeHandle((CustomAttributeHandle)handle);

                case HandleKind.MethodDefinition:
                    return GetTypeFromMethodDefinitionHandle((MethodDefinitionHandle)handle);

                case HandleKind.MemberReference:
                    return GetTypeFromMemberReferenceHandle((MemberReferenceHandle)handle);
                default:
                    return null;
            }
        }

        private NetType GetTypeFromMethodDefinitionHandle(MethodDefinitionHandle handle)
        {
            var methodDefinition = Reader.GetMethodDefinition(handle);
            return GetTypeFromEntityHandle(methodDefinition.GetDeclaringType());
        }

        private NetType GetTypeFromMemberReferenceHandle(MemberReferenceHandle handle)
        {
            var memberReference = Reader.GetMemberReference(handle);
            return GetTypeFromEntityHandle((TypeReferenceHandle)memberReference.Parent);
        }

        private NetType GetTypeFromCustomAttributeHandle(CustomAttributeHandle handle)
        {
            var attribute = Reader.GetCustomAttribute(handle);
            return GetTypeFromEntityHandle(attribute.Constructor);
        }

        private NetType GetTypeFromTypeReferenceHandle(TypeReferenceHandle handle)
        {
            var id = Reader.GetTypeReference(handle).GetTypeKey(Reader);
            return Factory.CreateTypeModel(id);
        }

        private NetType GetTypeFromTypeDefinitonHandle(TypeDefinitionHandle handle)
        {
            var id = Reader.GetTypeDefinition(handle).GetTypeKey(Reader);
            return Factory.CreateTypeModel(id);
        }
    }
}
