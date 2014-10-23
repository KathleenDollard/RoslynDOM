using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class CommentWhiteFactory : RDomBaseItemFactory<ICommentWhite, SyntaxNode>
   {
      // TODO: Consider IOC for trivia manager
      private TriviaManager triviaManager = new TriviaManager();

      public CommentWhiteFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return 0; } }

      public override bool CanCreateFrom(SyntaxNode syntaxNode)
      { return true; }

      public override Type[] SyntaxNodeTypes
      { get { return null; } }

      public override Type[] ExplicitNodeTypes
      { get { return new Type[] { typeof(ICommentWhite) }; } }

      protected override IEnumerable<IDom> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         return InternalCreateFrom(syntaxNode, parent, model);
      }

      private IEnumerable<ICommentWhite> InternalCreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         // The parent of the syntax is the next item. The parent of the region is the thing it's attached to
         parent = parent.Parent;
         var ret = new List<ICommentWhite>();
         if (!syntaxNode.HasLeadingTrivia) return ret;
         var triviaList = syntaxNode.GetLeadingTrivia();
         var lastWasComment = false;
         var precedingTrivia = new List<SyntaxTrivia>();
         foreach (var trivia in triviaList)
         {
            // This is ugly, but we assume comments stand on their own lines. 
            var skip = (lastWasComment && trivia.CSharpKind() == SyntaxKind.EndOfLineTrivia);
            lastWasComment = false;
            if (!skip)
            {
               switch (trivia.CSharpKind())
               {
                  case SyntaxKind.EndOfLineTrivia:
                     // TODO: Consider whether leading WS on a vert whitespace matters
                     ret.Add(new RDomVerticalWhitespace(1, false));
                     break;
                  case SyntaxKind.SingleLineCommentTrivia:
                     ret.Add(MakeComment(syntaxNode, precedingTrivia, trivia, false));
                     lastWasComment = true;
                     break;
                  case SyntaxKind.MultiLineCommentTrivia:
                     ret.Add(MakeComment(syntaxNode, precedingTrivia, trivia, true));
                     lastWasComment = true;
                     break;
                  case SyntaxKind.RegionDirectiveTrivia:
                     ret.Add(MakeRegion(syntaxNode, precedingTrivia, trivia, parent));
                     //lastWasComment = true;
                     break;
                  case SyntaxKind.EndRegionDirectiveTrivia:
                     ret.Add(MakeEndRegion(syntaxNode, precedingTrivia, trivia, parent));
                     //lastWasComment = true;
                     break;
               }
            }
            precedingTrivia.Add(trivia);
         }
         return ret;
      }

      private RDomComment MakeComment(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia,
              SyntaxTrivia trivia, bool isMultiline)
      {
         var commentText = trivia.ToString();
         var tuple = ExtractComment(trivia.ToString());
         var newComment = new RDomComment(tuple.Item2, isMultiline);
         triviaManager.StoreWhitespaceForComment(newComment, precedingTrivia, tuple.Item1, tuple.Item3);
         return newComment;
      }

      private IRegionStart MakeRegion(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia, SyntaxTrivia trivia, IDom parent)
      {
         if (!trivia.HasStructure) return null;
         var structure = trivia.GetStructure();
         var regionSyntax = structure as RegionDirectiveTriviaSyntax;
         var text = regionSyntax.EndOfDirectiveToken.ToFullString().Replace("\r\n", "");
         var newRegion = new RDomRegionStart(regionSyntax, parent, null, text);
         return newRegion;
      }

      private IRegionEnd MakeEndRegion(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia, SyntaxTrivia trivia, IDom parent)
      {
         if (!trivia.HasStructure) return null;
         var structure = trivia.GetStructure();
         var regionSyntax = structure as EndRegionDirectiveTriviaSyntax;
         var startDirectives = regionSyntax
                                 .GetRelatedDirectives()
                                 .Where(x => x is RegionDirectiveTriviaSyntax);
         if (startDirectives.Count() != 1) { throw new NotImplementedException(); }
         var startSyntax = startDirectives.Single();
         var newRegion = new RDomRegionEnd(regionSyntax, parent, null, startSyntax);
         return newRegion;
      }

      private Tuple<string, string, string> ExtractComment(string text)
      {

         if (text.StartsWith("//")) { text = text.Substring(2); }
         if (text.StartsWith("/*"))
         {
            text = text.Substring(2);
            if (text.EndsWith("*/"))
            { text = text.Substring(0, text.Length - 2); }
         }
         // TODO: Ensure you test with whitespace only comment of both types
         var trailing = text.SubstringAfter(text.TrimEnd());
         var leading = text.SubstringBefore(text.TrimStart());
         return Tuple.Create(leading, text.Trim(), trailing);
      }

      [ExcludeFromCodeCoverage]
      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         return null;
      }

   }
}
