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
         var csharpCode =
@"          //[[ root:kad_Test1(""Fred"", val2 = 42) ]]
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
         CheckAnnotations1(root);
         var root2 = root.Copy();
         CheckAnnotations1(root2);
      }

      private void CheckAnnotations1(IRoot root)
      {
         var rootMembers = root.StemMembersAll.ToArray();
         CheckPublicAnnotation<object>(rootMembers[0] as IPublicAnnotation, "kad_Test1", "root", MakeTuple("", "Fred"), MakeTuple("val2", 42));
         CheckPublicAnnotation<object>(rootMembers[1] as IPublicAnnotation, "kad_Test2", "file", MakeTuple("", "Fred"), MakeTuple("val2", 42));
         CheckPublicAnnotation<object>(rootMembers[2] as IPublicAnnotation, "kad_Test3", "");
         Assert.IsNotNull(rootMembers[3] as IUsingDirective);
         Assert.IsNotNull(rootMembers[4] as IVerticalWhitespace);
         CheckPublicAnnotation<object>(rootMembers[5] as IPublicAnnotation, "kad_Test4", "", MakeTuple("", "Fred"), MakeTuple("val2", 42));
         CheckPublicAnnotation<object>(rootMembers[6] as IPublicAnnotation, "kad_Test5", "", MakeTuple("", "Fred"), MakeTuple("val2", 42));
         var nSpace = rootMembers[7] as INamespace; Assert.IsNotNull(nSpace);

         var members = nSpace.StemMembersAll.ToArray();
         CheckPublicAnnotation(members[0] as IPublicAnnotation, "kad_Test6", "");
         var cl = members[1] as IClass;
         Assert.IsNotNull(cl);

         var classMembers = cl.MembersAll.ToArray();
         CheckPublicAnnotation(classMembers[0] as IPublicAnnotation, "kad_Test7", "");
         var field = classMembers[1] as IField;
         Assert.IsNotNull(field);
         CheckPublicAnnotation(classMembers[2] as IPublicAnnotation, "kad_Test8", "");
         CheckPublicAnnotation(classMembers[3] as IPublicAnnotation, "kad_Test9", "");
         var property = classMembers[4] as IProperty;
         Assert.IsNotNull(property);
         CheckPublicAnnotation(classMembers[5] as IPublicAnnotation, "kad_TestA", "");
         var method = classMembers[6] as IMethod;
         Assert.IsNotNull(method);

         var methodMembers = method.StatementsAll.ToArray();
         CheckPublicAnnotation<object>(methodMembers[0] as IPublicAnnotation, "kad_TestB", "", MakeTuple("", "Fred"), MakeTuple("val2", 42));
         var declaration = methodMembers[1] as IDeclarationStatement;
         Assert.IsNotNull(declaration);

         CheckPublicAnnotation(members[3] as IPublicAnnotation, "kad_Test7", "");
         CheckPublicAnnotation(members[4] as IPublicAnnotation, "kad_Test8", "");
         cl = members[5] as IClass;
         Assert.IsNotNull(cl);

      }
      private Tuple<string, object> MakeTuple(string name, object value)
      { return Tuple.Create(name, value); }

      private void CheckPublicAnnotation(IPublicAnnotation annotation, string name, string target)
      {
         CheckPublicAnnotation<object>(annotation, name, target);
      }
      private void CheckPublicAnnotation<T>(IPublicAnnotation annotation, string name, string target, params Tuple<string, T>[] values)
      {
         Assert.AreEqual(name, annotation.Name);
         Assert.AreEqual(target, annotation.Target);
         Assert.AreEqual(values.Count(), annotation.Keys.Count());
         int i = 0;
         foreach (var key in annotation.Keys)
         {
            var value = annotation.GetValue(key);
            Assert.AreEqual(values[i].Item1, key);
            Assert.AreEqual(values[i].Item2, value);
            i++;
         }
      }
      #endregion
   }
}
