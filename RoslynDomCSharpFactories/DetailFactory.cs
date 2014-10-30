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
   public class DetailFactory : RDomBaseSyntaxTriviaFactory<IDetail>
   {
      // TODO: Consider IOC for trivia manager and PublicAnnotationMatch
      private TriviaManager triviaManager = new TriviaManager();
      //private IPublicAnnotationFactory _publicAnnotationFactory;

      public override IDom CreateFrom(SyntaxTrivia trivia, IDom parent, OutputContext context)
      {
         if (trivia.CSharpKind() == SyntaxKind.RegionDirectiveTrivia) { return CreateStartRegion(trivia, parent, context); }
         else if (trivia.CSharpKind() == SyntaxKind.EndRegionDirectiveTrivia) { return CreateEndRegion(trivia, parent, context); }
         return CreateComment(trivia, parent, context);
      }

      private static IDom CreateStartRegion(SyntaxTrivia trivia, IDom parent, OutputContext context)
      {
         if (!trivia.HasStructure) return null;
         var structure = trivia.GetStructure();
         var regionSyntax = structure as RegionDirectiveTriviaSyntax;
         var text = regionSyntax.EndOfDirectiveToken.ToFullString().Replace("\r\n", "");
         var newRegion = new RDomDetailBlockStart(trivia, text);
         return newRegion;
      }

      private static IDom CreateEndRegion(SyntaxTrivia trivia, IDom parent, OutputContext context)
      {
         if (!trivia.HasStructure) return null;
         var structure = trivia.GetStructure();
         var regionSyntax = structure as EndRegionDirectiveTriviaSyntax;
         var startDirectives = regionSyntax
                                 .GetRelatedDirectives()
                                 .Where(x => x is RegionDirectiveTriviaSyntax);
         if (startDirectives.Count() != 1) { throw new NotImplementedException(); }
         var startSyntax = startDirectives.Single();
         var container = parent as IContainer;
         if (container == null) { throw new NotImplementedException(); }
         var startBlock = container.GetMembers()
                           .OfType<RDomDetailBlockStart>()
                           .Where(x => MatchEndRegion(startSyntax, x.TypedTrivia))
                           .SingleOrDefault();
         var newRegion = new RDomRegionEnd(trivia, startBlock);
         return newRegion;
      }

      private static bool MatchEndRegion(SyntaxNode startSyntax, SyntaxTrivia typedTrivia)
      {
         var structure = typedTrivia.GetStructure();
         if (structure == null) throw new InvalidOperationException();
         return (structure == startSyntax);
      }

      private IDom CreateComment(SyntaxTrivia trivia, IDom parent, OutputContext context)
      {
         var isMultiline = (trivia.CSharpKind() == SyntaxKind.MultiLineCommentTrivia);
         var precedingTrivia = trivia.Token.LeadingTrivia.PreviousSiblings(trivia);
         var commentText = trivia.ToString();
         var tuple = context.Corporation.CreateFromWorker.ExtractComment(trivia.ToString());
         var commentString = tuple.Item2;
         var newComment = new RDomComment(trivia, tuple.Item2, isMultiline);
         triviaManager.StoreWhitespaceForComment(newComment, precedingTrivia, tuple.Item1, tuple.Item3);
         return newComment;
      }

      public override IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IDom item, OutputContext context)
      {
         var itemAsComment = item as IComment;
         if (itemAsComment != null) return BuildCommentSyntaxTrivia(itemAsComment);
         var itemAsStartBlock = item as IDetailBlockStart;
         if (itemAsStartBlock != null) return BuildBlockStartSyntaxTrivia(itemAsStartBlock);
         var itemAsEndBlock = item as IDetailBlockEnd;
         if (itemAsEndBlock != null) return BuildBlockEndSyntaxTrivia(itemAsEndBlock);
         var itemAsPublicAnnotation = item as IPublicAnnotation;
         if (itemAsPublicAnnotation != null)
         {
            if (context.SkipPublicAnnotationsOnOutput)
            { return new SyntaxTrivia[] { }; }
            return BuildPublicAnnotationSyntaxTrivia(itemAsPublicAnnotation);
         }
         throw new NotImplementedException();
      }

      private IEnumerable<SyntaxTrivia> BuildPublicAnnotationSyntaxTrivia(IPublicAnnotation itemAsPublicAnnotation)
      {
         throw new NotImplementedException();
      }

      private IEnumerable<SyntaxTrivia> BuildBlockEndSyntaxTrivia(IDetailBlockEnd itemAsEndBlock)
      {
         if (itemAsEndBlock.BlockStyleName.Equals("region", StringComparison.OrdinalIgnoreCase))
         {
            var trivia = SyntaxFactory.Trivia(SyntaxFactory.EndRegionDirectiveTrivia(true));
            return new SyntaxTrivia[] { trivia };
         }
         throw new NotImplementedException();
      }

      private IEnumerable<SyntaxTrivia> BuildBlockStartSyntaxTrivia(IDetailBlockStart itemAsStartBlock)
      {
         if (itemAsStartBlock.BlockStyleName.Equals("region", StringComparison.OrdinalIgnoreCase))
         {

            var message = SyntaxFactory.PreprocessingMessage(itemAsStartBlock.Text);
            var token = SyntaxFactory.Token(SyntaxFactory.TriviaList(message),
                        SyntaxKind.EndOfDirectiveToken,
                        SyntaxFactory.TriviaList());

            var directive = SyntaxFactory.RegionDirectiveTrivia(true)
                     .WithEndOfDirectiveToken(token)
                     .NormalizeWhitespace();
            return new SyntaxTrivia[] { SyntaxFactory.Trivia(directive) };
         }
         throw new NotImplementedException();
      }

      private IEnumerable<SyntaxTrivia> BuildCommentSyntaxTrivia(IComment itemAsComment)
      {
         var ret = new List<SyntaxTrivia>();
         if (itemAsComment == null) return ret;
         Guardian.Assert.IsNotNull(itemAsComment, nameof(itemAsComment));
         var innerWs = itemAsComment.Whitespace2Set[LanguagePart.Inner, LanguageElement.Comment];
         var comment = innerWs.LeadingWhitespace + itemAsComment.Text + innerWs.TrailingWhitespace;
         if (itemAsComment.IsMultiline) { comment = "/*" + comment + "*/"; }
         else { comment = "//" + comment; }
         var commentSyntax = SyntaxFactory.Comment(comment);

         // Assume just one whitespace
         var whitespace = itemAsComment.Whitespace2Set.FirstOrDefault();
         if (whitespace != null)
         {
            // for now assume only whitespace before and newline after
            ret.Add(SyntaxFactory.Whitespace(whitespace.LeadingWhitespace));
            ret.Add(commentSyntax);
         }
         else
         { ret.Add(commentSyntax); }
         ret.Add(SyntaxFactory.EndOfLine("\r\n"));
         return ret;
      }

   }
}
