using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomPropertyAccessorMiscFactory
          : RDomMiscFactory<RDomPropertyAccessor, AccessorDeclarationSyntax>
    {
        public override IEnumerable<IMisc> CreateFrom(SyntaxNode syntaxNode, SemanticModel model)
        {
            var syntax = syntaxNode as AccessorDeclarationSyntax;
            var newItem = new RDomPropertyAccessor(syntaxNode, model);

            var attributes = RDomFactoryHelper.GetAttributesFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            if (syntax.Body != null)
            {
                var statements = ListUtilities.MakeList(syntax, x => x.Body.Statements, x => RDomFactoryHelper.GetHelper<IStatement>().MakeItem(x, model));
                foreach (var statement in statements)
                { newItem.AddStatement(statement); }
            }

            return new IMisc[] { newItem };

        }
    }

}
