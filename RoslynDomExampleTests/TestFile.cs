using System.Diagnostics.Tracing;
using System;
using System.Linq;

namespace testing
{
    namespace Namespace3
    {
        public class FooClass
        { }
    }
}

namespace testing.Namespace1
{
}

namespace Namespace2
{
    public class FooClass
    {
        public string FooField = "Bar";
        public string FooMethod(int bar1, string bar2)
        {
            var x = 2;
            var y = FooField + x;
            if (true)
            {
                var x2 = 2;
                var y3 = FooField + x2;
                return y3;

            }
            return "42";
        }
        public string FooProperty { get; set; }
    }

    public class BarClass
    {   }


    public struct FooStruct
    {    }

    public interface FooInterface
    {    }

    public enum FooEnum
    { One = 1, Two, Three}
}
                        
