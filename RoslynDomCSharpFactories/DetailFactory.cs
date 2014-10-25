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
   /// <summary>
   /// Factory for working with SyntaxTrivia on nodes. It should, perhaps, be called TriviaFactory
   /// </summary>
   /// <remarks>
   /// There are two kinds of trivia - attached and member. The concept of trivia in C#/VB is
   /// quite different than the human understanding. 
   /// <para/>
   /// Structured documentation and of end of line comments should both be attached to 
   /// other IDom elements, but to the full item (the class, not the first keyword for example).
   /// <para/> 
   /// Vertical whitespace, comments, and 
   /// public annotations (comments with structure intended for compilers) and directives
   /// all float - they are not inherently related to other IDom items. 
   /// <para/>
   /// Floating annotations further belong in three groups 
   /// <list type="">
   /// <item>vertical whitespace is only a number of lines</item>
   /// <item>comments and public annotations which provide data at a specific spot</item>
   /// <item>blocks, like regions, that contain information and a corresponding block of items</item>
   /// <item>conditions, like #if/#elif that involve multiple blocks</item>
   /// </list>
   /// It is fundamenatal to the RoslynDom design that it doesn't matter whether something is defined 
   /// in the normal C# way or a special way - blocks could be reigons, or they could be defined with 
   /// special public start/stop annotations
   /// <para/>
   /// Horizontal whitespace attaches to elements, so is handled differently
   /// </remarks>
   public class DetailFactory : RDomBaseItemFactory<IDetail, SyntaxNode>
   {
      // TODO: Consider IOC for trivia manager and PublicAnnotationMatch
      private TriviaManager triviaManager = new TriviaManager();
      //private IPublicAnnotationFactory _publicAnnotationFactory;

      public DetailFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return 0; } }

      public override Type[] SyntaxNodeTypes
      { get { return null; } }

      public override Type[] ExplicitNodeTypes
      { get { return new Type[] { typeof(IDetail) }; } }

      protected override IEnumerable<IDom> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         // The parent of the syntax is the next item. The parent of the region is the thing it's attached to
         parent = parent.Parent;
         var ret = new List<IDetail>();
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

      private IDetail MakeComment(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia,
           SyntaxTrivia trivia, bool isMultiline)
      {
         var commentText = trivia.ToString();
         var tuple = ExtractComment(trivia.ToString());
         var commentString = tuple.Item2;
         var publicAnnotation = OutputContext.Corporation.GetTriviaFactory<IPublicAnnotation>().CreateFrom(commentString, OutputContext.Corporation);
         if (publicAnnotation != null) return publicAnnotation;
         var newComment = new RDomComment(tuple.Item2, isMultiline);
         triviaManager.StoreWhitespaceForComment(newComment, precedingTrivia, tuple.Item1, tuple.Item3);
         return newComment;
      }

      private IBlockStartDetail MakeRegion(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia, SyntaxTrivia trivia, IDom parent)
      {
         if (!trivia.HasStructure) return null;
         var structure = trivia.GetStructure();
         var regionSyntax = structure as RegionDirectiveTriviaSyntax;
         var text = regionSyntax.EndOfDirectiveToken.ToFullString().Replace("\r\n", "");
         var newRegion = new RDomRegionStart(regionSyntax, parent, null, text);
         return newRegion;
      }

      private IBlockEndDetail MakeEndRegion(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia, SyntaxTrivia trivia, IDom parent)
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
