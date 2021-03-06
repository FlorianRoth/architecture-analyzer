﻿namespace ArchitectureAnalyzer.Net.Scanner.Test
{
    using System;
    using System.Collections.Generic;

    using ArchitectureAnalyzer.Net.Model;

    using NUnit.Framework;

    using TestLibrary;

    [TestFixture]
    public class MethodScannerTest : MetadataScannerTestBase
    {
        private MethodScanner _scanner;
        
        [SetUp]
        public void SetupScanner()
        {
            _scanner = new MethodScanner(Module, ModelFactory, Logger);

            var assembly = new NetAssembly { Name = "TestLibrary" };

            var typeScanner = new TypeScanner(Module, ModelFactory, Logger);
            typeScanner.ScanType(GetTypeDefintion<ClassWithMembers>(), assembly);
            typeScanner.ScanType(GetTypeDefintion<InheritedFromClassWithMembers>(), assembly);
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
            
            Assert.That(model.Parameters.Count, Is.EqualTo(2));

            Assert.That(model.Parameters[0].Name, Is.EqualTo("a"));
            Assert.That(model.Parameters[0].Order, Is.EqualTo(1));
            Assert.That(model.Parameters[0].Type.Name, Is.EqualTo(nameof(Int32)));

            Assert.That(model.Parameters[1].Name, Is.EqualTo("b"));
            Assert.That(model.Parameters[1].Order, Is.EqualTo(2));
            Assert.That(model.Parameters[1].Type.Name, Is.EqualTo(nameof(String)));
        }
        
        [Test]
        public void GenericParametersAreCorrect()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.GenericMethod));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            Assert.That(model.GenericParameters.Count, Is.EqualTo(1));
            
            Assert.That(model.GenericParameters[0].Name, Is.EqualTo("TMethodArg"));
            Assert.That(model.GenericParameters[0].Type, Is.EqualTo(Net.Model.NetType.TypeClass.GenericTypeArg));
        }

        [Test]
        public void GenericMethodArgIsCorrect()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.GenericMethodArg));
            
            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            Assert.That(model.Parameters.Count, Is.EqualTo(1));

            Assert.That(model.Parameters[0].Name, Is.EqualTo("arg"));
            Assert.That(model.Parameters[0].Type.Name, Is.EqualTo("TMethodArg"));
        }

        [Test]
        public void IsStaticFlagIsSetForStaticMethod()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.StaticMethod));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            Assert.That(model.IsStatic, Is.True);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.IsGeneric, Is.False);
        }

        [Test]
        public void IsAbstractFlagIsSetForAbstractMethod()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.AbstractMethod));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.True);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.IsGeneric, Is.False);
        }
        
        [Test]
        public void IsSealedFlagIsSetForSealedMethod()
        {
            var method = GetMethodDefinition<InheritedFromClassWithMembers>(nameof(InheritedFromClassWithMembers.AbstractMethod));

            var model = _scanner.ScanMethod(method, NetType<InheritedFromClassWithMembers>());

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.True);
            Assert.That(model.IsGeneric, Is.False);
        }

        [Test]
        public void IsGenericFlagIsSetForGenericMethod()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.GenericMethod));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            Assert.That(model.IsStatic, Is.False);
            Assert.That(model.IsAbstract, Is.False);
            Assert.That(model.IsSealed, Is.False);
            Assert.That(model.IsGeneric, Is.True);
        }

        [TestCase(nameof(ClassWithMembers.PublicMethod), ExpectedResult = Visibility.Public)]
        [TestCase(nameof(ClassWithMembers.InternalMethod), ExpectedResult = Visibility.Internal)]
        [TestCase("ProtectedMethod", ExpectedResult = Visibility.Protected)]
        [TestCase("PrivateMethod", ExpectedResult = Visibility.Private)]
        [TestCase("DefaultVisibilityMethod", ExpectedResult = Visibility.Private)]
        public Visibility VisibilityIsCorrect(string methodName)
        {
            var method = GetMethodDefinition<ClassWithMembers>(methodName);

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            return model.Visibility;
        }

        [Test]
        public void GenericTypeInstantiationIsCorrect()
        {
            var method = GetMethodDefinition<ClassWithMembers>(nameof(ClassWithMembers.ReturnTypeIsGenericTypeInstantiation));

            var model = _scanner.ScanMethod(method, NetType<ClassWithMembers>());

            var returnType = model.ReturnType;

            Assert.That(returnType.IsGenericTypeInstantiation, Is.True);
            Assert.That(returnType.GenericType, Is.EqualTo(NetType(typeof(IEnumerable<>))));
            Assert.That(returnType.GenericTypeInstantiationArgs, Is.EqualTo(new[] { NetType<string>() }));
        }
    }
}
