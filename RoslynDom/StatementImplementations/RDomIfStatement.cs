using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomIfStatementFactory
        : RDomStatementFactory<RDomIfStatement, IfStatementSyntax>
    {
        public RDomIfStatementFactory(RDomFactoryHelper helper)
            : base( helper)
        { }
    }

    public class RDomIfStatement : RDomBase<IIfStatement, IfStatementSyntax, ISymbol>, IIfStatement
    {
        private IList<IIfStatement> _elses = new List<IIfStatement>();
            private IList<IStatement> _statements = new List<IStatement>();

        internal RDomIfStatement(
             IfStatementSyntax rawItem,
               IEnumerable<PublicAnnotation> publicAnnotations)
           : base(rawItem,  publicAnnotations)
        {
           
        }

          internal RDomIfStatement(RDomIfStatement oldRDom)
             : base(oldRDom)
        {
            var newElses = RoslynDomUtilities.CopyMembers(oldRDom._elses);
            foreach (var elseItem in newElses)
            { AddOrMoveElse(elseItem); }
        }

        protected override void Initialize()
        {
            base.Initialize();
            //HasBlock = hasBlock;
            //if (elses != null)
            //{
            //    foreach (var elseItem in elses)
            //    { AddOrMoveElse(elseItem); }
            //}
        }

        public override IfStatementSyntax BuildSyntax()
        {
            // TODO: Current work KAD
            //var nameSyntax = SyntaxFactory.Identifier(Name);
            //var statementsAsSyntax = Statements.Select(x => ((RDomStatement)x).BuildSyntax()).ToArray();
            //StatementSyntax statement;
            //if (HasBlock || this.Statements.Count() > 1)
            //{ statement = SyntaxFactory.Block(statementsAsSyntax); }
            //else if (this.Statements.Count() == 1)
            //{ statement = statementsAsSyntax.First(); }
            //else
            //{ statement = SyntaxFactory.EmptyStatement(); }
            //var condition = (Condition as RDomCondition).BuildSyntax();
            //var node = SyntaxFactory.IfStatement(condition, statement);

            //node = RoslynUtilities.UpdateNodeIfItemNotNull(BuildElseSyntax(), node, (n, item) => n.WithElse(item));

            //return (StatementSyntax)RoslynUtilities.Format(node);
            return null;
        }

     

        public ICondition Condition { get; set; }

        public bool HasBlock { get; set; }

        public void RemoveElse(IIfStatement statement)
        { _elses.Remove(statement); }

        public void AddOrMoveElse(IIfStatement statement)
        { _elses.Add(statement); }

        public IEnumerable<IIfStatement> Elses
        { get { return _elses; } }


        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddOrMoveStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }
    }
}
