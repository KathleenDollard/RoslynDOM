using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomExpressionFactory
                 : RDomExpressionFactory<RDomExpression, ExpressionSyntax>
    {
        public override void InitializeItem(RDomExpression newItem, ExpressionSyntax rawItem)
        {
            newItem.Expression = rawItem.ToString();
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IExpression item)
        {
            var itemAsT = item as IExpression;
            var node = SyntaxFactory.ParseExpression(itemAsT.Expression );
            return new SyntaxNode[] { node.NormalizeWhitespace() };
        }
    }
}
