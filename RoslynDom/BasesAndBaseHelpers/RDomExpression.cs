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
        public override IEnumerable<SyntaxNode> BuildSyntax(IExpression item)
        {
            var itemAsT = item as IExpression;
            var node = SyntaxFactory.ParseExpression(itemAsT.Expression );
            return new SyntaxNode[] { node.NormalizeWhitespace() };
        }
    }

    public class RDomExpression : RDomBase<IExpression, ExpressionSyntax, ISymbol>, IExpression
    {
        internal RDomExpression(ExpressionSyntax rawItem)
           : base(rawItem)
        {
            Expression = rawItem.ToString();
        }

        internal RDomExpression(RDomExpression oldRDom)
            : base(oldRDom)
        {
            Expression = oldRDom.Expression;
        }

        public string Expression { get; set; }
    }
}
