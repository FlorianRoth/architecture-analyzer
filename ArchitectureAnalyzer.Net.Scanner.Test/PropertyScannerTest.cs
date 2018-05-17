﻿
namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using System;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class PropertyScannerTest : MetadataScannerTestBase
    {
        private PropertyScanner _scanner;

        [SetUp]
        public void SetupScanner()
        {
            _scanner = new PropertyScanner(MetadataReader, ModelFactory, Logger);

            var typeScanner = new TypeScanner(MetadataReader, ModelFactory, Logger);
            typeScanner.ScanType(GetTypeDefintion<ClassWithMembers>());
            typeScanner.ScanType(GetTypeDefintion<InheritedFromClassWithMembers>());
        }

        [Test]
        public void NameIsCorrect()
        {
            var property = GetPropertyDefinition<ClassWithMembers>(nameof(ClassWithMembers.Property));

            var model = _scanner.ScanProperty(property, NetType<ClassWithMembers>());

            Assert.That(model.Name, Is.EqualTo(nameof(ClassWithMembers.Property)));
        }

        [Test]
        public void ReturnTypeIsCorrect()
        {
            var property = GetPropertyDefinition<ClassWithMembers>(nameof(ClassWithMembers.Property));

            var model = _scanner.ScanProperty(property, NetType<ClassWithMembers>());

            Assert.That(model.Type.Name, Is.EqualTo(nameof(String)));
        }
    }
}
