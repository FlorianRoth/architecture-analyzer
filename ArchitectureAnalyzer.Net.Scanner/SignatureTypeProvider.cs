
namespace ArchitectureAnalyzer.Net.Scanner
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Text;

    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;
    using ArchitectureAnalyzer.Net.Scanner.Utils;

    internal class SignatureTypeProvider : ISignatureTypeProvider<NetType, object>
    {
        private readonly IModelFactory _factory;

        private readonly IDictionary<PrimitiveTypeCode, NetType> _primitiveTypes;

        public SignatureTypeProvider(IModelFactory factory)
        {
            _factory = factory;

            _primitiveTypes = new Dictionary<PrimitiveTypeCode, NetType>
                                  {
                                      { PrimitiveTypeCode.Boolean, _factory.CreateTypeModel(TypeKey.FromType<bool>()) },
                                      { PrimitiveTypeCode.Byte, _factory.CreateTypeModel(TypeKey.FromType<byte>()) },
                                      { PrimitiveTypeCode.SByte, _factory.CreateTypeModel(TypeKey.FromType<sbyte>()) },
                                      { PrimitiveTypeCode.Char, _factory.CreateTypeModel(TypeKey.FromType<char>()) },
                                      { PrimitiveTypeCode.Int16, _factory.CreateTypeModel(TypeKey.FromType<short>()) },
                                      { PrimitiveTypeCode.UInt16, _factory.CreateTypeModel(TypeKey.FromType<ushort>()) },
                                      { PrimitiveTypeCode.Int32, _factory.CreateTypeModel(TypeKey.FromType<int>()) },
                                      { PrimitiveTypeCode.UInt32, _factory.CreateTypeModel(TypeKey.FromType<uint>()) },
                                      { PrimitiveTypeCode.Int64, _factory.CreateTypeModel(TypeKey.FromType<long>()) },
                                      { PrimitiveTypeCode.UInt64, _factory.CreateTypeModel(TypeKey.FromType<ulong>()) },
                                      { PrimitiveTypeCode.Single, _factory.CreateTypeModel(TypeKey.FromType<float>()) },
                                      { PrimitiveTypeCode.Double, _factory.CreateTypeModel(TypeKey.FromType<double>()) },
                                      { PrimitiveTypeCode.IntPtr, _factory.CreateTypeModel(TypeKey.FromType<IntPtr>()) },
                                      { PrimitiveTypeCode.UIntPtr, _factory.CreateTypeModel(TypeKey.FromType<UIntPtr>()) },
                                      { PrimitiveTypeCode.Object, _factory.CreateTypeModel(TypeKey.FromType<object>()) },
                                      { PrimitiveTypeCode.String, _factory.CreateTypeModel(TypeKey.FromType<string>()) },
                                      { PrimitiveTypeCode.Void, _factory.CreateTypeModel(TypeKey.FromType(typeof(void))) },
                                      // Not sure about this one
                                      { PrimitiveTypeCode.TypedReference, _factory.CreateTypeModel(TypeKey.FromType<TypeReference>()) }
                                  };
        }

        public NetType GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            if (_primitiveTypes.TryGetValue(typeCode, out var type))
            {
                return type;
            }

            throw new ArgumentOutOfRangeException(nameof(typeCode), typeCode, null);
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
            var key = genericType.GetKey().ToGenericType(typeArguments.Select(t => TypeKeyExtensions.GetKey(t)));

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
