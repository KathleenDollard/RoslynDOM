using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomIfStatement : RDomIfBaseStatement<IIfStatement>, IIfStatement
    {
        private RDomList<IElseStatement> _elses;

        public RDomIfStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomIfStatement(RDomIfStatement oldRDom)
            : base(oldRDom)
        {
            // Initialize called in base
            var newElses = RoslynDomUtilities.CopyMembers(oldRDom.Elses);
            Elses.AddOrMoveRange(newElses);
            Condition = oldRDom.Condition.Copy();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _elses = new RDomList<IElseStatement>(this);
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = new List<IDom>();
                list.Add(Condition);
                list.AddRange(base.Children.ToList());
                list.AddRange(Elses);
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
                foreach (var elseif in Elses)
                { list.AddRange(elseif.DescendantsAndSelf); }
                return list;
            }
        }

        public RDomList<IElseStatement> Elses
        { get { return _elses; } }

        public IExpression Condition { get; set; }

        public IFinalElseStatement Else
        {
            get
            {
                var candidates = Elses.OfType<IFinalElseStatement>();
                switch (candidates.Count())
                {
                    case 0:
                        return null;
                    case 1:
                        return candidates.First();
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public IEnumerable<IElseIfStatement> ElseIfs
        { get { return Elses.OfType<IElseIfStatement>(); } }
    }
}
