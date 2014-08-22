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
    public class CommentWhiteFactory : RDomMiscFactory<ICommentWhite, SyntaxNode>
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

        protected override IEnumerable<IMisc> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return InternalCreateFrom(syntaxNode, parent, model);
        }

        private IEnumerable<ICommentWhite> InternalCreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
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
                            ret.Add( MakeComment(syntaxNode,  precedingTrivia, trivia, false));
                            lastWasComment = true;
                            break;
                        case SyntaxKind.MultiLineCommentTrivia:
                            ret.Add(MakeComment(syntaxNode, precedingTrivia, trivia, true));
                            lastWasComment = true;
                            break;
                        default:
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
