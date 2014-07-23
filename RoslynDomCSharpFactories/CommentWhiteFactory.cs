using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class CommentWhiteFactory : RDomMiscFactory<RDomCommentWhite, SyntaxNode>, ICommentWhiteFactory
    {
        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            // Is only called directly, not as an IMisc (RDomFactoryHelper.GetFromFactory<ICommentWhite>)
            return false;
        }

        protected override IEnumerable<IMisc> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return InternalCreateFrom(syntaxNode, parent, model);
        }

        IEnumerable<ICommentWhite> IRDomFactory<ICommentWhite>.CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return InternalCreateFrom(syntaxNode, parent, model);
        }

        private IEnumerable<ICommentWhite> InternalCreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = new List<ICommentWhite>();
            if (!syntaxNode.HasLeadingTrivia) return ret;
            var triviaList = syntaxNode.GetLeadingTrivia();
            var lastWasComment = false;
            foreach (var trivia in triviaList)
            {
                // This is ugly, but we assume comments stand on their own lines. 
                if (lastWasComment && trivia.CSharpKind() == SyntaxKind.EndOfLineTrivia) continue;
                lastWasComment = false;
                switch (trivia.CSharpKind())
                {
                    case SyntaxKind.EndOfLineTrivia:
                        ret.Add(new RDomVerticalWhitespace(1, false));
                        break;
                    case SyntaxKind.SingleLineCommentTrivia:
                        ret.Add(new RDomComment(ExtractComment(trivia, syntaxNode), false));
                        lastWasComment = true;
                        break;
                    case SyntaxKind.MultiLineCommentTrivia:
                        ret.Add(new RDomComment(ExtractComment(trivia, syntaxNode), true));
                        lastWasComment = true;
                        break;
                    default:
                        break;
                }

            }
            return ret;
        }

        private string ExtractComment(SyntaxTrivia trivia, SyntaxNode node)
        {
            var text = node.SyntaxTree.GetText().ToString(trivia.Span).Trim();
            if (text.StartsWith("//")) { text = text.Substring(2).Trim(); }
            if (text.StartsWith("/*"))
            {
                text = text.Substring(2);
                if (text.EndsWith("*/")) { text = text.Substring(0, text.Length - 2).Trim(); }
            }
            return text;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IMisc item)
        {
            return null;
            //return BuildSyntax((IAttribute)item);
        }

        public IEnumerable<SyntaxNode> BuildSyntax(ICommentWhite item)
        {
            return null;
            //var itemAsT = item as IAttribute;
            //var nameSyntax = SyntaxFactory.ParseName(itemAsT.Name);
            //var arguments = new SeparatedSyntaxList<AttributeArgumentSyntax>();
            //var values = itemAsT.AttributeValues.Select(x => BuildAttributeValueSyntax(x));
            //arguments = arguments.AddRange(values.OfType<AttributeArgumentSyntax>().ToList());
            //var argumentList = SyntaxFactory.AttributeArgumentList(arguments);
            //var node = SyntaxFactory.Attribute(nameSyntax, argumentList);

            //return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }

        //public IEnumerable<SyntaxNode> BuildSyntax(AttributeList attributeList)
        //{
        //    var list = SyntaxFactory.List<SyntaxNode>();
        //    var attributes = attributeList.Attributes;
        //    if (attributes.Any())
        //    {
        //        var attribList = SyntaxFactory.AttributeList();
        //        var attributeSyntaxItems = attributes.SelectMany(x => BuildSyntax(x)).ToArray();
        //        // TODO: attributeSyntaxItems = attributeSyntaxItems.Select(x => x.Format()).ToArray();
        //        attribList = attribList.AddAttributes(attributeSyntaxItems.OfType<AttributeSyntax>().ToArray());
        //        list = list.Add(attribList);
        //    }
        //    return list;
        //}

    }
}
