namespace ArchitectureAnalyzer.DotnetScanner.Scanner
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.DotnetScanner.Model;
    using ArchitectureAnalyzer.DotnetScanner.Utils;

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

            if (IncludeType(type) == false)
            {
                return null;
            }

            var typeModel = Factory.CreateTypeModel(type.GetTypeKey(Reader));
            typeModel.Type = GetTypeClass(type);
            typeModel.Name = type.Name.GetString(Reader);
            typeModel.Namespace = type.Namespace.GetString(Reader);
            typeModel.IsStatic = IsStatic(type);
            typeModel.IsAbstract = IsAbstract(type);
            typeModel.IsSealed = IsSealed(type);
            typeModel.HasAttribute = HasAttribute(type);

            SetBaseType(type, typeModel);
            SetImplementedInterfaces(type, typeModel);
            SetAttributes(type, typeModel);

            var methodScanner = new MethodScanner(Reader, Factory, Logger);
            foreach (var method in type.GetMethods().Select(Reader.GetMethodDefinition))
            {
               var methodModel =  methodScanner.ScanMethod(method, typeModel);

                if (methodModel == null)
                {
                    continue;
                }

                typeModel.Methods.Add(methodModel);
            }

            return typeModel;
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
                return GetTypeFromEntityHandle((TypeDefinitionHandle)methodDefinition.GetDeclaringType());
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

        private bool IncludeType(TypeDefinition type)
        {
            if ((type.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public)
            {
                return false;
            }

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
