namespace ArchitectureAnalyzer.DotnetScanner.Scanner
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.DotnetScanner.Model;

    using Microsoft.Extensions.Logging;

    internal class TypeScanner : AbstractScanner
    {
        public TypeScanner(MetadataReader reader, IModelFactory factory, IGraphDatabase database, ILogger logger)
            : base(reader, factory, database, logger)
        {
        }

        public NetType ScanType(TypeDefinition type)
        {
            Logger.LogTrace("  Scanning type '{0}'", GetString(type.Name));

            if (IncludeType(type) == false)
            {
                return null;
            }

            var model = Factory.CreateTypeModel(GetTypeId(type));
            model.Type = GetTypeClass(type);
            model.Name = GetString(type.Name);
            model.Namespace = GetString(type.Namespace);
            model.IsStatic = IsStatic(type);
            model.IsAbstract = IsAbstract(type);
            model.IsSealed = IsSealed(type);

            Database.CreateNode(model);

            SetBaseType(type, model);
            SetImplementedInterfaces(type, model);

            var methodScanner = new MethodScanner(Reader, Factory, Database, Logger);
            foreach(var method in type.GetMethods().Select(Reader.GetMethodDefinition))
            {
               var methodModel =  methodScanner.ScanMethod(method, model);

                if (methodModel == null)
                {
                    continue;
                }

                Database.CreateRelationship(model, methodModel, Relationship.DEFINES_METHOD);
            }

            return model;
        }

        private NetType.TypeClass GetTypeClass(TypeDefinition type)
        {
            var atts = type.Attributes;

            if (atts.HasFlag(TypeAttributes.Interface))
            {
                return NetType.TypeClass.Interface;
            }

            var baseType = GetTypeFromEntityHandle(type.BaseType);
            if (Equals(baseType?.Id, typeof(Enum).FullName))
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

                default:
                    return null;
            }
        }

        private NetType GetTypeFromTypeReferenceHandle(TypeReferenceHandle handle)
        {
            var id = GetTypeId(Reader.GetTypeReference(handle));
            return Factory.CreateTypeModel(id);
        }

        private NetType GetTypeFromTypeDefinitonHandle(TypeDefinitionHandle handle)
        {
            var id = GetTypeId(Reader.GetTypeDefinition(handle));
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

            var name = GetString(type.Name);
            if (name.StartsWith("<"))
            {
                return false;
            }

            return true;
        }
    }
}
