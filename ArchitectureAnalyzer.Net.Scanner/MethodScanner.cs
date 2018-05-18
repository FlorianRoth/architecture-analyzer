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

    internal class MethodScanner : AbstractScanner
    {
        public MethodScanner(MetadataReader reader, IModelFactory factory, ILogger logger)
            : base(reader, factory, logger)
        {
        }

        public NetMethod ScanMethod(MethodDefinition method, NetType typeModel)
        {
            Logger.LogTrace("    Scanning method '{0}'", method.Name.GetString(Reader));

            var key = new MethodKey(
                typeModel.GetKey(),
                method.Name.GetString(Reader),
                method.Signature.GetHashCode());

            var methodModel = Factory.CreateMethodModel(key);
            methodModel.DeclaringType = typeModel;
            methodModel.Visibility = GetVisibility(method);
            methodModel.IsAbstract = IsAbstract(method);
            methodModel.IsStatic = IsStatic(method);
            methodModel.IsSealed = IsSealed(method);
            methodModel.IsGeneric = IsGeneric(method);
            methodModel.GenericParameters = CreateGenericParameters(method, key);

            var signatureTypeProvider = new SignatureTypeProvider(Factory);
            var signature = method.DecodeSignature(signatureTypeProvider, methodModel);

            methodModel.ReturnType = signature.ReturnType;
            methodModel.Parameters = CreateParameters(method, methodModel, signature.ParameterTypes);
            
            return methodModel;
        }

        private Visibility GetVisibility(MethodDefinition method)
        {
            return method.Attributes.ToVisibility();
        }

        private IReadOnlyList<NetMethodParameter> CreateParameters(
            MethodDefinition method,
            NetMethod methodModel,
            IReadOnlyList<NetType> signatureParameterTypes)
        {
            return method.GetParameters()
                .Select(Reader.GetParameter)
                .Select((param, order) => CreateParameter(param, order, methodModel, signatureParameterTypes[order]))
                .ToList();
        }

        private NetMethodParameter CreateParameter(
            Parameter param,
            int order,
            NetMethod methodModel,
            NetType parameterType)
        {
            var name = param.Name.GetString(Reader);
            var key = new MethodParameterKey(methodModel.Id, name);

            var model = Factory.CreateMethodParameter(key);
            model.Order = order;
            model.Type = parameterType;
            model.DeclaringMethod = methodModel;

            return model;
        }

        private IReadOnlyList<NetType> CreateGenericParameters(MethodDefinition method, MethodKey methodKey)
        {
            return method.GetGenericParameters()
                .Select(Reader.GetGenericParameter)
                .Select(arg => CreateGenericParameter(arg, methodKey))
                .ToList();
        }

        private NetType CreateGenericParameter(GenericParameter arg, MethodKey methodKey)
        {
            var name = arg.Name.GetString(Reader);

            return Factory.CreateGenericParameter(methodKey, name);
        }

        private static bool IsAbstract(MethodDefinition method)
        {
            return method.Attributes.HasFlag(MethodAttributes.Abstract);
        }

        private static bool IsStatic(MethodDefinition method)
        {
            return method.Attributes.HasFlag(MethodAttributes.Static);
        }
        
        private static bool IsSealed(MethodDefinition method)
        {
            return method.Attributes.HasFlag(MethodAttributes.Final);
        }

        private bool IsGeneric(MethodDefinition method)
        {
            return method.GetGenericParameters().Any();
        }
    }
}