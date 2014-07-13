﻿using System;
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
    { }

    public class RDomReturnStatement : RDomBase<IReturnStatement, ReturnStatementSyntax, ISymbol>,  IReturnStatement
    {

        internal RDomReturnStatement(ReturnStatementSyntax rawItem)
           : base(rawItem)
        {
            Initialize2();
        }

        internal RDomReturnStatement(
              ReturnStatementSyntax  rawReturn,
              IEnumerable<PublicAnnotation> publicAnnotations)
            : base(rawReturn,  publicAnnotations)
        {
            Initialize();
        }

        internal RDomReturnStatement(RDomReturnStatement oldRDom)
             : base(oldRDom)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            Return = new RDomExpression(TypedSyntax.Expression);
        }

        protected void Initialize2()
        {
            Initialize();
        }

        public override ReturnStatementSyntax BuildSyntax()
        {
            var expr = ((RDomExpression)Return).BuildSyntax();
            var node = SyntaxFactory.ReturnStatement(expr);
            return node;
        }

        public IExpression Return { get; set; }
    }
}
