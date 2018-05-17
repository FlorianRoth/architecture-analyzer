namespace TestLibrary
{
    using System;

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

        internal void InternalMethod()
        {

        }

        public static void StaticMethod()
        {
        }

        public abstract void AbstractMethod();

        public void GenericMethod<TMethodArg>()
        {
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
