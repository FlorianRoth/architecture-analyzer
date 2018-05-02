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

        protected IGraphDatabase Database { get; private set; }

        protected IModelFactory ModelFactory { get; private set; }

        protected string AssemblyPath =>  Path.Combine(TestContext.CurrentContext.TestDirectory, "TestLibrary.dll");

        [SetUp]
        public void SetUp()
        {
            Logger = A.Fake<ILogger>();
            Database = A.Fake<IGraphDatabase>();
            ModelFactory = new ModelFactory();
        }
        
        protected void AssertCreateTypeNode<T>()
        {
            A.CallTo(() => Database.CreateNode(NetType<T>())).MustHaveHappened(Repeated.Exactly.Once);
        }

        protected void AssertNoTypeNode<T>()
        {
            A.CallTo(() => Database.CreateNode(NetType<T>())).MustNotHaveHappened();
        }

        protected void AssertCreateMethodNode<T>(string methodName)
        {
            A.CallTo(() => Database.CreateNode(NetMethod<T>(methodName))).MustHaveHappened(Repeated.Exactly.Once);
        }

        protected void AssertNoMethodNode<T>(string methodName)
        {
            A.CallTo(() => Database.CreateNode(NetMethod<T>(methodName))).MustNotHaveHappened();
        }

        protected void AssertTypeRelationShip<TFrom, TTo>(string relationship)
        {
            A.CallTo(() => Database.CreateRelationship(NetType<TFrom>(), NetType<TTo>(), relationship)).MustHaveHappened(Repeated.Exactly.Once);
        }

        protected void AssertNoTypeRelationShip<TFrom, TTo>(string relationship)
        {
            A.CallTo(() => Database.CreateRelationship(NetType<TFrom>(), NetType<TTo>(), relationship)).MustNotHaveHappened();
        }

        protected static NetAssembly NetAssembly(string id)
        {
            return new NetAssembly(id);
        }

        protected static NetType NetType<T>()
        {
            return new NetType(typeof(T).FullName);
        }

        protected static NetMethod NetMethod<T>(string methodName)
        {
            return new NetMethod(typeof(T).FullName + "." + methodName);
        }
    }
}
