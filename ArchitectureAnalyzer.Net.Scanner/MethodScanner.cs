namespace ArchitectureAnalyzer.Net.Scanner
{
    using System.Collections.Generic;
    using System.Linq;

    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;
    using ArchitectureAnalyzer.Net.Scanner.Utils;

    using Microsoft.Extensions.Logging;

    using Mono.Cecil;
    
    internal class MethodScanner : AbstractScanner
    {
        public MethodScanner(ModuleDefinition module, IModelFactory factory, ILogger logger)
            : base(module, factory, logger)
        {
        }

        public NetMethod ScanMethod(MethodDefinition method, NetType typeModel)
        {
            Logger.LogTrace("    Scanning method '{0}'", method.Name);
            
            var methodModel = Factory.CreateMethodModel(method);
            methodModel.DeclaringType = typeModel;
            methodModel.Visibility = GetVisibility(method);
            methodModel.IsAbstract = IsAbstract(method);
            methodModel.IsStatic = IsStatic(method);
            methodModel.IsSealed = IsSealed(method);
            methodModel.IsGeneric = IsGeneric(method);
            methodModel.GenericParameters = CreateGenericParameters(method);
            
            methodModel.ReturnType = GetTypeFromTypeReference(method.ReturnType);
            methodModel.Parameters = CreateParameters(method, methodModel);
            
            return methodModel;
        }

        private Visibility GetVisibility(MethodDefinition method)
        {
            return method.Attributes.ToVisibility();
        }

        private IReadOnlyList<NetMethodParameter> CreateParameters(
            MethodDefinition method,
            NetMethod methodModel)
        {
            return method.Parameters
                .Select(param => CreateParameter(method, param, methodModel))
                .ToList();
        }

        private NetMethodParameter CreateParameter(
            MethodDefinition method,
            ParameterDefinition param,
            NetMethod methodModel)
        {
            var model = Factory.CreateMethodParameter(method, param);
            model.Order = param.Sequence;
            model.Type = GetTypeFromTypeReference(param.ParameterType);
            model.DeclaringMethod = methodModel;

            return model;
        }

        private IReadOnlyList<NetType> CreateGenericParameters(MethodDefinition method)
        {
            return method.GenericParameters
                .Select(CreateGenericParameter)
                .ToList();
        }

        private NetType CreateGenericParameter(GenericParameter arg)
        {
            return Factory.CreateGenericTypeArg(arg);
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

        private static bool IsGeneric(MethodDefinition method)
        {
            return method.HasGenericParameters;
        }
    }
}