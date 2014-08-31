﻿using System;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomExpression : RDomBase<IExpression, ISymbol>, IExpression
    {
        public RDomExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomExpression(RDomExpression oldRDom)
            : base(oldRDom)
        {
            Expression = oldRDom.Expression;
            ExpressionType = oldRDom.ExpressionType;
        }

        public string Expression { get; set; }

        public ExpressionType ExpressionType { get; set; }
    
    }
}
