using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomPropertyAccessorMiscFactory
          : RDomMiscFactory<IAccessor, AccessorDeclarationSyntax>
    {
        public override void InitializeItem(IAccessor newItem, AccessorDeclarationSyntax syntax)
        {
            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            if (syntax.Body != null)
            {
                var statements = ListUtilities.MakeList(syntax, x => x.Body.Statements, x => RDomFactoryHelper.StatementFactoryHelper.MakeItem(x));
                foreach (var statement in statements)
                { newItem.AddStatement(statement); }
            }
        }
    }

 }
