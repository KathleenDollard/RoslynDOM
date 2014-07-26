using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomBlockStatementFactory
             : RDomStatementFactory<RDomBlockStatement, BlockSyntax>
    {
        protected  override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as BlockSyntax;
            var newItem = new RDomBlockStatement(syntaxNode, parent, model);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statements = RDomFactoryHelper.GetHelperForStatement().MakeItems(statementSyntax, newItem, model);
                newItem.Statements.AddOrMoveRange(statements);
            }

            return newItem ;
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStatementCommentWhite item)
        {
            var itemAsT = item as IBlockStatement;

            var statementSyntaxList = itemAsT.Statements
              .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
              .ToList();
            var node = SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList));

            return item.PrepareForBuildSyntaxOutput(node);

        }
    }

}
