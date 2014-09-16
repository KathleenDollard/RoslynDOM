﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomIfStatement : RDomStatementBlockBase<IIfStatement>, IIfStatement
    {
        private RDomCollection<IElseBaseStatement> _elses;

        public RDomIfStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomIfStatement(RDomIfStatement oldRDom)
            : base(oldRDom)
        {
            Initialize();
            var newElses = RoslynDomUtilities.CopyMembers(oldRDom.Elses);
            Elses.AddOrMoveRange(newElses);
            Condition = oldRDom.Condition.Copy();
        }

        protected void Initialize()
        {
            _elses = new RDomCollection<IElseBaseStatement>(this);
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

        //public override IEnumerable<IDom> Descendants
        //{
        //    get
        //    {
        //        var list = new List<IDom>();
        //        list.AddRange(Condition.DescendantsAndSelf);
        //        list.AddRange(base.Descendants.ToList());
        //        foreach (var elseif in Elses)
        //        { list.AddRange(elseif.DescendantsAndSelf); }
        //        return list;
        //    }
        //}

        public RDomCollection<IElseBaseStatement> Elses
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
        { get { return Elses.OfType<IElseIfStatement>().ToList(); } }
    }
}
