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
        public RDomBlockStatementFactory(RDomCorporation corporation)
              : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as BlockSyntax;
            var newItem = new RDomBlockStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statements = Corporation.CreateFrom<IStatementCommentWhite>(statementSyntax, newItem, model);
                newItem.Statements.AddOrMoveRange(statements);
            }

            return newItem;
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IBlockStatement;
            var block = BuildSyntaxWorker.GetStatementBlock(itemAsT.Statements);
            //var statementSyntaxList = itemAsT.Statements
            //  .SelectMany(x => Corporation.BuildSyntaxGroup(x))
            //  .ToList();
            var node = SyntaxFactory.Block(SyntaxFactory.List(block.Statements));

            return item.PrepareForBuildSyntaxOutput(node);

        }
    }

}
