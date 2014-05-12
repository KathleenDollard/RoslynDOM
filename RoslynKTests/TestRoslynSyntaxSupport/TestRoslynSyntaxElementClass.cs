using KadGen.Common.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TestRoslyn
{

    [TestClass]
    public class TestRoslynSyntaxElementClass : TestRoslynSyntaxElementWithAttributes<ClassDeclarationSyntax>
    {
        public TestRoslynSyntaxElementClass() : base(typeof(RoslynSyntaxSupportClass), "Class")
        {
            totalFlattenedCountRoot = 5;
            totalNotFlattenedCountRoot = 2;
            totalFlattenedCountNamespaceB = 3;
            totalNotFlattenedCountNamespaceB = 2;
        }

        [TestMethod]
        public void Class_namespace_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    public class NormalEventSource
    {
        [Version(2)]
        [Keyword()]
        public void foo()      {        }
    }

}
    public class SimpleEventSource
    {
        [Version(2)]
        [Keyword()]
        public void foo()      {        }
    }
");

            Assert.AreEqual("ETWPreMan", tree.GetClasses().First().GetNamespaceName());
            Assert.AreEqual("", tree.GetClasses().Last().GetNamespaceName());
        }

        [TestMethod]
        public void Method_counts_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    public class NormalEventSource
    {
        public void foo()      {        }

        [Version(2)]
        [Keyword()]
        public void foo2()      {        }
    }

    public class SimpleEventSource
    {
        [Version(2)]
        [Keyword()]
        public void foo()      {        }
    }

   public class SimpleEventSource2
    {}
}
");
            var classes = tree.GetClasses();
            Assert.AreEqual(2, classes.First().GetMethods().Count());
            Assert.AreEqual(1, classes.Skip(1).First().GetMethods().Count());
            Assert.AreEqual(0, classes.Skip(2).First().GetMethods().Count());
        }

        [TestMethod]
        public void Method_names_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    public class NormalEventSource
    {
        public void foo()      {        }

        [Version(2)]
        [Keyword()]
        public void foo2()      {        }
    }
}
");
            var classNode = tree.GetClasses().First();
            Assert.AreEqual("foo", classNode.GetMethods().First().GetName());
            Assert.AreEqual("foo2", classNode.GetMethods().Last().GetName());
        }

        [TestMethod]
        public void Method_bodies_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    public class NormalEventSource
    {
        public void foo()    
        {     
            var x = ""Fred"";
            WriteEvent(1);
        }

        [Version(2)]
        [Keyword()]
        public void foo2()      {}
    }
}
");
            var expected = @"{     
            var x = ""Fred"";
            WriteEvent(1);
        }";
            var classNode = tree.GetClasses().First();
            Assert.AreEqual(expected, classNode.GetMethods().First().GetMethodBody());
            Assert.AreEqual("{}", classNode.GetMethods().Last().GetMethodBody());
        }

        [TestMethod]
        public void Method_ReturnType_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    public class NormalEventSource
    {
        public void foo()    
        {     
            var x = ""Fred"";
            WriteEvent(1);
        }

        [Version(2)]
        [Keyword()]
        public String foo2()      {}
    }
}
");
            var classNode = tree.GetClasses().First();
            Assert.AreEqual("void", classNode.GetMethods().First().GetMethodReturnType());
            Assert.AreEqual("String", classNode.GetMethods().Last().GetMethodReturnType());
        }

        [TestMethod]
        public void MethodAttribute_counts_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    public class NormalEventSource
    {

        [Version(2)]
        [Keyword()]
        public void foo2()      {        }

        [Keyword()]
        public void foo()      {        }

        public void foo3()      {        }
    }
}
");
            var methods = tree.GetClasses().First().GetMethods();
            Assert.AreEqual(2, methods.First().GetAttributes().Count());
            Assert.AreEqual(1, methods.Skip(1).First().GetAttributes().Count());
            Assert.AreEqual(0, methods.Skip(2).First().GetAttributes().Count());
        }

        [TestMethod]
        public void NodeAttribute_counts_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    [Foo(3), Foo(2), Foo(1)]
    [Foo1]
    [Foo2(23, 45)]
    public class NormalEventSource
    { 
       [Version(2)]
       [Keyword()]
       public void foo2()
       { }

       [Keyword()]
       public void foo()      {        }

       public void foo3()      {        }
    }

    [Foo()]
    public class SimpleEventSource
    {}

}
   public class SimpleEventSource2
    {}
