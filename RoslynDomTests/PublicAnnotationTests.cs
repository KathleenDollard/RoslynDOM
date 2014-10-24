using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class PublicAnnotationTests
    {
        private const string PublicAnnotationCategory = "PublicAnnotations";

      #region public annotations 
      [TestMethod, TestCategory(PublicAnnotationCategory)]
      public void Get_public_annotations()
      {
         var csharpCode = @"
            //[[ root:kad_Test1(""Fred"", val2 = 42) ]]
            //[[ file:kad_Test2(""Fred"", val2 = 42) ]]
            //[[ kad_Test3() ]]
            using Foo;
            
            //[[ kad_Test4(""Fred"", val2 = 42) ]]
            //[[ kad_Test5(""Fred"", val2 = 42) ]]
            namespace Namespace1
            {         
                //[[ kad_Test6() ]]
                public class MyClass
                {
                    //[[ kad_Test7() ]]
                    public string Foo3;
                    //[[ kad_Test8() ]]
                    //[[ kad_Test9() ]]
                    public string Foo4{get;}
                    //[[ kad_TestA() ]]
                    public string Foo5() 
                    {
                       //[[ kad_TestB(""Fred"", val2 = 42) ]]
                       var x = b;
                       //[[ kad_TestC(""Fred"", val2 = 42) ]]
                    }
                }
 
                //[[ kad_Test7() ]]
                //[[ kad_Test8() ]]
                public class MyClass
                { }
}
            ";
         var root = RDom.CSharp.Load(csharpCode);
         var annotations = root.GetMembers().OfType<IPublicAnnotation>().ToArray();
         CheckPublicAnnotation<object>(annotations[0], "kad_Test1","root", MakeTuple("", "Fred"), MakeTuple("val2", 42));
      }

      private Tuple<string, object> MakeTuple(string name, object value)
      { return Tuple.Create(name, value); }

      private void CheckPublicAnnotation<T>(IPublicAnnotation annotation,string name, string target, params Tuple<string, T>[] values)
      {
         Assert.AreEqual(name, annotation.Name);
         Assert.AreEqual(target, annotation.Target);
         int i = 0;
         foreach (var key in annotation.Keys )
         {
            var value = annotation[key];
            Assert.AreEqual(values[i].Item1, key);
            Assert.AreEqual(values[i].Item2, value);
            i++;
         }
      }
#endregion
   }
}
