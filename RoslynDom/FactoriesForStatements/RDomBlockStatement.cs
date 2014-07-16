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

}
