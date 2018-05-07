
namespace ArchitectureAnalyzer.DotnetScanner.Scanner
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Text;

    using ArchitectureAnalyzer.DotnetScanner.Model;
    using ArchitectureAnalyzer.DotnetScanner.Utils;

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
                    return _factory.CreateTypeModel(typeof(bool).Name);

                case PrimitiveTypeCode.Byte:
                    return _factory.CreateTypeModel(typeof(byte).Name);

                case PrimitiveTypeCode.SByte:
                    return _factory.CreateTypeModel(typeof(sbyte).Name);

                case PrimitiveTypeCode.Char:
                    return _factory.CreateTypeModel(typeof(char).Name);

                case PrimitiveTypeCode.Int16:
                    return _factory.CreateTypeModel(typeof(short).Name);

                case PrimitiveTypeCode.UInt16:
                    return _factory.CreateTypeModel(typeof(ushort).Name);

                case PrimitiveTypeCode.Int32:
                    return _factory.CreateTypeModel(typeof(int).Name);

                case PrimitiveTypeCode.UInt32:
                    return _factory.CreateTypeModel(typeof(uint).Name);

                case PrimitiveTypeCode.Int64:
                    return _factory.CreateTypeModel(typeof(long).Name);

                case PrimitiveTypeCode.UInt64:
                    return _factory.CreateTypeModel(typeof(ulong).Name);

                case PrimitiveTypeCode.Single:
                    return _factory.CreateTypeModel(typeof(float).Name);

                case PrimitiveTypeCode.Double:
                    return _factory.CreateTypeModel(typeof(double).Name);

                case PrimitiveTypeCode.IntPtr:
                    return _factory.CreateTypeModel(typeof(IntPtr).Name);

                case PrimitiveTypeCode.UIntPtr:
                    return _factory.CreateTypeModel(typeof(UIntPtr).Name);

                case PrimitiveTypeCode.Object:
                    return _factory.CreateTypeModel(typeof(object).Name);

                case PrimitiveTypeCode.String:
                    return _factory.CreateTypeModel(typeof(string).Name);

                case PrimitiveTypeCode.TypedReference:
                    // todo: not sure
                    return _factory.CreateTypeModel(typeof(TypeReference).Name);

                case PrimitiveTypeCode.Void:
                    return _factory.CreateTypeModel(typeof(void).Name);

                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
            }
        }

        public NetType GetTypeFromDefinition(
            MetadataReader reader,
            TypeDefinitionHandle handle,
            byte rawTypeKind)
        {
            return _factory.CreateTypeModel(handle.GetTypeId(reader));
        }

        public NetType GetTypeFromReference(
            MetadataReader reader,
            TypeReferenceHandle handle,
            byte rawTypeKind)
        {
            return _factory.CreateTypeModel(handle.GetTypeId(reader));
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
            return _factory.CreateTypeModel(elementType.Id + "[]");
        }

        public NetType GetGenericInstantiation(NetType genericType, ImmutableArray<NetType> typeArguments)
        {
            var id = genericType.Id + "<" + string.Join(",", typeArguments.Select(a => a.Id)) + ">";

            return _factory.CreateTypeModel(id);
        }

        public NetType GetArrayType(NetType elementType, ArrayShape shape)
        {
            var builder = new StringBuilder();

            builder.Append(elementType.Id);
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

            return _factory.CreateTypeModel(builder.ToString());
        }

        public NetType GetByReferenceType(NetType elementType)
        {
            return _factory.CreateTypeModel(elementType.Id + "&");
        }

        public NetType GetPointerType(NetType elementType)
        {
            return _factory.CreateTypeModel(elementType.Id + "*");
        }

        public NetType GetFunctionPointerType(MethodSignature<NetType> signature)
        {
            throw new NotImplementedException();
        }

        public NetType GetGenericMethodParameter(object genericContext, int index)
        {
            return _factory.CreateTypeModel("!!" + index);
        }

        public NetType GetGenericTypeParameter(object genericContext, int index)
        {
            return _factory.CreateTypeModel("!" + index);
        }

        public NetType GetModifiedType(NetType modifier, NetType unmodifiedType, bool isRequired)
        {
            var id = modifier.Id + " " + unmodifiedType.Id;

            return _factory.CreateTypeModel(id);
        }

        public NetType GetPinnedType(NetType elementType)
        {
            return _factory.CreateTypeModel(elementType.Id + " pinned");
        }
    }
}
