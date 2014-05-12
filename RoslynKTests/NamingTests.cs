using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynK;

namespace RoslynKTests
{
    [TestClass]
    public class NamingTests
    {
        private IRoot GetTestRootA()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
using System.Diagnostics.Tracing;
namespace testing.Namespace1
{
    namespace nested.Namespace
    {
        [EventSource()]
        public class FooBar
        { }

        [System.Serializable]
        public class TestingGenericClass<KadEventSource>
        {
            private string _bar2;

            public void Foo(int i) {};
            public int Foo(string s, int i) {};
            public string Bar{ get; set; }
            public string Bar2
            {
                get
                {
                    return _bar2;
                }
            }
            public string @class {get; private set;}
        }

        namespace @namespace
        { }
    }
}
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            return treeWrapper.Root;
        }

        private IRoot GetTestRootB()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
using System.Diagnostics.Tracing;
namespace testing.Namespace1
{}
");
            var factory = new KFactory();
            var treeWrapper = factory.CreateTreeWrapper(tree);
            return treeWrapper.Root;
        }


        private INamespace GetInnerNamespaceA()
        {
            var root = GetTestRootA();
            return root.Namespaces.First().Namespaces.First();
        }

        [TestMethod]
        public void Root_name_should_be_Root()
        {
            var root = GetTestRootA();
            Assert.AreEqual("Root", root.Name);
        }

        [TestMethod]
        public void Namespace_name_should_be_correct()
        {
            var root = GetTestRootB();
            var nspace1 = root.Namespaces.Single();
            var name1 = nspace1.Name;
            Assert.AreEqual("testing.Namespace1", name1);
        }

        [TestMethod]
        public void Namespace_name_should_be_correct_including_nesting()
        {
            var root = GetTestRootA();
            var nspace1 = root.Namespaces.Single();
            var nspace2 = nspace1.Namespaces.Single();
            var name1 = nspace1.Name;
            var name2 = nspace2.Name;
            Assert.AreEqual("testing.Namespace1", name1);
            Assert.AreEqual("nested.Namespace", name2);
        }

        [TestMethod]
        public void ClasOrStruct_name_should_be_correct()
        {
            var nspace2 = GetInnerNamespaceA();
            Assert.AreEqual(2, nspace2.Classes.Count());
            var class1 = nspace2.Classes.First();
            var class2 = nspace2.Classes.Last();
            var name1 = class1.Name;
            var name2 = class2.Name;
            Assert.AreEqual("FooBar", name1);
            Assert.AreEqual("TestingGenericClass", name2);
        }

        [TestMethod]
        public void Keyword_should_not_include_at_sign()
        {
            var nspace = GetInnerNamespaceA();
            var nspace1 = nspace.Namespaces.Last();
            var class1 = nspace.Classes.Last();
            var property = class1.Properties.Last();
            var name1 = nspace1.Name;
            var name2 = property.Name;
            Assert.AreEqual("namespace", name1);
            Assert.AreEqual("class", name2);
        }
    }
}
