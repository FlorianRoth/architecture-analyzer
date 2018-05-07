namespace ArchitectureAnalyzer.DotnetScanner.Test.Scanner
{
    using ArchitectureAnalyzer.DotnetScanner.Scanner;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class TypeScannerTest : MetadataScannerTestBase
    {
        private TypeScanner _scanner;

        [SetUp]
        public void SetupScanner()
        {
            _scanner = new TypeScanner(MetadataReader, ModelFactory, Logger);
        }

        [Test]
        public void IdIsCorrect()
        {
            var type = GetTypeDefintion(typeof(EmptyClass));

            var model = _scanner.ScanType(type);

            Assert.That(model.Id, Is.EqualTo(typeof(EmptyClass).FullName));
        }

        [Test]
        public void NameIsCorrect()
        {
            var type = GetTypeDefintion(typeof(EmptyClass));

            var model = _scanner.ScanType(type);

            Assert.That(model.Name, Is.EqualTo(nameof(EmptyClass)));
        }

        [Test]
        public void NamespaceIsCorrect()
        {
            var type = GetTypeDefintion(typeof(EmptyClass));

            var model = _scanner.ScanType(type);

            Assert.That(model.Namespace, Is.EqualTo(typeof(EmptyClass).Namespace));
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
