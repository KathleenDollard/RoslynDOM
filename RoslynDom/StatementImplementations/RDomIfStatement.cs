using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomIfStatement : RDomBase<IIfStatement, ISymbol>, IIfStatement
    {
        private IList<IIfStatement> _elses = new List<IIfStatement>();
        private IList<IStatement> _statements = new List<IStatement>();
        private IList<IStatement> _elseStatements = new List<IStatement>();

        public RDomIfStatement(SyntaxNode rawItem)
            : base(rawItem)
        {
  //          Initialize2();
        }

        //internal RDomIfStatement(
        //     IfStatementSyntax rawItem,
        //       IEnumerable<PublicAnnotation> publicAnnotations)
        //    : base(rawItem, publicAnnotations)
        //{
        //    Initialize();
        //}

        internal RDomIfStatement(RDomIfStatement oldRDom)
            : base(oldRDom)
        {
            var newElses = RoslynDomUtilities.CopyMembers(oldRDom.ElseIfs);
            foreach (var elseItem in newElses)
            { AddOrMoveElseIf(elseItem); }
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            foreach (var statement in statements)
            { AddOrMoveStatement(statement); }
            statements = RoslynDomUtilities.CopyMembers(oldRDom.ElseStatements);
            foreach (var statement in statements)
            { AddOrMoveElseStatement(statement); }
            Condition = oldRDom.Condition.Copy();
            HasBlock = oldRDom.HasBlock;
            ElseHasBlock = oldRDom.ElseHasBlock;
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //    Condition = RDomFactoryHelper.ExpressionFactoryHelper.MakeItem(TypedSyntax.Condition).FirstOrDefault();
        //    if (Condition == null) { throw new InvalidOperationException(); }

        //    var statements = GetStatementsFromSyntax(TypedSyntax.Statement);
        //    foreach (var statement in statements)
        //    { AddOrMoveStatement(statement); }

        //    InitializeElse();
        //}

        //private IEnumerable<IStatement> GetStatementsFromSyntax(StatementSyntax statementSyntax)
        //{
        //    var statement = RDomFactoryHelper.StatementFactoryHelper.MakeItem(statementSyntax).First();
        //    var list = new List<IStatement>();
        //    var blockStatement = statement as IBlockStatement;
        //    if (blockStatement != null)
        //    {
        //        HasBlock = true;
        //        foreach (var state in blockStatement.Statements)
        //        {
        //            // Don't need to copy because abandoning block
        //            list.Add(state);
        //        }
        //    }
        //    else
        //    { list.Add(statement); }
        //    return list;
        //}

        //private void InitializeElse()
        //{
        //    if (TypedSyntax.Else == null) return;
        //    var elseAsIf = TypedSyntax.Else.Statement as IfStatementSyntax;
        //    if (elseAsIf == null)
        //    {
        //        InitializeElseStatement(TypedSyntax.Else.Statement);
        //    }
        //    else
        //    {

        //        // Recurse this down the if chain
        //        var newIf = new RDomIfStatement(elseAsIf);
        //        var elseIfs = newIf.ElseIfs;
        //        var elseStatements = newIf.ElseStatements;
        //        //foreach (var item in newIf.ElseIfs) { newIf.RemoveElseIf(item); }
        //        //foreach (var item in newIf.ElseStatements) { newIf.RemoveElseStatement(item); }
        //        AddOrMoveElseIf(newIf);
        //        if (!newIf.ElseIfs.Any())
        //        {
        //            // this should move them
        //            foreach (var elseif in elseIfs)
        //            { AddOrMoveElseIf(elseif); }  // Don't need to copy as we are trashing original
        //            if (elseStatements.Any())
        //            {
        //                ElseHasBlock = newIf.ElseHasBlock;
        //                foreach (var statement in elseStatements)
        //                { AddOrMoveElseStatement(statement); }
        //            }
        //        }
        //    }
        //}

        //private void InitializeElseStatement(StatementSyntax statement)
        //{
        //    ElseHasBlock = statement is BlockSyntax;
        //    var statements = GetStatementsFromSyntax(TypedSyntax.Else.Statement);
        //    foreach (var state in statements)
        //    { AddOrMoveElseStatement(state); }
        //}

        //protected void Initialize2()
        //{
        //    Initialize();
        //}

        //public override IfStatementSyntax BuildSyntax()
        //{
        //    // TODO: Current work KAD
        //    //var nameSyntax = SyntaxFactory.Identifier(Name);
        //    //var statementsAsSyntax = Statements.Select(x => ((RDomStatement)x).BuildSyntax()).ToArray();
        //    //StatementSyntax statement;
        //    //if (HasBlock || this.Statements.Count() > 1)
        //    //{ statement = SyntaxFactory.Block(statementsAsSyntax); }
        //    //else if (this.Statements.Count() == 1)
        //    //{ statement = statementsAsSyntax.First(); }
        //    //else
        //    //{ statement = SyntaxFactory.EmptyStatement(); }
        //    //var condition = (Condition as RDomCondition).BuildSyntax();
        //    //var node = SyntaxFactory.IfStatement(condition, statement);

        //    //node = RoslynUtilities.UpdateNodeIfItemNotNull(BuildElseSyntax(), node, (n, item) => n.WithElse(item));

        //    //return (StatementSyntax)RoslynUtilities.Format(node);
        //    return null;
        //}

        public IExpression Condition { get; set; }

        public bool HasBlock { get; set; }
        public bool ElseHasBlock { get; set; }

        public void RemoveElseIf(IIfStatement statement)
        { _elses.Remove(statement); }

        public void AddOrMoveElseIf(IIfStatement statement)
        { _elses.Add(statement); }

        public IEnumerable<IIfStatement> ElseIfs
        { get { return _elses; } }


        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddOrMoveStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }

        public void RemoveElseStatement(IStatement statement)
        { _elseStatements.Remove(statement); }

        public void AddOrMoveElseStatement(IStatement statement)
        { _elseStatements.Add(statement); }

        public IEnumerable<IStatement> ElseStatements
        { get { return _elseStatements; } }
    }
}
