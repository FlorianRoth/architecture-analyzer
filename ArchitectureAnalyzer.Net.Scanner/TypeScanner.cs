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
            typeModel.HasAttribute = HasAttribute(type);
            typeModel.IsGeneric = IsGeneric(type);
            typeModel.GenericTypeArgs = CreateGenericTypeArgs(type, typeKey);
            typeModel.Methods = CreateMethods(type, typeModel).ToList();
            typeModel.Properties = CreateProperties(type, typeModel).ToList();
            typeModel.DisplayName = GetDisplayName(typeModel);

            SetBaseType(type, typeModel);
            SetImplementedInterfaces(type, typeModel);
            SetAttributes(type, typeModel);
            
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
            var visibility = type.Attributes & TypeAttributes.VisibilityMask;

            switch (visibility)
            {
                case TypeAttributes.Public:
                case TypeAttributes.NestedPublic:
                    return Visibility.Public;

                case TypeAttributes.NotPublic:
                    return Visibility.Internal;


                case TypeAttributes.NestedAssembly:
                    return Visibility.Internal;

                case TypeAttributes.NestedFamily:
                    return Visibility.Protected;

                case TypeAttributes.NestedPrivate:
                    return Visibility.Private;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool IsSealed(TypeDefinition type)
        {
            var atts = type.Attributes;
            return atts.HasFlag(TypeAttributes.Sealed) && !atts.HasFlag(TypeAttributes.Abstract);
        }

        private bool HasAttribute(TypeDefinition type)
        {
            var atts = type.GetCustomAttributes();
            return atts.Any();
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

        private void SetBaseType(TypeDefinition type, NetType model)
        {
            model.BaseType = GetTypeFromEntityHandle(type.BaseType);
        }

        private bool IsGeneric(TypeDefinition type)
        {
            return type.GetGenericParameters().Any();
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

        private IEnumerable<NetMethod> CreateMethods(TypeDefinition type, NetType typeModel)
        {
            var methodScanner = new MethodScanner(Reader, Factory, Logger);

            var methods = type.GetMethods()
                .Select(Reader.GetMethodDefinition)
                .Where(IncludeMethod);

            foreach (var method in methods)
            {
                var methodModel = methodScanner.ScanMethod(method, typeModel);

                if (methodModel == null)
                {
                    continue;
                }

                yield return methodModel;
            }
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

        private IEnumerable<NetProperty> CreateProperties(TypeDefinition type, NetType typeModel)
        {
            var propertyScanner = new PropertyScanner(Reader, Factory, Logger);

            foreach (var property in type.GetProperties().Select(Reader.GetPropertyDefinition))
            {
                var propertyModel = propertyScanner.ScanProperty(property, typeModel);

                if (propertyModel == null)
                {
                    continue;
                }

                yield return propertyModel;
            }
        }


        private void SetImplementedInterfaces(TypeDefinition type, NetType model)
        {
            foreach (var interfaceImpl in type.GetInterfaceImplementations().Select(Reader.GetInterfaceImplementation))
            {
                var interfaceType = GetTypeFromEntityHandle(interfaceImpl.Interface);
                if (interfaceType == null)
                {
                    return;
                }

                model.Implements.Add(interfaceType);
            }
        }

        private void SetAttributes(TypeDefinition type, NetType model)
        {
            foreach (var attributeHandle in type.GetCustomAttributes())
            {
                var attributeType = GetTypeFromEntityHandle(attributeHandle);

                if (attributeType == null)
                {
                    return;
                }

                model.Attributes.Add(attributeType);
            }
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
