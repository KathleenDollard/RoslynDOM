using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomThrowStatement : RDomBase<IThrowStatement, ISymbol>, IThrowStatement
    {

        public RDomThrowStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {
            Initialize();
        }

        internal RDomThrowStatement(RDomThrowStatement oldRDom)
            : base(oldRDom)
        {
            ExceptionExpression = oldRDom.ExceptionExpression.Copy();
           
        }

        public IExpression ExceptionExpression { get; set; }

    }
}
