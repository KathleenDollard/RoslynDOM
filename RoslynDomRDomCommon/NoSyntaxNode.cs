using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace RoslynDomRDomCommon
{
    /// <summary>
    /// This class is used as a type parameter for items that have no meaningful syntax node -
    /// at least IPublicAnnotations, ICommentWhite, IReferencedType, and IStructuredDocumentation 
    /// </summary>
    public class NoSyntaxNode : SyntaxNode
    {
        public override string Language
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override string KindText
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override SyntaxTree SyntaxTreeCore
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal override AbstractSyntaxNavigator Navigator { }
 
        public override SyntaxNodeOrToken ChildThatContainsPosition(int position)
        {
            throw new NotImplementedException();
        }

        public override void SerializeTo(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public override string ToFullString()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public override void WriteTo(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        protected override bool EquivalentToCore(SyntaxNode other)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxToken FindTokenCore(int position, Func<SyntaxTrivia, bool> stepInto)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxToken FindTokenCore(int position, bool findInsideTrivia)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxTrivia FindTriviaCore(int position, bool findInsideTrivia)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode InsertNodesInListCore(SyntaxNode nodeInList, IEnumerable<SyntaxNode> nodesToInsert, bool insertBefore)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode InsertTokensInListCore(SyntaxToken originalToken, IEnumerable<SyntaxToken> newTokens, bool insertBefore)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode InsertTriviaInListCore(SyntaxTrivia originalTrivia, IEnumerable<SyntaxTrivia> newTrivia, bool insertBefore)
        {
            throw new NotImplementedException();
        }

        protected override bool IsEquivalentToCore(SyntaxNode node, bool topLevel = false)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode NormalizeWhitespaceCore(string indentation, bool elasticTrivia)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode RemoveNodesCore(IEnumerable<SyntaxNode> nodes, SyntaxRemoveOptions options)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode ReplaceCore<TNode>(IEnumerable<TNode> nodes = null, Func<TNode, TNode, SyntaxNode> computeReplacementNode = null, IEnumerable<SyntaxToken> tokens = null, Func<SyntaxToken, SyntaxToken, SyntaxToken> computeReplacementToken = null, IEnumerable<SyntaxTrivia> trivia = null, Func<SyntaxTrivia, SyntaxTrivia, SyntaxTrivia> computeReplacementTrivia = null)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode ReplaceNodeInListCore(SyntaxNode originalNode, IEnumerable<SyntaxNode> replacementNodes)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode ReplaceTokenInListCore(SyntaxToken originalToken, IEnumerable<SyntaxToken> newTokens)
        {
            throw new NotImplementedException();
        }

        protected override SyntaxNode ReplaceTriviaInListCore(SyntaxTrivia originalTrivia, IEnumerable<SyntaxTrivia> newTrivia)
        {
            throw new NotImplementedException();
        }

        internal override SyntaxNode GetCachedSlot(int index)
        {
            throw new NotImplementedException();
        }

  
        internal override SyntaxNode GetNodeSlot(int slot)
        {
            throw new NotImplementedException();
        }

  
    }
}
