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
        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as BlockSyntax;
            var newItem = new RDomBlockStatement(syntaxNode, parent, model);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statements = RDomFactoryHelper.GetHelper<IStatement>().MakeItem(statementSyntax, newItem, model);
                foreach (var statement in statements)
                { newItem.AddOrMoveStatement(statement); }
            }

            return new IStatement[] { newItem };
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IBlockStatement;

            var statementSyntaxList = itemAsT.Statements
              .SelectMany(x => RDomCSharpFactory.Factory.BuildSyntaxGroup(x))
              .ToList();
            var node = SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList));
           // TODO: return new SyntaxNode[] { node.Format() };
            return new SyntaxNode[] { node };

        }
    }

}
