namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using System;
    using System.IO;
    using System.Linq;

    using Mono.Cecil;

    using NUnit.Framework;

    public abstract class MetadataScannerTestBase : ScannerTestBase
    {
        private FileStream _stream;
        
        protected ModuleDefinition Module { get; private set; }

        [SetUp]
        public void SetUpMetadataReader()
        {
            _stream = File.OpenRead(AssemblyPath);

            Module = ModuleDefinition.ReadModule(_stream);
        }

        [TearDown]
        public void TearDownMetadataReader()
        {
            Module.Dispose();
            _stream.Dispose();
        }

        protected TypeDefinition GetTypeDefintion(Type type)
        {
            return Module.Types
                .First(t => t.Name == type.Name);
        }

        protected TypeDefinition GetTypeDefintion<T>()
        {
            return GetTypeDefintion(typeof(T));
        }

        protected MethodDefinition GetMethodDefinition<T>(string methodName)
        {
            return GetTypeDefintion<T>().Methods.First(m => m.Name == methodName);
        }

        protected PropertyDefinition GetPropertyDefinition<T>(string propertyName)
        {
            return GetTypeDefintion<T>().Properties.First(property => property.Name == propertyName);
        }
    }
}
