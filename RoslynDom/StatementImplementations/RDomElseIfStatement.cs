using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomElseIfStatement : RDomIfBaseStatement<IElseIfStatement>, IElseIfStatement
    {
        public RDomElseIfStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomElseIfStatement(RDomElseIfStatement oldRDom)
            : base(oldRDom)
        {
            Condition = oldRDom.Condition.Copy();
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = new List<IDom>();
                list.Add(Condition);
                list.AddRange(base.Children.ToList());
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = new List<IDom>();
                list.AddRange(Condition.DescendantsAndSelf);
                list.AddRange(base.Descendants.ToList());
                return list;
            }
        }

        public IExpression Condition { get; set; }


        //IElseStatement IDom<IElseStatement>.Copy()
        //{
        //    // The current interface implementation - where ElseIf is also an Else - means we need this, even though it probably will never be callsed
        //    return ((IElseIfStatement)this).Copy();
        //}
    }
}
