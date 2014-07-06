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
            var class2 = class1.Copy();
            rDomRoot.AddOrMoveMember(class2);
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
            [Foo(""Fred"", bar:3, bar2=3.14)] 
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
            Assert.AreEqual(@"Foo2(""Fred"",bar:3,bar2=3.14)", attribute2.BuildSyntax().ToString());
        }

        [TestMethod, TestCategory(MutabilityCategory)]
        public void Can_change_class_name_and_output()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2=3.14)] 
            public class Bar
            {
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
            Assert.AreEqual("[Foo(\"Fred\", bar: 3, bar2 = 3.14)]\r\npublic class Bar2\r\n{\r\n}", newCode );
        }

        #endregion
    }
}