");
            var classes = tree.GetClasses();
            Assert.AreEqual(5, ((SyntaxNode)classes.First()).GetAttributes().Count());
            Assert.AreEqual(1, ((SyntaxNode)classes.Skip(1).First()).GetAttributes().Count());
            Assert.AreEqual(0, ((SyntaxNode)classes.Skip(2).First()).GetAttributes().Count());
            var methods = tree.GetClasses().First().GetMethods();
            Assert.AreEqual(2, ((SyntaxNode)methods.First()).GetAttributes().Count());
            Assert.AreEqual(1, ((SyntaxNode)methods.Skip(1).First()).GetAttributes().Count());
            Assert.AreEqual(0, ((SyntaxNode)methods.Skip(2).First()).GetAttributes().Count());
        }

        [TestMethod]
        public void Attribute_names_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    [Foo(3), Foo(2), Foo(1)]
    [Foo(3)]
    [Bar()]
    public class NormalEventSource
    {

        [Version(2)]
        [Keyword()]
        public void foo2()      {        }
    }
}
");
            var classNode = tree.GetClasses().First();
            var methods = classNode.GetMethods();
            var classAttributes = classNode.GetAttributes();
            var methodAttributes = methods.First().GetAttributes();

            Assert.AreEqual("Foo", classAttributes.First().GetName());
            Assert.AreEqual("Bar", classAttributes.Last().GetName());
            Assert.AreEqual("Version", methodAttributes.First().GetName());
            Assert.AreEqual("Keyword", methodAttributes.Last().GetName());
        }

        [TestMethod]
        public void Attribute_values_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    [Foo(3), Foo(2), Foo(1)]
    [Foo(3.4)]
    [Bar()]
    public class NormalEventSource
    {

        [Version(2)]
        [Keyword(""Baz"")]
        [Test(true)]
       public void foo2()      {        }
    }
}
");
            var classNode = tree.GetClasses().First();
            var methods = classNode.GetMethods();
            var classAttributes = classNode.GetAttributes();
            var methodAttributes = methods.First().GetAttributes();

            Assert.AreEqual(3, classAttributes.First().GetAttributeValue<int>());
            Assert.AreEqual(2, classAttributes.Skip(1).First().GetAttributeValue<int>());
            Assert.AreEqual(3.4, classAttributes.Skip(3).First().GetAttributeValue<double>());
            Assert.AreEqual(0, classAttributes.Last().GetAttributeValue<int>());
            Assert.AreEqual(2, methodAttributes.First().GetAttributeValue<int>());
            Assert.AreEqual("Baz", methodAttributes.Skip(1).First().GetAttributeValue<string>());
            Assert.AreEqual(true, methodAttributes.Last().GetAttributeValue<bool>());
        }


        [TestMethod]
        public void Attribute_values_should_be_correct_if_untyped()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    [Foo(3), Foo(2), Foo(1)]
    [Foo(3.4)]
    [Bar()]
    public class NormalEventSource
    {

        [Version(2)]
        [Keyword(""Baz"")]
        [Test(true)]
       public void foo2()      {        }
    }
}
");
            var classNode = tree.GetClasses().First();
            var methods = classNode.GetMethods();
            var classAttributes = classNode.GetAttributes();
            var methodAttributes = methods.First().GetAttributes();

            Assert.AreEqual(3, classAttributes.First().GetAttributeValue(typeof(int)));
            Assert.AreEqual(2, classAttributes.Skip(1).First().GetAttributeValue(typeof(int)));
            Assert.AreEqual(3.4, classAttributes.Skip(3).First().GetAttributeValue(typeof(double)));
            Assert.AreEqual(0, classAttributes.Last().GetAttributeValue(typeof(int)));
            Assert.AreEqual(2, methodAttributes.First().GetAttributeValue(typeof(int)));
            Assert.AreEqual("Baz", methodAttributes.Skip(1).First().GetAttributeValue(typeof(string)));
            Assert.AreEqual(true, methodAttributes.Last().GetAttributeValue(typeof(bool)));
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))] // An InvalidOp exception here represents a problem 
        public void Attribute_values_should_throw_on_non_literal()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    [Foo(3 + 4)]
    [Bar()]
    public class NormalEventSource
    {    }
}
");
            var classNode = tree.GetClasses().First();
            var classAttributes = classNode.GetAttributes();
            Assert.AreEqual(0, classAttributes.First().GetAttributeValue<int>());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Attribute_values_should_throw_on_bad_type()
        {
            // This test is important because VS isn't handling type checks if input isn't compiled
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    [Keyword(""Baz"")]
    public class NormalEventSource
    { }
}
");
            var classNode = tree.GetClasses().First();
            var classAttributes = classNode.GetAttributes();

            Assert.AreEqual(3, classAttributes.First().GetAttributeValue<int>());
        }

        [TestMethod]
        public void Parameter_counts_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    public class NormalEventSource
    {

        [Version(2)]
        [Keyword()]
        public void foo2(int Bar, string Bar2)      {        }

        [Keyword()]
        public void foo(int Bar)      {        }

        public void foo3()      {        }
    }
}
");
            var methods = tree.GetClasses().First().GetMethods();
            Assert.AreEqual(2, methods.First().GetParameters().Count());
            Assert.AreEqual(1, methods.Skip(1).First().GetParameters().Count());
            Assert.AreEqual(0, methods.Skip(2).First().GetParameters().Count());
        }

        [TestMethod]
        public void Parameter_names_and_types_should_be_correct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace ETWPreMan
{
    public class NormalEventSource
    {

        [Version(2)]
        [Keyword()]
        public void foo2(int Bar, string Bar2)      {        }
    }
}
");
            var parameters = tree.GetClasses().First().GetMethods().First().GetParameters();
            Assert.AreEqual("Bar", parameters.First().GetName());
            Assert.AreEqual("Bar2", parameters.Last().GetName());
            Assert.AreEqual("int", parameters.First().GetParameterType());
            Assert.AreEqual("string", parameters.Last().GetParameterType());
        }

        //[TestMethod]
        //public void GetClasses_correct_flattened()
        //{
        //    SyntaxTree tree = CSharpSyntaxTree.ParseText(RoslynTestCommon.treeForClassTests);
        //    Assert.AreEqual(5, RoslynSupport.GetClasses(tree).Count());
        //}
        //[TestMethod]
        //public void GetClasses_correct_not_flattened()
        //{
        //    SyntaxTree tree = CSharpSyntaxTree.ParseText(RoslynTestCommon.treeForClassTests);
        //    Assert.AreEqual(2, RoslynSupport.GetClasses(tree, true).Count());
        //}

        //[TestMethod]
        //public void GetClassByName_finds_class()
        //{
        //    SyntaxTree tree = CSharpSyntaxTree.ParseText(RoslynTestCommon.treeForClassTests);
        //    var classNode = tree.GetClassByName("ClassC");
        //    Assert.IsNotNull(classNode);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void GetClassByName_throws_when_no_match()
        //{
        //    SyntaxTree tree = CSharpSyntaxTree.ParseText(RoslynTestCommon.treeForClassTests);
        //    var classNode = tree.GetClassByName("ClassX");
        //}

        //[TestMethod]
        //[ExpectedException(typeof(InvalidOperationException))]
        //public void GetClassByName_throws_with_multiple_matches()
        //{
        //    SyntaxTree tree = CSharpSyntaxTree.ParseText(RoslynTestCommon.treeForClassTests);
        //    var classNode = tree.GetClassByName("ClassD");

        //}

    }
}