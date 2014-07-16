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
    public class RDomExpression : RDomBase<IExpression, ISymbol>, IExpression
    {
        internal RDomExpression(ExpressionSyntax rawItem)
           : base(rawItem)
        {
            //Initialize();
        }

        internal RDomExpression(RDomExpression oldRDom)
            : base(oldRDom)
        {
            Expression = oldRDom.Expression;
        }

        //protected override void Initialize()
        //{
        //    // don't call base initialize, name has no meaning
        //    Expression = RawItem.ToString();
        //}

        public string Expression { get; set; }
    }
}
