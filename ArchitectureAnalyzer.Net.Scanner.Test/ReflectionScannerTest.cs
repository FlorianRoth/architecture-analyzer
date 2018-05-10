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

            AssertNodeInDatabase(
                NetAssembly("TestLibrary"));
        }

        [Test]
        public void InheritedClassIsLinkedToBaseClass()
        {
            _scanner.Scan();
            
            AssertRelationshipInDatabase(
                NetType<InheritedClass>(),
                NetType<EmptyClass>(),
                Relationship.EXTENDS);
        }

        [Test]
        public void ImplementedInterfaceIsLinkedToClass()
        {
            _scanner.Scan();

            AssertRelationshipInDatabase(
                NetType<ImplementsInterface>(),
                NetType<IInterface>(),
                Relationship.IMPLEMENTS);
        }

        [Test]
        public void UserDefinedAttributeIsLinkedToClass()
        {
            _scanner.Scan();

            AssertRelationshipInDatabase(
                NetType<UserTypeAttributedClass>(),
                NetType<UserDefinedAttribute>(),
                Relationship.HAS_ATTRIBUTE);
        }

        [Test]
        public void TestFixtureAttributeIsLinkedToClass()
        {
            _scanner.Scan();

            AssertRelationshipInDatabase(
                NetType<TestFixtureAttributedClass>(),
                NetType<TestFixtureAttribute>(),
                Relationship.HAS_ATTRIBUTE);
        }

        [Test]
        public void InterfaceInheritedFromBaseClassIsNotLinked()
        {
            _scanner.Scan();

            AssertRelationshipInDatabase(
                NetType<InheritsInterfaceFromBaseClass>(),
                NetType<ImplementsInterface>(),
                Relationship.EXTENDS);

            AssertNoRelationshipInDatabase(
                NetType<InheritsInterfaceFromBaseClass>(),
                NetType<IInterface>(),
                Relationship.IMPLEMENTS);
        }

        [Test]
        public void InterfaceInheritedFromBaseInterfaceIsNotLinked()
        {
            _scanner.Scan();

            AssertRelationshipInDatabase(
                NetType<IAgainExtendedInterface>(),
                NetType<IExtendedInterface>(),
                Relationship.IMPLEMENTS);

            AssertNoRelationshipInDatabase(
                NetType<IAgainExtendedInterface>(),
                NetType<IInterface>(),
                Relationship.IMPLEMENTS);
        }

        [Test]
        public void PublicClassIsAddedToDatabase()
        {
            _scanner.Scan();

            AssertNodeInDatabase(
                NetType<EmptyClass>());
        }

        [Test]
        public void InternalClassIsNotAddedToDatabase()
        {
            _scanner.Scan();

            AssertNoNodeInDatabase(
                NetType<InternalClass>());
        }

        [Test]
        public void MethodIsLinkedToDeclaringClass()
        {
            _scanner.Scan();

            AssertRelationshipInDatabase(
                NetType<ClassWithMembers>(),
                NetMethod<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod)),
                Relationship.DEFINES_METHOD);
        }

        [Test]
        public void OnlyDeclaredMethodsAreProcessed()
        {
            _scanner.Scan();

            AssertNoRelationshipInDatabase(
                NetType<InheritedFromClassWithMembers>(),
                NetMethod<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod)),
                Relationship.DEFINES_METHOD);
        }

        [Test]
        public void PublicMethodIsAddedToDatabase()
        {
            _scanner.Scan();

            AssertNodeInDatabase(
                NetMethod<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod)));
        }

        [Test]
        public void InternalMethodIsNotAddedToDatabase()
        {
            _scanner.Scan();

            AssertNoNodeInDatabase(
                NetMethod<ClassWithMembers>(nameof(ClassWithMembers.InternalMethod)));
        }

        [Test]
        public void ConstructorIsAddedToDatabase()
        {
            _scanner.Scan();
            
            AssertNodeInDatabase(
                NetMethod<InheritedFromClassWithMembers>(".ctor"));
        }

        [TestCase("get_")]
        [TestCase("set_")]
        public void PropertyAccessorIsNotAddedToDatabase(string accessor)
        {
            var methodName = accessor + nameof(ClassWithMembers.Property);
            
            _scanner.Scan();

            AssertNoNodeInDatabase(
                NetMethod<ClassWithMembers>(methodName));
        }

        [TestCase("add_")]
        [TestCase("remove_")]
        public void EventAccessorIsNotAddedToDatabase(string accessor)
        {
            var methodName = accessor + nameof(ClassWithMembers.Event);

            _scanner.Scan();

            AssertNoNodeInDatabase(
                NetMethod<ClassWithMembers>(methodName));
        }

        [TestCase("op_Equality")]
        [TestCase("op_Inequality")]
        public void OperatorIsAddedToDatabase(string methodName)
        {
            _scanner.Scan();

            AssertNodeInDatabase(
                NetMethod<ClassWithMembers>(methodName));
        }

        [Test]
        public void GenericTypeArgumentIsLinked()
        {
            _scanner.Scan();

            var from = NetType(typeof(GenericClass<>));

            AssertRelationshipInDatabase(
                NetType(typeof(GenericClass<>)),
                NetType(typeof(GenericClass<>), "TTypeArg"),
                Relationship.DEFINES_TYPE_ARG);
        }

        private void AssertNodeInDatabase<TNode>(TNode node) where TNode : Node
        {
            A.CallTo(() => _database.CreateNode(node)).MustHaveHappened(Repeated.Exactly.Once);
        }

        private void AssertNoNodeInDatabase<TNode>(TNode node) where TNode : Node
        {
            A.CallTo(() => _database.CreateNode(node)).MustNotHaveHappened();
        }
        
        private void AssertRelationshipInDatabase<TFrom, TTo>(TFrom from, TTo to, string relationship) where TFrom : Node where TTo : Node
        {
            A.CallTo(() => _database.CreateRelationship(from, to, relationship)).MustHaveHappened(Repeated.Exactly.Once);
        }

        private void AssertNoRelationshipInDatabase<TFrom, TTo>(TFrom from, TTo to, string relationship) where TFrom : Node where TTo : Node
        {
            A.CallTo(() => _database.CreateRelationship(from, to, relationship)).MustNotHaveHappened();
        }
    }
}
