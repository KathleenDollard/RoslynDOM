using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    //public abstract class RDomBaseBlock : RDomBase<IStatement, ReturnStatementSyntax, ISymbol>
    //{
    //    private IList<IStatement> _statements = new List<IStatement>();

    //    internal RDomBaseBlock(
    //          StatementSyntax rawItem,
    //          StatementKind statementKind,
    //          IEnumerable<PublicAnnotation> publicAnnotations)
    //        : base(rawItem, statementKind, publicAnnotations)
    //    {
           
    //    }
    //    internal RDomBaseBlock(
    //        StatementSyntax rawItem,
    //        IEnumerable<IStatement> statements,
    //        StatementKind statementKind,
    //          IEnumerable<PublicAnnotation> publicAnnotations)
    //      : base(rawItem, statementKind, publicAnnotations)
    //    {
    //        foreach (var statement in statements)
    //        { AddOrMoveStatement(statement); }
    //    }

    //    internal RDomBaseBlock(RDomBaseBlock oldRDom)
    //         : base(oldRDom)
    //    {
    //        var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
    //        foreach (var statement in newStatements)
    //        { AddOrMoveStatement(statement); }
    //    }

    //    public override StatementSyntax BuildSyntax()
    //    {
    //        return null;
    //    }

    //    public void RemoveStatement(IStatement statement)
    //    { _statements.Remove(statement); }

    //    public void AddOrMoveStatement(IStatement statement)
    //    { _statements.Add(statement); }

    //    public IEnumerable<IStatement> Statements
    //    { get { return _statements; } }

    //}
}
