using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.IO;

namespace RoslynDom.CSharp
{
   public class RDomRootFactory
         : RDomBaseSyntaxNodeFactory<RDomRoot, CompilationUnitSyntax>
   {

      public RDomRootFactory(RDomCorporation corporation)
       : base(corporation)
      { }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as CompilationUnitSyntax;
         var newItem = new RDomRoot(OutputContext.Corporation.FactoryAccess, syntaxNode, parent, model);
         // Root does not call StandardInitialize because the info is attched to the first item
         // and particularly, whitespace would be doubled. 
         //CreateFromWorker.InitializePublicAnnotations(newItem,  syntaxNode,  parent,  model);

  

         newItem.FilePath = syntax.SyntaxTree.FilePath;
         newItem.Name = Path.GetFileNameWithoutExtension(newItem.FilePath);
         CreateFromWorker.LoadStemMembers(newItem, syntax.Members, syntax.Usings, model);

         var triviaList = EofTrivia(syntax, newItem, model);
         newItem.StemMembersAll.AddOrMoveRange(triviaList);
         return newItem;
      }

      private IEnumerable<IDetail> EofTrivia(CompilationUnitSyntax syntax, RDomRoot newItem, SemanticModel model)
      {
         // Special case for end of file end region
         var ret = new List<IDetail>();
         var eofTokens = syntax.ChildTokens()
                     .Where(x => x.CSharpKind() == SyntaxKind.EndOfFileToken);
         if (eofTokens.Any())
         {
            var eof = eofTokens.FirstOrDefault();
            if (eof.HasLeadingTrivia)
            {
               var trivia = eof.LeadingTrivia;
               ret.AddRange(((ICSharpCreateFromWorker)OutputContext.Corporation.CreateFromWorker)
                        .GetDetail(syntax, trivia, newItem, model, OutputContext));
            }
         }
         return ret;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IRoot;
         var node = SyntaxFactory.CompilationUnit();
         var usingsSyntax = itemAsT.UsingDirectives
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .ToList();
         var membersSyntax = itemAsT.StemMembers
                     .Where(x => !(x is IUsingDirective))
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .ToList();
         node = node.WithUsings(SyntaxFactory.List(usingsSyntax));
         node = node.WithMembers(SyntaxFactory.List(membersSyntax));
         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }
   }


}
