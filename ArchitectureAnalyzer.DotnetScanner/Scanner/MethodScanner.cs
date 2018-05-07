namespace ArchitectureAnalyzer.DotnetScanner.Scanner
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.DotnetScanner.Model;
    using ArchitectureAnalyzer.DotnetScanner.Utils;

    using Microsoft.Extensions.Logging;

    internal class MethodScanner : AbstractScanner
    {
        private static readonly ISet<string> SpecialNames =
            new HashSet<string>
                {
                    ".ctor",
                    "op_Implicit",
                    "op_Explicit",
                    "op_Addition",
                    "op_Subtraction",
                    "op_Multiply",
                    "op_Division",
                    "op_Modulus",
                    "op_ExclusiveOr",
                    "op_BitwiseAnd",
                    "op_BitwiseOr",
                    "op_LogicalAnd",
                    "op_LogicalOr",
                    "op_Assign",
                    "op_LeftShift",
                    "op_RightShift",
                    "op_SignedRightShift",
                    "op_UnsignedRightShift",
                    "op_Equality",
                    "op_GreaterThan",
                    "op_LessThan",
                    "op_Inequality",
                    "op_GreaterThanOrEqual",
                    "op_LessThanOrEqual",
                    "op_MultiplicationAssignment",
                    "op_SubtractionAssignment",
                    "op_ExclusiveOrAssignment",
                    "op_LeftShiftAssignment",
                    "op_ModulusAssignment",
                    "op_AdditionAssignment",
                    "op_BitwiseAndAssignment",
                    "op_BitwiseOrAssignment",
                    "op_Comma",
                    "op_DivisionAssignment",
                    "op_Decrement",
                    "op_Increment",
                    "op_UnaryNegation",
                    "op_UnaryPlus",
                    "op_OnesComplement"
                };

        public MethodScanner(MetadataReader reader, IModelFactory factory, ILogger logger)
            : base(reader, factory, logger)
        {
        }

        public NetMethod ScanMethod(MethodDefinition method, NetType typeModel)
        {
            Logger.LogTrace("    Scanning method '{0}'", method.Name.GetString(Reader));

            if (IncludeMethod(method) == false)
            {
                return null;
            }

            var name = method.Name.GetString(Reader);
            var signatureTypeProvider = new SignatureTypeProvider(Factory);
            var signature = method.DecodeSignature(signatureTypeProvider, null);

            var methodModel = Factory.CreateMethodModel(typeModel.Id + "." + name);
            methodModel.Name = name;
            methodModel.IsAbstract = IsAbstract(method);
            methodModel.IsStatic = IsStatic(method);
            methodModel.IsSealed = IsSealed(method);
            methodModel.ReturnType = signature.ReturnType;
            methodModel.ParameterTypes = signature.ParameterTypes;

            return methodModel;
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

        private bool IncludeMethod(MethodDefinition method)
        {
            if ((method.Attributes & MethodAttributes.MemberAccessMask) != MethodAttributes.Public)
            {
                return false;
            }

            var name = method.Name.GetString(Reader);
            if (SpecialNames.Contains(name))
            {
                return true;
            }

            if (method.Attributes.HasFlag(MethodAttributes.SpecialName))
            {
                return false;
            }

            return true;
        }
    }
}