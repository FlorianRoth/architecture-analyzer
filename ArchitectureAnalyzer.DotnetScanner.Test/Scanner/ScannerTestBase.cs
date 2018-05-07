namespace ArchitectureAnalyzer.DotnetScanner.Test.Scanner
{
    using System.IO;
    using System.Linq;

    using ArchitectureAnalyzer.DotnetScanner.Model;

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
            return ModelFactory.GetTypeModels().FirstOrDefault(t => t.Name == typeof(T).Name && t.Namespace == typeof(T).Namespace);
        }

        protected NetMethod NetMethod<T>(string methodName)
        {
            return ModelFactory.GetMethodModels().FirstOrDefault(m => m.Name == methodName);
        }
    }
}
