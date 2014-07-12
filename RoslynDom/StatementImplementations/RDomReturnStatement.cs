using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomReturnStatementFactory
                : RDomStatementFactory<RDomReturnStatement, ReturnStatementSyntax>
    {
        public RDomReturnStatementFactory( RDomFactoryHelper  helper)
            : base( helper)
        { }
    }

    public class RDomReturnStatement : RDomStatement, IReturnStatement
    {
 
        internal RDomReturnStatement(
              ReturnStatementSyntax  rawReturn,
              IEnumerable<PublicAnnotation> publicAnnotations)
            : base(rawReturn, StatementKind.Return, publicAnnotations)
        {
            Initialize();
            // Return = expression
        }

        internal RDomReturnStatement(RDomReturnStatement oldRDom)
             : base(oldRDom)
        { }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public override StatementSyntax BuildSyntax()
        {
            return null;
        }

        public IExpression Return { get; set; }
    }
}
