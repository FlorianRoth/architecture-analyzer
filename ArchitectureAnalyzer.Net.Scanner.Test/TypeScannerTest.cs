namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using ArchitectureAnalyzer.Net.Model;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class TypeScannerTest : MetadataScannerTestBase
    {
        private TypeScanner _scanner;

        private NetAssembly _assembly;

        [SetUp]
        public void SetupScanner()
        {
            _assembly = NetAssembly("TestLibrary");
            _scanner = new TypeScanner(Module, ModelFactory, Logger);
        }
        
        [Test]
        public void NameIsCorrect()
        {
            var type = GetTypeDefintion(typeof(EmptyClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Name, Is.EqualTo(nameof(EmptyClass)));
        }

        [Test]
        public void NameIsCorrectForGenericType()
        {
            var type = GetTypeDefintion(typeof(GenericClass<>));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Name, Is.EqualTo(typeof(GenericClass<>).Name));
        }

        [Test]
        public void NamespaceIsCorrect()
        {
            var type = GetTypeDefintion(typeof(EmptyClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Namespace, Is.EqualTo(typeof(EmptyClass).Namespace));
        }

        [Test]
        public void AssemblyIsCorrect()
        {
            var type = GetTypeDefintion(typeof(EmptyClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Assembly, Is.EqualTo(_assembly));
        }

        [Test]
        public void BaseTypeIsCorrect()
        {
            var type = GetTypeDefintion(typeof(InheritedClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.BaseType, Is.EqualTo(NetType<EmptyClass>()));
        }

        [Test]
        public void NoFlagIsSetForNormalClass()
        {
            var type = GetTypeDefintion(typeof(EmptyClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.HasAttributes, Is.False);
            Assert.That(model.IsGeneric, Is.False);
        }

        [Test]
        public void IsStaticFlagIsSetForStaticClass()
        {
            var type = GetTypeDefintion(typeof(StaticClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.IsStatic, Is.True);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.HasAttributes, Is.False);
            Assert.That(model.IsGeneric, Is.False);
        }

        [Test]
        public void IsAbstractFlagIsSetForAbstractClass()
        {
            var type = GetTypeDefintion(typeof(AbstractClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.True);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.HasAttributes, Is.False);
            Assert.That(model.IsGeneric, Is.False);
        }

        [Test]
        public void IsSealedFlagIsSetForSealedClass()
        {
            var type = GetTypeDefintion(typeof(SealedClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.True);
            Assert.That(model.HasAttributes, Is.False);
            Assert.That(model.IsGeneric, Is.False);
        }

        [Test]
        public void HasAttributeFlagIsSetForUserTypeAttributedClass()
        {
            var type = GetTypeDefintion(typeof(UserTypeAttributedClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.HasAttributes, Is.True);
            Assert.That(model.IsGeneric, Is.False);
        }

        [Test]
        public void HasAttributeFlagIsSetForTestFixtureClass()
        {
            var type = GetTypeDefintion(typeof(TestFixtureAttributedClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.IsGeneric, Is.False);
            Assert.That(model.HasAttributes, Is.True);
        }

        [Test]
        public void IsGenericFlagIsSetForGenericClass()
        {
            var type = GetTypeDefintion(typeof(GenericClass<>));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.IsGeneric, Is.True);
            Assert.That(model.HasAttributes, Is.False);
        }

        [Test]
        public void TypeIsClassForClass()
        {
            var type = GetTypeDefintion<EmptyClass>();

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Type, Is.EqualTo(Net.Model.NetType.TypeClass.Class));
        }

        [Test]
        public void TypeIsInterfaceForInterface()
        {
            var type = GetTypeDefintion<IInterface>();

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Type, Is.EqualTo(Net.Model.NetType.TypeClass.Interface));
        }

        [Test]
        public void TypeIsEnumForEnum()
        {
            var type = GetTypeDefintion<SomeEnum>();

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Type, Is.EqualTo(Net.Model.NetType.TypeClass.Enum));
        }

        [Test]
        public void GenericTypeArgsAreSetForGenericClass()
        {
            var type = GetTypeDefintion(typeof(GenericClass<>));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.GenericTypeArgs.Count, Is.EqualTo(1));

            Assert.That(model.GenericTypeArgs[0].Name, Is.EqualTo("TTypeArg"));
            Assert.That(model.GenericTypeArgs[0].Type, Is.EqualTo(Net.Model.NetType.TypeClass.GenericTypeArg));
        }

        [Test]
        public void VisibilityIsPublicForPublicClass()
        {
            var type = GetTypeDefintion(typeof(EmptyClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Visibility, Is.EqualTo(Visibility.Public));
        }

        [Test]
        public void VisibilityIsInternalForInternalClass()
        {
            var type = GetTypeDefintion(typeof(InternalClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Visibility, Is.EqualTo(Visibility.Internal));
        }

        [Test]
        public void VisibilityIsInternalForDefaultVisiblityClass()
        {
            var type = GetTypeDefintion(typeof(DefaultVisiblityClass));

            var model = _scanner.ScanType(type, _assembly);

            Assert.That(model.Visibility, Is.EqualTo(Visibility.Internal));
        }
    }
}
