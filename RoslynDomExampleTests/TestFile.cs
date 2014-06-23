using System.Diagnostics.Tracing;
using System;
using System.Linq;

namespace testing
{
    namespace Namespace1
    { }
}

    namespace testing.Namespace1
{
    public class FooClass
    {
        public string FooField = "Bar";
        public string FooMethod(int bar1, string bar2) { return "42"; }
        public string FooProperty { get; set; }
    }

    public class BarClass
    {   }


    public class FooStruct
    {    }

    public class FooInterface
    {    }
}
                        
