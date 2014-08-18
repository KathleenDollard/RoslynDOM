using System.Linq;
using ApprovalTests;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class MutabilityTests
    {
        private const string MutabilityCategory = "Mutability";

        #region Mutability tests
        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_add_copied_class_to_root()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2:""George"")] 
            public class Bar{}           
            ";
            var rDomRoot = RDomCSharp.Factory.GetRootFromString(csharpCode) as RDomRoot;
            var class1 = rDomRoot.RootClasses.First();
            var attribute = class1.Attributes.Attributes.First();
            var class2 = class1.Copy();
            rDomRoot.StemMembersAll. AddOrMove(class2);
            var classes = rDomRoot.Classes.ToArray();
            Assert.AreEqual(2, classes.Count());
            Assert.IsFalse(classes[0] == classes[1]); // reference equality fails
            Assert.IsTrue(classes[0].SameIntent(classes[1]));
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_add_copied_method_to_class()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2:""George"")] 
            public class Bar
            {
                public string FooBar() {}
            }           
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var rDomClass = root.RootClasses.First() as RDomClass;
            var method1 = rDomClass.Methods.First();
            var method2 = method1.Copy();
            rDomClass.MembersAll.AddOrMove(method2);
            var methods = rDomClass.Methods.ToArray();
            Assert.AreEqual(2, methods.Count());
            Assert.IsFalse(methods[0] == methods[1]); // reference equality fails
            Assert.IsTrue(methods[0].SameIntent(methods[1]));
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_change_attribute_name_and_output()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar : 3, bar2 = 3.14, bar3=true)] 
            public class Bar
            {
                public string FooBar()
                {}
            }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var attribute1 = class1.Attributes.Attributes.First();
            var attribute2 = class1.Attributes.Attributes.First().Copy() as RDomAttribute;
            Assert.IsTrue(attribute1.SameIntent(attribute2));
            attribute2.Name = "Foo2";
            Assert.IsFalse(attribute1.SameIntent(attribute2));
            Assert.AreEqual("Foo2", attribute2.Name);
            var expected = "            [Foo2(\"Fred\", bar : 3, bar2 = 3.14, bar3=true)] \r\n";
            var actual = RDomCSharp.Factory.BuildSyntax(attribute2).ToFullString();
            var syntax1 = RDomCSharp.Factory.BuildSyntax(class1);
            var actualClass = syntax1.ToFullString();
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(csharpCode, actualClass);
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_change_class_name_and_output()
        {
            var csharpCode =
@"            [ Foo ( ""Fred"" , bar:3 , bar2=3.14 ) ] 
            public class Bar
            {
                [Bar(bar:42)] 
                public string FooBar() {}
            }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var class2 = root.RootClasses.First().Copy() as RDomClass;
            Assert.IsTrue(class1.SameIntent(class2));
            class2.Name = "Bar2";
            var csharpCodeChanged = csharpCode.ReplaceFirst("class Bar", "class Bar2");
            Assert.IsFalse(class1.SameIntent(class2));
            Assert.AreEqual("Bar2", class2.Name);
            var origCode = RDomCSharp.Factory.BuildSyntax(class1).ToFullString();
            var newCode = RDomCSharp.Factory.BuildSyntax(class2).ToFullString();
            Assert.AreEqual(csharpCodeChanged, newCode);
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_change_generic_class_name_and_output()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2=3.14)] 
            public class Bar<T>
            {
                private int fooish;
                [Bar( bar:42)] 
                public string FooBar() {}
            }           
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var class2 = root.RootClasses.First().Copy() as RDomClass;
            var newCode = RDomCSharp.Factory.BuildSyntax(class2).ToFullString();
            Assert.IsTrue(class1.SameIntent(class2));
            class2.Name = "Bar2";
            Assert.IsFalse(class1.SameIntent(class2));
            Assert.AreEqual("Bar2", class2.Name);
            newCode = RDomCSharp.Factory.BuildSyntax(class2).ToFullString();
            var expected = "            [Foo(\"Fred\", bar:3, bar2=3.14)] \r\n            public class Bar2<T>\r\n            {\r\n                private int fooish;\r\n                [Bar( bar:42)] \r\n                public string FooBar() {}\r\n            }           \r\n";
            Assert.AreEqual(expected , newCode);
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_remove_params_from_method()
        {
            var csharpCode = @"
            public class Bar
            {
                public string FooBar(int bar1, string bar2) {}
            }           
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var method = root.RootClasses.First().Methods.First() as RDomMethod;
            var param = method.Parameters.First();
            Assert.AreEqual(2, method.Parameters.Count());
            method.Parameters.Remove(param);
            Assert.AreEqual(1, method.Parameters.Count());
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_remove_type_params_from_class()
        {
            var csharpCode = @"
            public class Bar<T1, T2, T3>
            {
                public string FooBar(int bar1, string bar2) {}
            }           
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First() as RDomClass;
            var param = class1.TypeParameters.Skip (1).First();
            Assert.AreEqual(3, class1.TypeParameters.Count());
            class1.TypeParameters.Remove(param);
            Assert.AreEqual(2, class1.TypeParameters.Count());
            Assert.AreEqual("T1", class1.TypeParameters.First().Name);
            Assert.AreEqual("T3", class1.TypeParameters.Last().Name);
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_clear_type_params_from_class()
        {
            var csharpCode = @"
            public class Bar<T1, T2, T3>
            {
                public string FooBar(int bar1, string bar2) {}
            }           
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First() as RDomClass;
            Assert.AreEqual(3, class1.TypeParameters.Count());
            class1.TypeParameters.Clear();
            Assert.AreEqual(0, class1.TypeParameters.Count());
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_remove_member_from_class()
        {
            var csharpCode = @"
            public class Bar<T1, T2, T3>
            {
                private int foo;
                public string FooBar(int bar1, string bar2) {}
            }           
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First() as RDomClass;
            Assert.AreEqual(2, class1.Members.Count());
            class1.MembersAll.Clear();
            Assert.AreEqual(0, class1.Members.Count());
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_remove_stem_member_from_root()
        {
            var csharpCode = @"
            using System;
            public class Bar{}           
            public struct Bar2{}             ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode) as RDomRoot;
            Assert.AreEqual(3, root.StemMembers.Count());
            var class1 = root.Classes.First();
            root.StemMembersAll. Remove(class1);
            Assert.AreEqual(2, root.StemMembers.Count());
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_clear_stem_members_from_root()
        {
            var csharpCode = @"
            using System;
            public class Bar{}           
            public struct Bar2{}             ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode) as RDomRoot;
            Assert.AreEqual(3, root.StemMembers.Count());
            var class1 = root.Classes.First();
            root.ClearStemMembers ();
            Assert.AreEqual(0, root.StemMembers.Count());
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_copy_multi_member_root()
        {
            var csharpCode = @"
            using System;
            public class Bar{}           
            public struct Bar2{}           
            public enum Bar3{}           
            public interface Bar4{}           
            ";
            var rDomRoot = RDomCSharp.Factory.GetRootFromString(csharpCode) as RDomRoot;
            var rDomRoot2 = rDomRoot.Copy() ;
            var class1 = rDomRoot.RootClasses.First() as RDomClass;
            Assert.IsTrue(rDomRoot.SameIntent(rDomRoot2));
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_build_syntax_for_multi_member_root()
        {
            var csharpCode = @"
            using System;
            using System.Data;
            public class Bar{}           
            public struct Bar2{}           
            public enum Bar3{}           
            public interface Bar4{}";
            var rDomRoot = RDomCSharp.Factory.GetRootFromString(csharpCode) as RDomRoot;
            var output = RDomCSharp.Factory .BuildSyntax(rDomRoot);
            Assert.AreEqual(csharpCode, output.ToFullString() );
        }
        #endregion
    }
}
