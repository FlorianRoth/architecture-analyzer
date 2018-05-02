namespace TestLibrary
{
    using System;

    public abstract class ClassWithMembers
    {
        public string Property { get; set; }

        public event EventHandler Event;

        public void SomeMethod()
        {

        }

        internal void InternalMethod()
        {

        }

        public static void StaticMethod()
        {
        }

        public abstract void AbstractMethod();

        public static bool operator==(ClassWithMembers a, ClassWithMembers b)
        {
            return false;
        }

        public static bool operator!=(ClassWithMembers a, ClassWithMembers b)
        {
            return false;
        }
    }
}
