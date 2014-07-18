using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomForEachStatement : RDomBaseLoop<IForEachStatement>, IForEachStatement
    {

        public RDomForEachStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomForEachStatement(RDomForEachStatement oldRDom)
            : base(oldRDom)
        {
            Variable = oldRDom.Variable;
        }

        public IVariableDeclaration Variable { get;set; }
          }
}
