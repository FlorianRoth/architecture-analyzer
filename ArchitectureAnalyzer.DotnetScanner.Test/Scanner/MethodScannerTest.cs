namespace ArchitectureAnalyzer.DotnetScanner.Test.Scanner
{
    using ArchitectureAnalyzer.DotnetScanner.Scanner;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class MethodScannerTest : MetadataScannerTestBase
    {
        private MethodScanner _scanner;

        [SetUp]
        public void SetupScanner()
        {
            _scanner = new MethodScanner(MetadataReader, ModelFactory, Database, Logger);
        }

        [Test]
        public void PublicMethodIsAddedToDatabase()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod));

            _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            AssertCreateMethodNode<ClassWithMembers>(nameof(ClassWithMembers.SomeMethod));
        }

        [Test]
        public void InternalMethodIsNotAddedToDatabase()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.InternalMethod));

            _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            AssertNoMethodNode<ClassWithMembers>(nameof(ClassWithMembers.InternalMethod));
        }

        [Test]
        public void ConstructorIsAddedToDatabase()
        {
            var methodName = ".ctor";

            var method = GetMethodDefinition<InheritedFromClassWithMembers>(methodName);

            _scanner.ScanMethod(method, NetType<InheritedFromClassWithMembers>());

            AssertCreateMethodNode<InheritedFromClassWithMembers>(methodName);
        }

        [TestCase("get_")]
        [TestCase("set_")]
        public void PropertyAccessorIsNotAddedToDatabase(string accessor)
        {
            var methodName = accessor + nameof(ClassWithMembers.Property);

            var method = GetMethodDefinition<ClassWithMembers>(methodName);

            _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            AssertNoMethodNode<ClassWithMembers>(methodName);
        }

        [TestCase("add_")]
        [TestCase("remove_")]
        public void EventAccessorIsNotAddedToDatabase(string accessor)
        {
            var methodName = accessor + nameof(ClassWithMembers.Event);

            var method = GetMethodDefinition<ClassWithMembers>(methodName);

            _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            AssertNoMethodNode<ClassWithMembers>(methodName);
        }

        [TestCase("op_Equality")]
        [TestCase("op_Inequality")]
        public void OperatorIsAddedToDatabase(string methodName)
        {
            var method = GetMethodDefinition<ClassWithMembers>(methodName);

            _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            AssertCreateMethodNode<ClassWithMembers>(methodName);
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
