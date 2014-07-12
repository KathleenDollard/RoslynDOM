using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAssignmentStatementFactory
         : RDomStatementFactory<RDomAssignmentStatement, ExpressionStatementSyntax>
    {
        public RDomAssignmentStatementFactory(RDomFactoryHelper helper)
            : base(helper)
        { }
    }

    public class RDomAssignmentStatement : RDomBase<IAssignmentStatement, ExpressionStatementSyntax, ISymbol>, IAssignmentStatement
    {
        private VariableDeclarationSyntax _variableSyntax;
        private VariableDeclaratorSyntax _declaratorSyntax;

 
        internal RDomAssignmentStatement(
              ExpressionStatementSyntax rawExpression,
              IEnumerable<PublicAnnotation> publicAnnotations)
            : base(rawExpression,  publicAnnotations)
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

        public override ExpressionStatementSyntax BuildSyntax()
        {
            return null;
        }

        public string VarName { get; set; }
        public IExpression Expression { get; set; }
    }
}
