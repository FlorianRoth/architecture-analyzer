namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using System;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class MethodScannerTest : MetadataScannerTestBase
    {
        private MethodScanner _scanner;

        [SetUp]
        public void SetupScanner()
        {
            _scanner = new MethodScanner(MetadataReader, ModelFactory, Logger);
        }
        
        [Test]
        public void NameIsCorrect()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            Assert.That(model.Name, Is.EqualTo(nameof(ClassWithMembers.SomeMethod)));
        }

        [Test]
        public void ReturnTypeIsCorrect()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.IntMethod));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            Assert.That(model.ReturnType.Name, Is.EqualTo(nameof(Int32)));
        }

        [Test]
        public void ParamterTypesAreCorrect()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.MethodWithParams));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());
            
            Assert.That(model.ParameterTypes.Count, Is.EqualTo(2));

            Assert.That(model.ParameterTypes[0].Name, Is.EqualTo(nameof(Int32)));
            Assert.That(model.ParameterTypes[1].Name, Is.EqualTo(nameof(String)));
        }

        [Test]
        public void IsStaticFlagIsSetForStaticMethod()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.StaticMethod));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            Assert.That(model.IsStatic, Is.True);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
        }

        [Test]
        public void IsAbstractFlagIsSetForAbstractMethod()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.AbstractMethod));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.True);
            Assert.That(model.IsSealed, Is.False);
        }
        
        [Test]
        public void IsSealedFlagIsSetForSealedMethod()
        {
            var method = GetMethodDefinition<InheritedFromClassWithMembers>(nameof(InheritedFromClassWithMembers.AbstractMethod));

            var model = _scanner.ScanMethod(method, NetType<InheritedFromClassWithMembers>());

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.True);
        }
    }
}
