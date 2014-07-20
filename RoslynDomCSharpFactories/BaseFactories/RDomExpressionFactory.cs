using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomExpressionFactory
                 : RDomExpressionFactory<RDomExpression, ExpressionSyntax>
    {
        public override IEnumerable<IExpression> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ExpressionSyntax;
           
            var newItem = new RDomExpression( syntaxNode, parent, model);
            newItem.Expression = syntax.ToString();
            newItem.ExpressionType = ExpressionTypeFromSyntax(syntaxNode);

            return new IExpression[] { newItem };

        }

        private ExpressionType ExpressionTypeFromSyntax(SyntaxNode syntaxNode)
        {
            if (syntaxNode is LiteralExpressionSyntax) { return ExpressionType.Literal; }
            if (syntaxNode is ObjectCreationExpressionSyntax ) { return ExpressionType.ObjectCreation ; }
            if (syntaxNode is InvocationExpressionSyntax ) { return ExpressionType.Invocation ; }
            if (syntaxNode is IdentifierNameSyntax) { return ExpressionType.Identifier; }
            if (syntaxNode is BinaryExpressionSyntax ) { return ExpressionType.Complex; }
            return ExpressionType.Unknown; 
        }


        public override IEnumerable<SyntaxNode> BuildSyntax(IExpression item)
        {
            var itemAsT = item as IExpression;
            var node = SyntaxFactory.ParseExpression(itemAsT.Expression);
            return new SyntaxNode[] { node.NormalizeWhitespace() };
        }
    }
}
