using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomVariableDeclaration : RDomBaseVariable
    {
        public RDomVariableDeclaration(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomVariableDeclaration(IVariableDeclaration oldRDom)
             : base(oldRDom)
        { }
         
    }
}
