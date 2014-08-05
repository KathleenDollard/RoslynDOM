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
           : RDomMiscFactory<RDomReferencedType, SyntaxNode>
    {
        public RDomReferencedTypeMiscFactory(RDomCorporation corporation)
            :base (corporation)
        { }

                  // I'm still evolving how types are handled.
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IReferencedType;
            var node =  SyntaxFactory.ParseTypeName(itemAsT.Name);
            return node.PrepareForBuildSyntaxOutput(item);
        }

        //protected  override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        //{
        //    // Not currently used
        //    throw new NotImplementedException();
        //}
    }

}
