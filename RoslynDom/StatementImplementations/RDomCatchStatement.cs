using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomCatchStatement : RDomStatementBlockBase<ICatchStatement>, ICatchStatement
    {
        public RDomCatchStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomCatchStatement(RDomCatchStatement oldRDom)
            : base(oldRDom)
        {
            if (oldRDom.Condition != null)
            { Condition = oldRDom.Condition.Copy(); }
            if (oldRDom.Variable != null)
            { Variable = oldRDom.Variable.Copy(); }
            if (oldRDom.ExceptionType != null)
            { ExceptionType = oldRDom.ExceptionType.Copy(); }
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = new List<IDom>();
                if (this.Variable != null)
                { list.Add(this.Variable); }
                else if (this.ExceptionType != null)
                { list.Add(this.ExceptionType); }
                if (Condition != null)
                { list.Add(Condition); }
                list.AddRange(base.Children.ToList());
                return list;
            }
        }

        public IExpression Condition { get; set; }
        public IVariableDeclaration Variable { get; set; }
        public IReferencedType  ExceptionType { get; set; }
    }
}
