using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomBaseLoop<T>
        : RDomBase<T, ISymbol>, IStatement
        where T : class, ILoop<T>, IStatement
    {
        private RDomList<IStatement> _statements ;

        internal RDomBaseLoop(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {  }


        internal RDomBaseLoop(T oldRDom)
             : base(oldRDom)
        {
            Statements.AddOrMoveRange(oldRDom.Statements);
            Condition = oldRDom.Condition.Copy();
            HasBlock = oldRDom.HasBlock;
            TestAtEnd = oldRDom.TestAtEnd;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _statements = new RDomList<IStatement>(this);
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = base.Children.ToList();
                list.Add(Condition);
                list.AddRange(Statements);
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = base.Descendants.ToList();
                list.AddRange(Condition.DescendantsAndSelf);
                foreach (var statement in Statements)
                { list.AddRange(statement.DescendantsAndSelf); }
                return list;
            }
        }

        public IExpression Condition { get; set; }

        public bool HasBlock { get; set; }
        public bool TestAtEnd { get; set; }

        //public void RemoveStatement(IStatement statement)
        //{ _statements.Remove(statement); }

        //public void AddOrMoveStatement(IStatement statement)
        //{ _statements.Add(statement); }

        public RDomList<IStatement> Statements
        { get { return _statements; } }

    }
}

