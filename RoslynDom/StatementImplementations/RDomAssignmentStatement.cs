using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAssignmentStatement : RDomStatement, IAssignmentStatement
    {
        private VariableDeclarationSyntax _variableSyntax;
        private VariableDeclaratorSyntax _declaratorSyntax;

 
        internal RDomAssignmentStatement(
              ExpressionStatementSyntax rawExpression,
              params PublicAnnotation[] publicAnnotations)
            : base(rawExpression, StatementKind.Assignment, publicAnnotations)
        {
            Initialize();
        }

        internal RDomAssignmentStatement(RDomAssignmentStatement oldRDom)
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

        public string VarName { get; set; }
        public IExpression Expression { get; set; }
    }
}
