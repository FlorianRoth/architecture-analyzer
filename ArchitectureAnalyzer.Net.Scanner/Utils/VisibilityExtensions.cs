
namespace ArchitectureAnalyzer.Net.Scanner.Utils
{
    using System;
    using System.Reflection;

    using ArchitectureAnalyzer.Net.Model;

    public static class VisibilityExtensions
    {
        public static Visibility ToVisibility(this TypeAttributes typeAttributes)
        {
            var visibilityAttributes = typeAttributes & TypeAttributes.VisibilityMask;

            switch (visibilityAttributes)
            {
                case TypeAttributes.Public:
                case TypeAttributes.NestedPublic:
                    return Visibility.Public;

                case TypeAttributes.NotPublic:
                    return Visibility.Internal;


                case TypeAttributes.NestedAssembly:
                    return Visibility.Internal;

                case TypeAttributes.NestedFamily:
                    return Visibility.Protected;

                case TypeAttributes.NestedPrivate:
                    return Visibility.Private;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Visibility ToVisibility(this MethodAttributes methodAttributes)
        {
            if (methodAttributes.HasFlag(MethodAttributes.Public))
            {
                return Visibility.Public;
            }

            if (methodAttributes.HasFlag(MethodAttributes.Assembly))
            {
                return Visibility.Internal;
            }

            if (methodAttributes.HasFlag(MethodAttributes.Family))
            {
                return Visibility.Protected;
            }

            if (methodAttributes.HasFlag(MethodAttributes.Private))
            {
                return Visibility.Private;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
