using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomReturnStatement : RDomStatement, IReturnStatement
    {
        private VariableDeclarationSyntax _variableSyntax;
        private VariableDeclaratorSyntax _declaratorSyntax;

  
        internal RDomReturnStatement(
              ReturnStatementSyntax  rawReturn,
              params PublicAnnotation[] publicAnnotations)
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
