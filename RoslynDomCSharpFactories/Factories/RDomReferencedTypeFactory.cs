using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomReferencedTypeMiscFactory
           : RDomMiscFactory<RDomReferencedType, FieldDeclarationSyntax>
    {
        // I'm still evolving how types are handled.
        public override IEnumerable<SyntaxNode> BuildSyntax(IMisc item)
        {
            var node =  SyntaxFactory.ParseTypeName(item.Name);
            // TODO: return new SyntaxNode[] { node.Format() };
            return new SyntaxNode[] { node };
        }

        public override IEnumerable<IMisc> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            // Not currently used
            throw new NotImplementedException();
        }
    }

}
