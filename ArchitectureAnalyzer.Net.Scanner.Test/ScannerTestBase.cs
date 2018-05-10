namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using System;
    using System.IO;
    using System.Linq;

    using ArchitectureAnalyzer.Net.Model;
    using ArchitectureAnalyzer.Net.Scanner.Model;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

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
            var key = TypeKey.FromType(type);

            return ModelFactory.GetTypeModels().FirstOrDefault(t => Equals(t.GetKey(), key));
        }

        protected NetType NetType(Type type, string typeArgName)
        {
            var key = TypeKey.FromTypeArgument(type, typeArgName);

            return ModelFactory.GetTypeModels().FirstOrDefault(t => Equals(t.GetKey(), key));
        }

        protected NetType NetType<T>(string methodName, string typeArgName)
        {
            var typeKey = TypeKey.FromType<T>();
            var methodKey = new MethodKey(typeKey, methodName);

            var key = TypeKey.FromMethodParameter(methodKey, typeArgName);
            
            return ModelFactory.GetTypeModels().FirstOrDefault(t => Equals(t.GetKey(), key));
        }

        protected NetMethod NetMethod<T>(string methodName)
        {
            return ModelFactory.GetMethodModels().FirstOrDefault(m => m.Name == methodName);
        }
    }
}
