using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslynDom
{
    namespace Common.Test
    {
                public class Foo { }
    }
}


namespace RoslynDom
{
    namespace Common
    {
        namespace Test
        {
          //  public class Foo { }
        }
    }
}

namespace RoslynDom.Common.Test
{
    //public class Foo { }
}

namespace RoslynDomTests
{

    public class SomeAttr : Attribute { }

    public class SomeAttr2 : Attribute { }

    [SomeAttr, SomeAttr2]
    struct Foo<T>
    {
        public void FooBar()
        {
        }
    }

    class PlaySpace
    {
        [SomeAttr()]
        private string Foo2;

        public void Foo()
        {
        }

        public string Bar
        {
            [SomeAttr()]
            get
            {
                return "";
            }
        }
    }

    class C
    {
        public string GetName(int value)
        {
            var ret = "";
            if (value == 1)
            {
                C2.Foo();
                ret = "Fred";
            }
            else if (value == 2)
            {
                ret = "George";
            }
            else
            {
                ret = "Percy";
            }
            return ret;
        }
    }

    class C2
    {
        public static void Foo()
        {
            #region 
            try
            { }
            catch (Exception ex)
            { }
            finally
            { }
            #endregion
        }
    }
}
