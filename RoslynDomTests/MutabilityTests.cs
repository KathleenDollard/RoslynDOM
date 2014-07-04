using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;

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
            rDomRoot.AddMember(class2);
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
            rDomClass.AddMember(method2);
            var methods = rDomClass.Methods.ToArray();
            Assert.AreEqual(2, methods.Count());
            Assert.IsFalse(methods[0] == methods[1]); // reference equality fails
            Assert.IsTrue(methods[0].SameIntent(methods[1]));
        }


        #endregion
    }
}
