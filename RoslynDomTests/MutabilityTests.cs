using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;

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
            var rDomRoot = RDomFactory.GetRootFromString(csharpCode) as RDomRoot;
            var class1 = rDomRoot.RootClasses.First();
            var attribute = class1.Attributes.First();
            var class2 = class1.Copy();
            rDomRoot.AddOrMoveStemMember(class2);
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
            var root = RDomFactory.GetRootFromString(csharpCode);
            var rDomClass = root.RootClasses.First() as RDomClass;
            var method1 = rDomClass.Methods.First();
            var method2 = method1.Copy();
            rDomClass.AddOrMoveMember(method2);
            var methods = rDomClass.Methods.ToArray();
            Assert.AreEqual(2, methods.Count());
            Assert.IsFalse(methods[0] == methods[1]); // reference equality fails
            Assert.IsTrue(methods[0].SameIntent(methods[1]));
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_change_attribute_name_and_output()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2=3.14, bar3=true)] 
            public class Bar
            {
                public string FooBar() {}
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var attribute1 = class1.Attributes.First();
            var attribute2 = class1.Attributes.First().Copy() as RDomAttribute;
            Assert.IsTrue(attribute1.SameIntent(attribute2));
            attribute2.Name = "Foo2";
            Assert.IsFalse(attribute1.SameIntent(attribute2));
            Assert.AreEqual("Foo2", attribute2.Name);
            Assert.AreEqual("Foo2(\"Fred\",bar:3,bar2=3.14,bar3=true)", attribute2.BuildSyntax().ToString());
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_change_class_name_and_output()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2=3.14)] 
            public class Bar
            {
                [Bar( bar:42)] 
                public string FooBar() {}
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var class2 = root.RootClasses.First().Copy() as RDomClass;
            Assert.IsTrue(class1.SameIntent(class2));
            class2.Name = "Bar2";
            Assert.IsFalse(class1.SameIntent(class2));
            Assert.AreEqual("Bar2", class2.Name);
            var newCode = RDomFactory.BuildSyntax(class2).ToString();
            var expected = "[Foo(\"Fred\", bar: 3, bar2 = 3.14)]\r\npublic class Bar2\r\n{\r\n    [Bar(bar: 42)]\r\n    public String FooBar()\r\n    {\r\n    }\r\n}";
            Assert.AreEqual(expected, newCode);
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
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var class2 = root.RootClasses.First().Copy() as RDomClass;
            Assert.IsTrue(class1.SameIntent(class2));
            class2.Name = "Bar2";
            Assert.IsFalse(class1.SameIntent(class2));
            Assert.AreEqual("Bar2", class2.Name);
            var newCode = class2.BuildSyntax().ToString();
            Assert.AreEqual("[Foo(\"Fred\", bar: 3, bar2 = 3.14)]\r\npublic class Bar2\r\n{\r\n    private Int32 fooish;\r\n\r\n    [Bar(bar: 42)]\r\n    public String FooBar()\r\n    {\r\n    }\r\n}", newCode);
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
            var root = RDomFactory.GetRootFromString(csharpCode);
            var method = root.RootClasses.First().Methods.First() as RDomMethod;
            var param = method.Parameters.First();
            Assert.AreEqual(2, method.Parameters.Count());
            method.RemoveParameter(param);
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
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First() as RDomClass;
            var param = class1.TypeParameters.Skip (1).First();
            Assert.AreEqual(3, class1.TypeParameters.Count());
            class1.RemoveTypeParameter(param);
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
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First() as RDomClass;
            Assert.AreEqual(3, class1.TypeParameters.Count());
            class1.ClearTypeParameters();
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
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First() as RDomClass;
            Assert.AreEqual(2, class1.Members.Count());
            class1.ClearMembers();
            Assert.AreEqual(0, class1.Members.Count());
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_remove_stem_member_from_root()
        {
            var csharpCode = @"
            using System;
            public class Bar{}           
            public struct Bar2{}             ";
            var root = RDomFactory.GetRootFromString(csharpCode) as RDomRoot;
            Assert.AreEqual(3, root.StemMembers.Count());
            var class1 = root.Classes.First();
            root.RemoveStemMember(class1);
            Assert.AreEqual(2, root.StemMembers.Count());
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_clear_stem_members_from_root()
        {
            var csharpCode = @"
            using System;
            public class Bar{}           
            public struct Bar2{}             ";
            var root = RDomFactory.GetRootFromString(csharpCode) as RDomRoot;
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
            var rDomRoot = RDomFactory.GetRootFromString(csharpCode) as RDomRoot;
            var rDomRoot2 = rDomRoot.Copy() ;
            var class1 = rDomRoot.RootClasses.First() as RDomClass;
            Assert.IsTrue(rDomRoot.SameIntent(rDomRoot2));
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_build_syntax_for_multi_member_root()
        {
            var csharpCode = @"
            using System;
            public class Bar{}           
            public struct Bar2{}           
            public enum Bar3{}           
            public interface Bar4{}           
            ";
            var rDomRoot = RDomFactory.GetRootFromString(csharpCode) as RDomRoot;
            var output = rDomRoot.BuildSyntax();
            var expectedCode = "using System;\r\n\r\npublic class Bar\r\n{\r\n}\r\n\r\npublic struct Bar2\r\n{\r\n}\r\n\r\npublic enum Bar3\r\n{\r\n}\r\n\r\npublic interface Bar4\r\n{\r\n}";
            Assert.AreEqual(expectedCode,output.ToString() );
        }
        #endregion
    }
}
