
namespace ArchitectureAnalyzer.DotnetScanner.Scanner
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Text;

    using ArchitectureAnalyzer.DotnetScanner.Model;
    using ArchitectureAnalyzer.DotnetScanner.Utils;
    using ArchitectureAnalyzer.Net.Model;

    internal class SignatureTypeProvider : ISignatureTypeProvider<NetType, object>
    {
        private readonly IModelFactory _factory;

        public SignatureTypeProvider(IModelFactory factory)
        {
            _factory = factory;
        }

        public NetType GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Boolean:
                    return _factory.CreateTypeModel(TypeKey.FromType<bool>());

                case PrimitiveTypeCode.Byte:
                    return _factory.CreateTypeModel(TypeKey.FromType<byte>());

                case PrimitiveTypeCode.SByte:
                    return _factory.CreateTypeModel(TypeKey.FromType<sbyte>());

                case PrimitiveTypeCode.Char:
                    return _factory.CreateTypeModel(TypeKey.FromType<char>());

                case PrimitiveTypeCode.Int16:
                    return _factory.CreateTypeModel(TypeKey.FromType<short>());

                case PrimitiveTypeCode.UInt16:
                    return _factory.CreateTypeModel(TypeKey.FromType<ushort>());

                case PrimitiveTypeCode.Int32:
                    return _factory.CreateTypeModel(TypeKey.FromType<int>());

                case PrimitiveTypeCode.UInt32:
                    return _factory.CreateTypeModel(TypeKey.FromType<uint>());

                case PrimitiveTypeCode.Int64:
                    return _factory.CreateTypeModel(TypeKey.FromType<long>());

                case PrimitiveTypeCode.UInt64:
                    return _factory.CreateTypeModel(TypeKey.FromType<ulong>());

                case PrimitiveTypeCode.Single:
                    return _factory.CreateTypeModel(TypeKey.FromType<float>());

                case PrimitiveTypeCode.Double:
                    return _factory.CreateTypeModel(TypeKey.FromType<double>());

                case PrimitiveTypeCode.IntPtr:
                    return _factory.CreateTypeModel(TypeKey.FromType<IntPtr>());

                case PrimitiveTypeCode.UIntPtr:
                    return _factory.CreateTypeModel(TypeKey.FromType<UIntPtr>());

                case PrimitiveTypeCode.Object:
                    return _factory.CreateTypeModel(TypeKey.FromType<object>());

                case PrimitiveTypeCode.String:
                    return _factory.CreateTypeModel(TypeKey.FromType<string>());

                case PrimitiveTypeCode.TypedReference:
                    // todo: not sure
                    return _factory.CreateTypeModel(TypeKey.FromType<TypeReference>());

                case PrimitiveTypeCode.Void:
                    return _factory.CreateTypeModel(TypeKey.FromType(typeof(void)));

                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }

        public NetType GetTypeFromDefinition(
            MetadataReader reader,
            TypeDefinitionHandle handle,
            byte rawTypeKind)
        {
            return _factory.CreateTypeModel(handle.GetTypeKey(reader));
        }

        public NetType GetTypeFromReference(
            MetadataReader reader,
            TypeReferenceHandle handle,
            byte rawTypeKind)
        {
            return _factory.CreateTypeModel(handle.GetTypeKey(reader));
        }

        public NetType GetTypeFromSpecification(
            MetadataReader reader,
            object genericContext,
            TypeSpecificationHandle handle,
            byte rawTypeKind)
        {
            throw new NotImplementedException();
        }

        public NetType GetSZArrayType(NetType elementType)
        {
            return _factory.CreateTypeModel(elementType.GetKey().ToArrayType());
        }

        public NetType GetGenericInstantiation(NetType genericType, ImmutableArray<NetType> typeArguments)
        {
            var key = genericType.GetKey().ToGenericType(typeArguments.Select(t => t.GetKey()));

            return _factory.CreateTypeModel(key);
        }

        public NetType GetArrayType(NetType elementType, ArrayShape shape)
        {
            var builder = new StringBuilder();

            var elementTypeKey = elementType.GetKey();

            builder.Append(elementTypeKey.Name);
            builder.Append('[');

            for (var i = 0; i < shape.Rank; i++)
            {
                var lowerBound = 0;

                if (i < shape.LowerBounds.Length)
                {
                    lowerBound = shape.LowerBounds[i];
                    builder.Append(lowerBound);
                }

                builder.Append("...");

                if (i < shape.Sizes.Length)
                {
                    builder.Append(lowerBound + shape.Sizes[i] - 1);
                }

                if (i < shape.Rank - 1)
                {
                    builder.Append(',');
                }
            }

            builder.Append(']');

            return _factory.CreateTypeModel(new TypeKey(elementType.Namespace, builder.ToString()));
        }

        public NetType GetByReferenceType(NetType elementType)
        {
            return _factory.CreateTypeModel(elementType.GetKey().ToReferenceType());
        }

        public NetType GetPointerType(NetType elementType)
        {
            return _factory.CreateTypeModel(elementType.GetKey().ToPointerType());
        }

        public NetType GetFunctionPointerType(MethodSignature<NetType> signature)
        {
            throw new NotImplementedException();
        }

        public NetType GetGenericMethodParameter(object genericContext, int index)
        {
            return _factory.CreateTypeModel(new TypeKey(string.Empty, "!!" + index));
        }

        public NetType GetGenericTypeParameter(object genericContext, int index)
        {
            return _factory.CreateTypeModel(new TypeKey(string.Empty, "!" + index));
        }

        public NetType GetModifiedType(NetType modifier, NetType unmodifiedType, bool isRequired)
        {
            var key = unmodifiedType.GetKey().ToModifiedType(modifier.GetKey());

            return _factory.CreateTypeModel(key);
        }

        public NetType GetPinnedType(NetType elementType)
        {
            return _factory.CreateTypeModel(elementType.GetKey().ToPinnedType());
        }
    }
}
