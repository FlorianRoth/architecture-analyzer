namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;

    using ArchitectureAnalyzer.Net.Scanner.Utils;

    using NUnit.Framework;

    public abstract class MetadataScannerTestBase : ScannerTestBase
    {
        private FileStream _stream;

        private PEReader _peReader;

        protected MetadataReader MetadataReader { get; private set; }

        [SetUp]
        public void SetUpMetadataReader()
        {
            _stream = File.OpenRead(AssemblyPath);
            _peReader = new PEReader(_stream);

            MetadataReader = _peReader.GetMetadataReader();
        }

        [TearDown]
        public void TearDownMetadataReader()
        {
            _peReader.Dispose();
            _stream.Dispose();
        }

        protected string GetString(StringHandle handle)
        {
            return handle.IsNil ? null : MetadataReader.GetString(handle);
        }

        protected TypeDefinition GetTypeDefintion(Type type)
        {
            return MetadataReader.TypeDefinitions
                .Select(MetadataReader.GetTypeDefinition)
                .First(t => GetString(t.Name) == type.Name);
        }

        protected TypeDefinition GetTypeDefintion<T>()
        {
            return GetTypeDefintion(typeof(T));
        }

        protected MethodDefinition GetMethodDefinition<T>(string methodName)
        {
            return MetadataReader.MethodDefinitions
                .Select(MetadataReader.GetMethodDefinition)
                .First(MatchTypeDefinition);

            bool MatchTypeDefinition(MethodDefinition method)
            {
                var declaringType = method.GetDeclaringType();
                var typeName = GetString(MetadataReader.GetTypeDefinition(declaringType).Name);

                if (typeName != typeof(T).Name)
                {
                    return false;
                }
                
                if (GetString(method.Name) != methodName)
                {
                    return false;
                }

                return true;
            }
        }

        protected PropertyDefinition GetPropertyDefinition<T>(string propertyName)
        {
            var typeDefinition = GetTypeDefintion<T>();
            
            return typeDefinition.GetProperties()
                .Select(MetadataReader.GetPropertyDefinition)
                .First(property => GetString(property.Name) == propertyName);
        }
    }
}
