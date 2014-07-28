using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomForEachStatementFactory
         : RDomStatementFactory<RDomForEachStatement, ForEachStatementSyntax>
    {
        public RDomForEachStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ForEachStatementSyntax;
            var newItem = new RDomForEachStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            newItem.TestAtEnd = false; // restating the obvious

            var variable = new RDomVariableDeclaration(syntaxNode, parent, model); // ick, there is no syntax node to associate with the variable
            variable.IsImplicitlyTyped = (syntax.Type.ToString() == "var");
            var typeSymbol = ((ILocalSymbol)newItem.TypedSymbol).Type; // not sure this is valid at all
            variable.Type = new RDomReferencedType(typeSymbol.DeclaringSyntaxReferences, typeSymbol);
            variable.Name =syntax.Identifier.ToString();
            newItem.Variable = variable;
            return LoopFactoryHelper.CreateItemFrom<IForEachStatement>(newItem, syntax.Expression, syntax.Statement, parent, model, Corporation, CreateFromWorker);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IForEachStatement;

           TypeSyntax typeSyntax;
            if (itemAsT.Variable.IsImplicitlyTyped)
            { typeSyntax = SyntaxFactory.IdentifierName("var"); }
            else
            { typeSyntax = (TypeSyntax)(RDomCSharp.Factory.BuildSyntax(itemAsT.Variable.Type)); }

            return LoopFactoryHelper.BuildSyntax<IForEachStatement>
                (itemAsT, (c, s) => SyntaxFactory.ForEachStatement(typeSyntax, itemAsT.Variable.Name, c,s)) ;

        }


    }
}
