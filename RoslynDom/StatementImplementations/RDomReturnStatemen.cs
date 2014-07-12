using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomReturnStatementFactory : IRDomFactory<IStatement>
    {
        private PublicAnnotationFactory _publicAnnotationFactory;

        public RDomReturnStatementFactory(PublicAnnotationFactory publicAnnotationFactory)
        {
            _publicAnnotationFactory = publicAnnotationFactory;
        }

        public FactoryPriority Priority
        { get { return FactoryPriority.Normal; } }

        public bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return (syntaxNode is LocalDeclarationStatementSyntax);
        }

        public IStatement CreateFrom(SyntaxNode syntaxNode)
        {
            var publicAnnotations = _publicAnnotationFactory.CreateFrom(syntaxNode);
            return new RDomDeclarationStatement((LocalDeclarationStatementSyntax)syntaxNode, publicAnnotations);
        }
    }

    public class RDomReturnStatement : RDomStatement, IReturnStatement
    {
 
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
