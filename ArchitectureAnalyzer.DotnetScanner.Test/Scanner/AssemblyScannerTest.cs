namespace ArchitectureAnalyzer.DotnetScanner.Test.Scanner
{
    using System.Reflection.Metadata;

    using ArchitectureAnalyzer.DotnetScanner.Scanner;

    using FakeItEasy;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class AssemblyScannerTest : MetadataScannerTestBase
    {
        private AssemblyScanner _scanner;

        private AssemblyDefinition Assembly => MetadataReader.GetAssemblyDefinition();

        [SetUp]
        public void SetupScanner()
        {
            _scanner = new AssemblyScanner(MetadataReader, ModelFactory, Database, Logger);
        }

        [Test]
        public void AssemblyIsAddedToDatabase()
        {
            _scanner.Scan(Assembly);
            
            A.CallTo(() => Database.CreateNode(NetAssembly("TestLibrary"))).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
