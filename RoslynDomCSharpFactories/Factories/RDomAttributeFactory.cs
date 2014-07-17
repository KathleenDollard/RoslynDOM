using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttributeMiscFactory
          : RDomMiscFactory<RDomAttribute, AttributeListSyntax>
    {
        public override IEnumerable<IMisc> CreateFrom(SyntaxNode syntaxNode, SemanticModel model)
        {
            var syntax = syntaxNode as AttributeListSyntax;
            var newItem = new RDomMethod(syntaxNode, model);

            newItem.Name = ((AttributeSyntax)TypedSyntax).Name.ToString();

            return new IMisc[] { newItem };
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IMisc item)
        {
            var itemAsT = item as IAttribute;
            var nameSyntax = SyntaxFactory.ParseName(itemAsT.Name);
            var arguments = new SeparatedSyntaxList<AttributeArgumentSyntax>();
            var values  = itemAsT.AttributeValues.Select(x => ((RDomAttributeValue)x).BuildSyntax());
            arguments = arguments.AddRange(values);
            var argumentList = SyntaxFactory.AttributeArgumentList(arguments);
            var node = SyntaxFactory.Attribute(nameSyntax, argumentList);

            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }


    }


}
