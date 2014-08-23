using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomDeclarationStatement : RDomBaseVariable, IDeclarationStatement 
    {
        public RDomDeclarationStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomDeclarationStatement(RDomDeclarationStatement oldRDom)
             : base(oldRDom)
        { }

        public bool IsConst { get; set; }

           IDeclarationStatement IDom<IDeclarationStatement>.Copy()
        {
            return (IDeclarationStatement)base.Copy();
        }

        public override string ToString()
        {
            return base.ToString() + " {" + Type.Name + "}";
        }
    }
}
