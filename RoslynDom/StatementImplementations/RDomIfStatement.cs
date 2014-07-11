using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomIfStatement : RDomBaseBlock, IIfStatement
    {
        private IList<IIfStatement> _elses = new List<IIfStatement>();

        internal RDomIfStatement(
            IfStatementSyntax rawItem,
            bool hasBlock,
            IEnumerable<IStatement> statements,
            IEnumerable<IIfStatement> elses,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, statements, StatementKind.If, publicAnnotations)
        {
            HasBlock = hasBlock;
            if (elses != null)
            {
                foreach (var elseItem in elses)
                { AddOrMoveElse(elseItem); }
            }
            Initialize();
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
        }

        public override StatementSyntax BuildSyntax()
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

    }
}
