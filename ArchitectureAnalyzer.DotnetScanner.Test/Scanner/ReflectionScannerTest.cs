namespace ArchitectureAnalyzer.DotnetScanner.Test.Scanner
{
    using ArchitectureAnalyzer.DotnetScanner.Scanner;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class ReflectionScannerTest : ScannerTestBase
    {
        private ReflectionScanner _scanner;
        
        [SetUp]
        public void SetupScanner()
        {
            var config = new ReflectionScannerConfiguration { Assemblies = new[] { AssemblyPath } };
            _scanner = new ReflectionScanner(config, ModelFactory, Database, A.Fake<ILogger<ReflectionScanner>>());
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
    }
}
