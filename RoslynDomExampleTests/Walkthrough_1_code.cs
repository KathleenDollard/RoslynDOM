using System;

namespace RoslynDom.Tests.Walkthrough_1_code
{

    public class Bar
    {
        private string firstName;
        private string lastName;

        public string Foo()
        {
            var ret = lastName;
            ret = Foo();
            ret = "xyz";
            var xx = new String('a', 4);
            ret = "abc" + Foo();

            if (!string.IsNullOrEmpty(firstName))
            { ret = firstName + lastName; }
            var x = ", ";
            uint y = 42;
            x = lastName + x + firstName;
            Foo2(x);
            return x;
        }

        private void Foo2(string dummy)
        {
            ulong x = 3;
            Console.WriteLine("Making up silly code to evaluate");
        }

        public string FooBar
        {
            get
            {
                ushort z = 432;
                return z.ToString();
            }
        }

        public void Foo3(string dummy)
        {
            Console.WriteLine("Making up silly code to evaluate");
        }
    }
}
