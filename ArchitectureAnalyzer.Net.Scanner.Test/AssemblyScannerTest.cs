namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using System.Linq;
    using System.Reflection.Metadata;

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
            _scanner = new AssemblyScanner(MetadataReader, ModelFactory, Logger);
        }
        
        [Test]
        public void NameIsCorrect()
        {
            var assemblyModel = _scanner.Scan(Assembly);
            
            Assert.That(assemblyModel.Name, Is.EqualTo("TestLibrary"));
        }

        [Test]
        public void DefinedTypesAreCorrect()
        {
            var assemblyModel = _scanner.Scan(Assembly);

            var expectedTypes = new[]
                                    {
                                        nameof(AbstractClass),
                                        nameof(ClassWithMembers),
                                        nameof(EmptyClass),
                                        nameof(IAgainExtendedInterface),
                                        nameof(IExtendedInterface),
                                        nameof(IInterface),
                                        nameof(ImplementsInterface),
                                        nameof(InheritedClass),
                                        nameof(InheritedFromClassWithMembers),
                                        nameof(InheritsInterfaceFromBaseClass),
                                        nameof(SealedClass),
                                        nameof(SomeEnum),
                                        nameof(StaticClass)
                                    };

            Assert.That(assemblyModel.DefinedTypes.Select(t => t.Name), Is.EquivalentTo(expectedTypes));
        }
    }
}
