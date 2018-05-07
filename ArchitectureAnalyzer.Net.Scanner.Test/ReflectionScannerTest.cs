namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using ArchitectureAnalyzer.Core.Graph;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class ReflectionScannerTest : ScannerTestBase
    {
        private ReflectionScanner _scanner;

        private IGraphDatabase _database;
        
        [SetUp]
        public void SetupScanner()
        {
            _database = A.Fake<IGraphDatabase>();

            var config = new ReflectionScannerConfiguration { Assemblies = new[] { AssemblyPath } };
            _scanner = new ReflectionScanner(config, ModelFactory, _database, A.Fake<ILogger<ReflectionScanner>>());
        }

        [Test]
        public void AssemblyIsAddedToDatabase()
        {
            _scanner.Scan();

            A.CallTo(() => _database.CreateNode(NetAssembly("TestLibrary"))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void InheritedClassIsLinkedToBaseClass()
        {
            _scanner.Scan();
            
            AssertTypeRelationShip<InheritedClass, EmptyClass>(Relationship.EXTENDS);
        }

        [Test]
        public void ImplementedInterfaceIsLinkedToClass()
        {
            _scanner.Scan();
            
            AssertTypeRelationShip<ImplementsInterface, IInterface>(Relationship.IMPLEMENTS);
        }

        [Test]
        public void UserDefinedAttributeIsLinkedToClass()
        {
            _scanner.Scan();

            AssertTypeRelationShip<UserTypeAttributedClass, UserDefinedAttribute>(Relationship.HAS_ATTRIBUTE);
        }

        [Test]
        public void TestFixtureAttributeIsLinkedToClass()
        {
            _scanner.Scan();

            AssertTypeRelationShip<TestFixtureAttributedClass, TestFixtureAttribute>(Relationship.HAS_ATTRIBUTE);
        }

        [Test]
        public void InterfaceInheritedFromBaseClassIsNotLinked()
        {
            _scanner.Scan();
            
            AssertTypeRelationShip<InheritsInterfaceFromBaseClass, ImplementsInterface>(Relationship.EXTENDS);
            AssertNoTypeRelationShip<InheritsInterfaceFromBaseClass, IInterface>(Relationship.IMPLEMENTS);
        }

        [Test]
        public void InterfaceInheritedFromBaseInterfaceIsNotLinked()
        {
            _scanner.Scan();
            
            AssertTypeRelationShip<IAgainExtendedInterface, IExtendedInterface>(Relationship.IMPLEMENTS);
            AssertNoTypeRelationShip<IAgainExtendedInterface, IInterface>(Relationship.IMPLEMENTS);
        }

        [Test]
        public void PublicClassIsAddedToDatabase()
        {
            _scanner.Scan();

            AssertCreateTypeNode<EmptyClass>();
        }

        [Test]
        public void InternalClassIsNotAddedToDatabase()
        {
            _scanner.Scan();

            AssertNoTypeNode<InternalClass>();
        }

        [Test]
        public void MethodIsLinkedToDeclaringClass()
        {
            _scanner.Scan();

            A.CallTo(() => _database.CreateRelationship(NetType<ClassWithMembers>(), NetMethod<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod)), Relationship.DEFINES_METHOD)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void OnlyDeclaredMethodsAreProcessed()
        {
            _scanner.Scan();

            A.CallTo(() => _database.CreateRelationship(NetType<InheritedFromClassWithMembers>(), NetMethod<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod)), Relationship.DEFINES_METHOD)).MustNotHaveHappened();
        }

        [Test]
        public void PublicMethodIsAddedToDatabase()
        {
            _scanner.Scan();

            AssertCreateMethodNode<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod));
        }

        [Test]
        public void InternalMethodIsNotAddedToDatabase()
        {
            _scanner.Scan();

            AssertNoMethodNode<ClassWithMembers>(nameof(ClassWithMembers.InternalMethod));
        }

        [Test]
        public void ConstructorIsAddedToDatabase()
        {
            _scanner.Scan();

            AssertCreateMethodNode<InheritedFromClassWithMembers>(".ctor");
        }

        [TestCase("get_")]
        [TestCase("set_")]
        public void PropertyAccessorIsNotAddedToDatabase(string accessor)
        {
            var methodName = accessor + nameof(ClassWithMembers.Property);
            
            _scanner.Scan();

            AssertNoMethodNode<ClassWithMembers>(methodName);
        }

        [TestCase("add_")]
        [TestCase("remove_")]
        public void EventAccessorIsNotAddedToDatabase(string accessor)
        {
            var methodName = accessor + nameof(ClassWithMembers.Event);

            _scanner.Scan();

            AssertNoMethodNode<ClassWithMembers>(methodName);
        }

        [TestCase("op_Equality")]
        [TestCase("op_Inequality")]
        public void OperatorIsAddedToDatabase(string methodName)
        {
            _scanner.Scan();

            AssertCreateMethodNode<ClassWithMembers>(methodName);
        }

        private void AssertCreateTypeNode<T>()
        {
            A.CallTo(() => _database.CreateNode(NetType<T>())).MustHaveHappened(Repeated.Exactly.Once);
        }

        private void AssertNoTypeNode<T>()
        {
            A.CallTo(() => _database.CreateNode(NetType<T>())).MustNotHaveHappened();
        }

        private void AssertCreateMethodNode<T>(string methodName)
        {
            A.CallTo(() => _database.CreateNode(NetMethod<T>(methodName))).MustHaveHappened(Repeated.Exactly.Once);
        }

        private void AssertNoMethodNode<T>(string methodName)
        {
            A.CallTo(() => _database.CreateNode(NetMethod<T>(methodName))).MustNotHaveHappened();
        }

        private void AssertTypeRelationShip<TFrom, TTo>(string relationship)
        {
            A.CallTo(() => _database.CreateRelationship(NetType<TFrom>(), NetType<TTo>(), relationship)).MustHaveHappened(Repeated.Exactly.Once);
        }

        private void AssertNoTypeRelationShip<TFrom, TTo>(string relationship)
        {
            A.CallTo(() => _database.CreateRelationship(NetType<TFrom>(), NetType<TTo>(), relationship)).MustNotHaveHappened();
        }
    }
}
