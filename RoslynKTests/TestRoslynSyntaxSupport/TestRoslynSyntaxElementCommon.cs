using KadGen.Common;
using KadGen.Common.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestRoslyn
{
    public class TestRoslynSupportBase
    {
        private static string treeForClassTests
        {
            get
            {
                return @"
using UsingA;
using System.Reflection;

namespace NamespaceA
{
    using UsingB;
    [ClassAttribute()]
    public class ClassA
    {
        [Version(2)]
        [Keyword()]
        public void foo2(int Bar, string Bar2)      {        }
    }
    [InterfaceAttribute()]
    public interface InterfaceA {}
    public interface InterfaceB {}

    public class ClassB {}
  
    namespace NamespaceB
    {
       using UsingC;
       using UsingD;
       [ClassAttribute1(42)]
       [ClassAttribute2(""Bar"")]
       public class ClassC : InterfaceA, InterfaceB
        {
            [Version(2)]
            [Keyword()]
            public void foo2(int Bar, string Bar2)      {        }
        }

        [InterfaceAttribute1(42)]
        [InterfaceAttribute2(""Bar"")]
        public interface InterfaceC {}
        public interface InterfaceD {}
        public class ClassD : IA   { }
 
        namespace NamespaceC{}
        namespace NamespaceD{
            using UsingD;
            namespace NamespaceD{}
            public class ClassD {}
            public interface InterfaceD {}
      }
    }
}
";
            }
        }

        private static SyntaxTree _tree;
        internal static SyntaxTree Tree

        {
            get
            {
                if (_tree == null) { _tree = CSharpSyntaxTree.ParseText(treeForClassTests); }
                return _tree;
            }
        }

        private static SyntaxNode _namespaceB;
        internal static SyntaxNode NamespaceB

        {
            get
            {
                if (_namespaceB == null) { _namespaceB = Tree.GetNamespaceByName("NamespaceB"); }
                return _namespaceB;
            }
        }

        private static SyntaxTree _emptyRoot;
        internal static SyntaxTree EmptyRoot

        {
            get
            {
                if (_emptyRoot == null) { _emptyRoot = CSharpSyntaxTree.ParseText(""); }
                return _emptyRoot;
            }
        }

    }
    public class TestRoslynSyntaxElement :TestRoslynSupportBase 
    {
        public TestRoslynSyntaxElement(Type supportType, string kind, string kindPlural = null)
        {
            this.kind = kind;
            if (kindPlural == null)
            {
                if (kind.EndsWith("s")) { kindPlural = kind + "es"; }
                else { kindPlural = kind + "s"; }
            }
            this.byNameMethodForTree = ReflectionSupport.GetMethod(supportType,
                    "Get" + kind + "ByName", typeof(SyntaxTree), typeof(string), typeof(bool));
            this.byNameMethodForNode = ReflectionSupport.GetMethod(supportType,
                    "Get" + kind + "ByName", typeof(CSharpSyntaxNode), typeof(string), typeof(bool));
            this.getListMethodForTree = ReflectionSupport.GetMethod(supportType,
                    "Get" + kindPlural, typeof(SyntaxTree), typeof(bool));
            this.getListMethodForNode = ReflectionSupport.GetMethod(supportType,
                    "Get" + kindPlural, typeof(CSharpSyntaxNode), typeof(bool));

        }

        protected string kind;
        protected MethodInfo byNameMethodForTree;
        protected MethodInfo byNameMethodForNode;
        protected MethodInfo getListMethodForTree;
        protected MethodInfo getListMethodForNode;
        protected int totalFlattenedCountRoot;
        protected int totalNotFlattenedCountRoot;
        protected int totalFlattenedCountNamespaceB;
        protected int totalNotFlattenedCountNamespaceB;
        protected int getByNameInCount;
        protected int getByNameInRootCount;
        protected string nameAppearingOnce;
        protected string nameAppearingAboveAndBelowB;


    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The purpose of this class is to test not only the behavior, but also
    /// that each of the types has strictly followed the pattern. 
    /// </remarks>
    public class TestRoslynSyntaxElement<T> : TestRoslynSyntaxElement
    {
        protected MethodInfo getName;

        public TestRoslynSyntaxElement(Type supportType, string kind, string kindPlural = null)
            : base(supportType, kind, kindPlural)
        {
             this.getName = ReflectionSupport.GetMethod(supportType,
                "GetName", typeof(T));
        }

        protected IEnumerable<T> GetList(SyntaxTree tree, bool doNotFlatten = false)
        {
            IEnumerable<T> ret;
            try
            {
                ret = (IEnumerable<T>)getListMethodForTree.Invoke(null, new object[] { tree, doNotFlatten });
            }
            catch (Exception ex)
            { throw ex.InnerException; }
            return ret;
        }

        protected IEnumerable<T> GetList(SyntaxNode node, bool doNotFlatten = false)
        {
            IEnumerable<T> ret;
            try
            {
                ret = (IEnumerable<T>)getListMethodForNode.Invoke(null, new object[] { node, doNotFlatten });
            }
            catch (Exception ex)
            { throw ex.InnerException; }
            return ret;
        }

        protected T GetNodeByName(SyntaxTree tree, string name, bool doNotFlatten = false)
        {
            T ret;
            try
            {
                ret = (T)byNameMethodForTree.Invoke(null, new object[] { tree, name, doNotFlatten });
            }
            catch (Exception ex)
            { throw ex.InnerException; }
            return ret;
        }

        protected T GetNodeByName(SyntaxNode node, string name, bool doNotFlatten = false)
        {
            T ret;
            try
            {
                ret = (T)byNameMethodForNode.Invoke(null, new object[] { node, name, doNotFlatten });
            }
            catch (Exception ex)
            { throw ex.InnerException; }
            return ret;
        }

        protected string GetName(T node)
        {
            string ret;
            try
            {
                ret = (string)getName.Invoke(null, new object[] { node });
            }
            catch (Exception ex)
            { throw ex.InnerException; }
            return ret;
        }

        [TestMethod]
        public void Get_correct_flattened_from_root()
        {
            Assert.AreEqual(totalFlattenedCountRoot, GetList(Tree).Count());
        }

        [TestMethod]
        public void Get_correct_not_flattened_from_root()
        {
            Assert.AreEqual(totalNotFlattenedCountRoot, GetList(Tree, true).Count());
        }

        [TestMethod]
        public void Get_correct_flattened_from_arbitrary_location()
        {
            Assert.AreEqual(totalFlattenedCountNamespaceB, GetList(NamespaceB).Count());
        }

        [TestMethod]
        public void Get_correct_not_flattened_arbitrary_location()
        {
            Assert.AreEqual(totalNotFlattenedCountNamespaceB, GetList(NamespaceB, true).Count());
        }

        [TestMethod]
        public void Get_zero_items_for_empty_tree()
        {
            Assert.AreEqual(0, GetList(EmptyRoot).Count());
            Assert.AreEqual(0, GetList(EmptyRoot, true).Count());
        }

        [TestMethod]
        public void Names_should_be_correct()
        {
            Assert.AreEqual(kind + "A", GetName(GetList(Tree).First()));
            Assert.AreEqual(kind + "C", GetName(GetList(NamespaceB).First()));
        }

        [TestMethod]
        public void GetByName_from_root()
        {
            var item = GetNodeByName(Tree, kind + "B");
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void GetByName_arbitrary_location()
        {
            var item = GetNodeByName(NamespaceB, kind + "C");
            Assert.IsNotNull(item);
            var item2 = GetNodeByName(NamespaceB, kind + "B");
            Assert.IsNull(item2);
        }

        [TestMethod]
        public void GetByName_returns_null_when_not_found()
        {
            var item = GetNodeByName(NamespaceB, kind + "XB");
            Assert.IsNull(item);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetByName_throws_exception_if_multiple()
        {
            var item = GetNodeByName(NamespaceB, kind + "D");
        }

        [TestMethod]
        public void GetByName_respects_do_not_flatten_attribute()
        {
            var item = GetNodeByName(NamespaceB, kind + "D", true);
            Assert.IsNotNull(item);
        }
    }
    public class TestRoslynSyntaxElementWithAttributes<T> : TestRoslynSyntaxElement<T>
    {
        public TestRoslynSyntaxElementWithAttributes(Type supportType, string kind, string kindPlural = null)
            : base(supportType, kind, kindPlural)
        {
        }

        [TestMethod]
        public void Attribute_counts_correct()
        {
            var item = (SyntaxNode)(object)GetNodeByName(Tree, kind + "A");
            Assert.AreEqual(1, item.GetAttributes().Count());
            item = (SyntaxNode)(object)GetNodeByName(Tree, kind + "B");
            Assert.AreEqual(0, item.GetAttributes().Count());
            item = (SyntaxNode)(object)GetNodeByName(Tree, kind + "C");
            Assert.AreEqual(2, item.GetAttributes().Count());
        }


        [TestMethod]
        public void Attribute_names_correct()
        {
            var item = (SyntaxNode)(object)GetNodeByName(Tree, kind + "C");
            var attribs = item.GetAttributes();
            Assert.AreEqual(kind + "Attribute1", attribs.First().GetName());
            Assert.AreEqual(kind + "Attribute2", attribs.Last().GetName());
        }

        [TestMethod]
        public void Attribute_values_correct()
        {
            var item = (SyntaxNode)(object)GetNodeByName(Tree, kind + "C");
            var attribs = item.GetAttributes();
            var attrib1 = attribs.First();
            var attrib2 = attribs.Last();
            var result1 = attrib1.GetAttributeValue<int>();
            var result2 = attrib2.GetAttributeValue<string>();
            Assert.AreEqual(42, result1);
            Assert.AreEqual("Bar", result2);
        }
    }
}
