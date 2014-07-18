using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomForStatement : RDomBaseLoop<IForStatement>, IForStatement
    {

        public RDomForStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomForStatement(RDomForStatement oldRDom)
            : base(oldRDom)
        {
            Incrementor = oldRDom.Incrementor.Copy();
            Variable = oldRDom.Variable.Copy();
        }

        public IExpression Incrementor { get; set; }
       

        public IVariableDeclaration Variable { get; set; }
   
    }
}
