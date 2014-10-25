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

         public override IDom CreateFrom(SyntaxTrivia trivia, OutputContext context)
      {
         var isMultiline = (trivia.CSharpKind() == SyntaxKind.MultiLineCommentTrivia);
         var precedingTrivia = trivia.Token.LeadingTrivia.PreviousSiblings(trivia);
         var commentText = trivia.ToString();
         var tuple = context.Corporation.CreateFromWorker.ExtractComment(trivia.ToString());
         var commentString = tuple.Item2;
         var newComment = new RDomComment(tuple.Item2, isMultiline);
         triviaManager.StoreWhitespaceForComment(newComment, precedingTrivia, tuple.Item1, tuple.Item3);
         return newComment;
      }

      public override IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IDom item, OutputContext context)
      {
         var trivias = BuildCommentSyntaxTrivia(item as IComment);
         return trivias;
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
