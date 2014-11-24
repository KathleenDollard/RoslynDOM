using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomCompilationFactory : IRDomCompilationFactory
   {
      private RDomCorporation _corporation;

      public RDomCompilationFactory(RDomCorporation corporation)
      { _corporation = corporation; }

      public RDomPriority Priority
      { get { return RDomPriority.Normal; } }

      //protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      //{
      //   var syntax = syntaxNode as CompilationUnitSyntax;
      //   var newItem = new RDomRoot(syntaxNode, parent, model);
      //   // Root does not call StandardInitialize because the info is attched to the first item
      //   // and particularly, whitespace would be doubled. 
      //   //CreateFromWorker.InitializePublicAnnotations(newItem,  syntaxNode,  parent,  model);

      //   newItem.Name = "Root";
      //   CreateFromWorker.LoadStemMembers(newItem, syntax.Members, syntax.Usings, model);

      //   return newItem;
      //}

      //public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      //{
      //   var itemAsT = item as IRoot;
      //   var node = SyntaxFactory.CompilationUnit();
      //   var usingsSyntax = itemAsT.UsingDirectives
      //               .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
      //               .ToList();
      //   var membersSyntax = itemAsT.StemMembers
      //               .Where(x => !(x is IUsingDirective))
      //               .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
      //               .ToList();
      //   node = node.WithUsings(SyntaxFactory.List(usingsSyntax));
      //   node = node.WithMembers(SyntaxFactory.List(membersSyntax));
      //   return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      //}

      public IRootGroup CreateFrom(Compilation compilation, bool skipDetail)
      {
         var csharpCompilation = compilation as CSharpCompilation;
         if (csharpCompilation == null) { throw new InvalidOperationException(); }
         var ret = new RDomRootGroup(compilation);
         foreach (var tree in compilation.SyntaxTrees)
         {
            var model = compilation.GetSemanticModel(tree);
            var root = _corporation.Create(tree.GetCompilationUnitRoot(), null, model).FirstOrDefault() as IRoot;
            ret.Roots.AddOrMove(root);
         }
         return ret;
      }

      public IEnumerable<SyntaxTree> GetCompilation(IRootGroup rootGroup)
      {
         throw new NotImplementedException();
      }
   }


}
