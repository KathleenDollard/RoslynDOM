using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDomTests
{
   [TestClass]
   public class RDomCollectionTests
   {
      private const string MakeListCategory = "MakeList";
      private const string CreateFromCategory = "CreateFrom";

      private RDomNamespace GetTestNamespaceWithClasses()
      {
         var nspace = new RDomNamespace("MyNamespace");
         nspace.StemMembersAll.AddOrMove(new RDomClass("A"));
         nspace.StemMembersAll.AddOrMove(new RDomClass("B"));
         nspace.StemMembersAll.AddOrMove(new RDomClass("C"));
         return nspace;
      }



      #region make list
      [TestMethod, TestCategory(MakeListCategory)]
      public void Can_load_collection_with_classes_using_input_and_selection()
      {
         var CSharpCode = @"
public class A {}
public class B {}
public class C {}
public class D {}";
         var treeRoot = SyntaxFactory.ParseSyntaxTree(CSharpCode).GetCompilationUnitRoot();
         var root = RDom.CSharp.Load("namespace Fred{}");
         var nspace = root.ChildNamespaces.First();
         nspace.StemMembersAll.CreateAndAdd(
            treeRoot,
            tr => tr.ChildNodes().OfType<ClassDeclarationSyntax>(),
            s => new RDomClass(s, nspace, null));
         Assert.AreEqual(4, nspace.Classes.Count());
      }

  
      #endregion
   }
}
