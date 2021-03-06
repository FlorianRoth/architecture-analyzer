﻿namespace TestLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class ClassWithMembers
    {
        public override bool Equals(object obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public string Property { get; set; }

        public int InternalProperty { get; set; }

        public event EventHandler Event;

        public void SomeMethod()
        {

        }

        public int IntMethod()
        {
            return 0;
        }

        public void MethodWithParams(int a, string b)
        {
        }

        public void PublicMethod()
        {
        }

        internal void InternalMethod()
        {
        }
    
        protected void ProtectedMethod()
        {
        }

        private void PrivateMethod()
        {
        }

        void DefaultVisibilityMethod()
        {
        }

        public static void StaticMethod()
        {
        }

        public abstract void AbstractMethod();

        public void GenericMethod<TMethodArg>()
        {
        }

        public void GenericMethodArg<TMethodArg>(TMethodArg arg)
        {
        }

        public IEnumerable<string> ReturnTypeIsGenericTypeInstantiation()
        {
            return Enumerable.Empty<string>();
        }

        public static bool operator==(ClassWithMembers a, ClassWithMembers b)
        {
            return false;
        }

        public static bool operator!=(ClassWithMembers a, ClassWithMembers b)
        {
            return false;
        }

        private void OnEvent()
        {
            Event?.Invoke(this, EventArgs.Empty);
        }
    }
}
