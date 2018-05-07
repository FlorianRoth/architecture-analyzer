namespace ArchitectureAnalyzer.DotnetScanner.Test.Scanner
{
    using System.IO;

    using ArchitectureAnalyzer.Core.Graph;
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

        protected static NetAssembly NetAssembly(string id)
        {
            return new NetAssembly { Id = id };
        }

        protected static NetType NetType<T>()
        {
            return new NetType { Id = typeof(T).FullName };
        }

        protected static NetMethod NetMethod<T>(string methodName)
        {
            return new NetMethod { Id = typeof(T).FullName + "." + methodName };
        }
    }
}
