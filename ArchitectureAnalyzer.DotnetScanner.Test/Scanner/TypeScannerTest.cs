namespace ArchitectureAnalyzer.DotnetScanner.Test.Scanner
{
    using ArchitectureAnalyzer.DotnetScanner.Scanner;

    using FakeItEasy;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class TypeScannerTest : MetadataScannerTestBase
    {
        private TypeScanner _scanner;

        [SetUp]
        public void SetupScanner()
        {
            _scanner = new TypeScanner(MetadataReader, ModelFactory, Database, Logger);
        }

        [Test]
        public void PublicClassIsAddedToDatabase()
        {
            var type = GetTypeDefintion<EmptyClass>();

            _scanner.ScanType(type);

            AssertCreateTypeNode<EmptyClass>();
        }

        [Test]
        public void InternalClassIsNotAddedToDatabase()
        {
            var type = GetTypeDefintion<InternalClass>();

            _scanner.ScanType(type);

            AssertNoTypeNode<InternalClass>();
        }

        [Test]
        public void MethodIsLinkedToDeclaringClass()
        {
            var type = GetTypeDefintion<ClassWithMembers>();

            _scanner.ScanType(type);
            
            A.CallTo(() => Database.CreateRelationship(NetType<ClassWithMembers>(), NetMethod<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod)), Relationship.DEFINES_METHOD)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void OnlyDeclaredMethodsAreProcessed()
        {
            var type = GetTypeDefintion<InheritedFromClassWithMembers>();

            _scanner.ScanType(type);

            A.CallTo(() => Database.CreateRelationship(NetType<InheritedFromClassWithMembers>(), NetMethod<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod)), Relationship.DEFINES_METHOD)).MustNotHaveHappened();
        }
        
        [Test]
        public void NoFlagIsSetForNormalClass()
        {
            var type = GetTypeDefintion(typeof(EmptyClass));

            var model = _scanner.ScanType(type);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.HasAttribute, Is.False);
        }

        [Test]
        public void IsStaticFlagIsSetForStaticClass()
        {
            var type = GetTypeDefintion(typeof(StaticClass));

            var model = _scanner.ScanType(type);

            Assert.That(model.IsStatic, Is.True);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.HasAttribute, Is.False);
        }

        [Test]
        public void IsAbstractFlagIsSetForAbstractClass()
        {
            var type = GetTypeDefintion(typeof(AbstractClass));

            var model = _scanner.ScanType(type);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.True);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.HasAttribute, Is.False);
        }

        [Test]
        public void IsSealedFlagIsSetForSealedClass()
        {
            var type = GetTypeDefintion(typeof(SealedClass));

            var model = _scanner.ScanType(type);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.True);
            Assert.That(model.HasAttribute, Is.False);
        }

        [Test]
        public void HasAttributeFlagIsSetForUserTypeAttributedClass()
        {
            var type = GetTypeDefintion(typeof(UserTypeAttributedClass));

            var model = _scanner.ScanType(type);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.HasAttribute, Is.True);
        }

        [Test]
        public void HasAttributeFlagIsSetForTestFixtureClass()
        {
            var type = GetTypeDefintion(typeof(TestFixtureAttributedClass));

            var model = _scanner.ScanType(type);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.HasAttribute, Is.True);
        }

        [Test]
        public void TypeIsClassForClass()
        {
            var type = GetTypeDefintion<EmptyClass>();

            var model = _scanner.ScanType(type);

            Assert.That(model.Type, Is.EqualTo(Model.NetType.TypeClass.Class));
        }

        [Test]
        public void TypeIsInterfaceForInterface()
        {
            var type = GetTypeDefintion<IInterface>();

            var model = _scanner.ScanType(type);

            Assert.That(model.Type, Is.EqualTo(Model.NetType.TypeClass.Interface));
        }

        [Test]
        public void TypeIsEnumForEnum()
        {
            var type = GetTypeDefintion<SomeEnum>();

            var model = _scanner.ScanType(type);

            Assert.That(model.Type, Is.EqualTo(Model.NetType.TypeClass.Enum));
        }
    }
}
