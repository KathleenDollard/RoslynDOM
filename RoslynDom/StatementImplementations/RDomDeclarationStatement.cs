using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomDeclarationStatementFactory
        : RDomStatementFactory<RDomDeclarationStatement, LocalDeclarationStatementSyntax>
    { }


    public class RDomDeclarationStatement : RDomBase<IDeclarationStatement, LocalDeclarationStatementSyntax, ISymbol>, IDeclarationStatement
    {
        private VariableDeclarationSyntax _variableSyntax;
        private VariableDeclaratorSyntax _declaratorSyntax;
        internal RDomDeclarationStatement(LocalDeclarationStatementSyntax rawItem)
           : base(rawItem)
        {
            Initialize2();
        }

        internal RDomDeclarationStatement(
              LocalDeclarationStatementSyntax rawDeclaration,
              IEnumerable<PublicAnnotation> publicAnnotations)
            : base(rawDeclaration,  publicAnnotations)
        {
            Initialize();
            // TODO: Fix this
            //var Name = rawDeclarator.Identifier;
            //_variableSyntax = rawVariable;
            //_declaratorSyntax = rawDeclarator;
            //var Initilizer = rawVariable.Initializer;
            // var Type = rawDeclaration.
        }

        //internal RDomDeclarationStatement(
        //      LocalDeclarationStatementSyntax rawDeclaration,
        //      VariableDeclarationSyntax rawVariable,
        //      VariableDeclaratorSyntax rawDeclarator,
        //      params PublicAnnotation[] publicAnnotations)
        //    : base(rawDeclaration, StatementKind.Declaration, publicAnnotations)
        //{
        //    Initialize();
        //    var Name = rawDeclarator.Identifier;
        //    _variableSyntax = rawVariable;
        //    _declaratorSyntax = rawDeclarator;
        //    //var Initilizer = rawVariable.Initializer;
        //    // var Type = rawDeclaration.
        //}

        internal RDomDeclarationStatement(RDomDeclarationStatement oldRDom)
             : base(oldRDom)
        { }

        protected override void Initialize()
        {
            base.Initialize();

        }

        protected void Initialize2()
        {
            Initialize();
        }

        public override LocalDeclarationStatementSyntax BuildSyntax()
        {
            return null;
        }

        public IExpression Initializer { get; set; }

        public IReferencedType Type { get; set; }

        public bool IsImplicitlyTyped { get; set; }
        public bool IsConst { get; set; }


    }
}
