using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomBlockStatementFactory
         : RDomStatementFactory<RDomBlockStatement, BlockSyntax>
    {
        public override void InitializeItem(RDomBlockStatement newItem, BlockSyntax syntax)
        {
            foreach (var statementSyntax in syntax.Statements)
            {
                var statements = RDomFactoryHelper.GetHelper<IStatement>().MakeItem(statementSyntax);
                foreach (var statement in statements)
                { newItem.AddOrMoveStatement(statement); }
            }
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IBlockStatement;

            var statementSyntaxList = itemAsT.Statements
              .SelectMany(x => RDomFactory.BuildSyntaxGroup(x))
              .ToList();
            var node = SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList));
            return new SyntaxNode[] { node.NormalizeWhitespace() };

        }
    }


    public class RDomBlockStatement : RDomBase<IBlockStatement, BlockSyntax, ISymbol>, IBlockStatement
    {
        private IList<IStatement> _statements = new List<IStatement>();

        internal RDomBlockStatement(BlockSyntax rawItem)
           : base(rawItem)
        {
            //Initialize2();
        }

        //internal RDomBlockStatement(
        //    BlockSyntax rawItem,
        //      IEnumerable<PublicAnnotation> publicAnnotations)
        //  : base(rawItem, publicAnnotations)
        //{
        //    Initialize();
        //}


        internal RDomBlockStatement(RDomBlockStatement oldRDom)
             : base(oldRDom)
        {
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            foreach (var statement in statements)
            { AddOrMoveStatement(statement); }
        }


        //protected override void Initialize()
        //{
        //    foreach (var statementSyntax in TypedSyntax.Statements)
        //    {
        //        var statements = RDomFactoryHelper.StatementFactoryHelper.MakeItem(statementSyntax);
        //        foreach (var statement in statements)
        //        { AddOrMoveStatement(statement); }
        //    }
        //}

        //protected void Initialize2()
        //{
        //    Initialize();
        //}

        //public override BlockSyntax BuildSyntax()
        //{
        //    return null;
        //}


        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddOrMoveStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }
    }
}
