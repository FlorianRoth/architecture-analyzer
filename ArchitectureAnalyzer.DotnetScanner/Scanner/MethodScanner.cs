namespace ArchitectureAnalyzer.DotnetScanner.Scanner
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.Core.Graph;
    using ArchitectureAnalyzer.DotnetScanner.Model;

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

        public MethodScanner(MetadataReader reader, IModelFactory factory, IGraphDatabase database, ILogger logger)
            : base(reader, factory, database, logger)
        {
        }

        public NetMethod ScanMethod(MethodDefinition method, NetType typeModel)
        {
            Logger.LogTrace("    Scanning method '{0}'", GetString(method.Name));

            if (IncludeMethod(method) == false)
            {
                return null;
            }

            var name = GetString(method.Name);

            var model = Factory.CreateMethodModel(typeModel.Id + "." + name);
            model.Name = name;
            model.IsAbstract = IsAbstract(method);
            model.IsStatic = IsStatic(method);
            model.IsSealed = IsSealed(method);

            Database.CreateNode(model);

            return model;
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

            var name = GetString(method.Name);
            if (SpecialNames.Contains(name))
            {
                return true;
            }

            if ((method.Attributes & MethodAttributes.SpecialName) != 0)
            {
                return false;
            }

            return true;
        }
    }
}