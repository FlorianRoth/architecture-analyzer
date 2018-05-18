
namespace ArchitectureAnalyzer.Net.Scanner
{
    using System.Collections.Generic;

    internal static class ScannerConstants
    {
        public static readonly ISet<string> MethodSpecialNames =
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
    }
}
