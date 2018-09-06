namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using Mono.Cecil;

    using NUnit.Framework;

    public abstract class ScannerTestBase
    {
        protected ILogger Logger { get; private set; }

        protected IModelFactory ModelFactory { get; private set; }

        protected string AssemblyPath =>  Path.Combine(TestContext.CurrentContext.TestDirectory, "TestLibrary.dll");

        [SetUp]
        public void SetUp()
        {
            Logger = A.Fake<ILogger>();
            ModelFactory = new ModelFactory();
        }

        protected NetAssembly NetAssembly(string name)
        {
            return ModelFactory.GetAssemblyModels().FirstOrDefault(a => a.Name == name);
        }

        protected NetType NetType<T>()
        {
            return NetType(typeof(T));
        }

        protected NetType NetType(Type type)
        {
            var typeName = CreateFullName(type);

            return ModelFactory.GetTypeModels().FirstOrDefault(t => Equals(t.FullName, typeName));
        }

        private static string CreateFullName(Type type)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(type.Namespace))
            {
                builder.Append(type.Namespace);
                builder.Append(".");
            }

            builder.Append(type.Name);
            if (type.IsGenericType && !type.ContainsGenericParameters)
            {
                builder.Append("<");
                builder.AppendJoin(",", type.GenericTypeArguments.Select(CreateFullName));
                builder.Append(">");
            }

           return builder.ToString();
        }

        protected NetType NetType(Type type, string typeArgName)
        {
            return NetType(type).GenericTypeArgs.First(t => t.Name == typeArgName);
        }

        protected NetType NetType<T>(string methodName, string typeArgName)
        {
            return NetMethod<T>(methodName).GenericParameters.First(t => t.Name == typeArgName);
        }

        protected NetMethod NetMethod<T>(string methodName)
        {
            return ModelFactory.GetMethodModels().FirstOrDefault(m => Equals(m.DeclaringType, NetType<T>()) && m.Name == methodName);
        }

        protected NetMethodParameter NetMethodParameter<T>(string methodName, string parameterName)
        {
            var method = NetMethod<T>(methodName);

            return ModelFactory.GetMethodParameterModels().FirstOrDefault(p => Equals(p.DeclaringMethod, method) && p.Name == parameterName);
        }

        protected NetProperty NetProperty<T>(string propertyName)
        {
            return ModelFactory.GetPropertyModels().FirstOrDefault(m => Equals(m.DeclaringType, NetType<T>()) && m.Name == propertyName);
        }
    }
}
