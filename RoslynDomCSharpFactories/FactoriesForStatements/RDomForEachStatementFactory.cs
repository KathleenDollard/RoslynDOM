using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomForEachStatementFactory
         : RDomStatementFactory<RDomForEachStatement, ForEachStatementSyntax>
    {
        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ForEachStatementSyntax;
            var newItem = new RDomForEachStatement(syntaxNode, parent, model);
            newItem.TestAtEnd = false; // restating the obvious
            newItem.Name = newItem.TypedSymbol.Name;

            var variable = new RDomVariableDeclaration(syntaxNode, parent, model); // ick, there is no syntax node to associate with the variable
            variable.IsImplicitlyTyped = (syntax.Type.ToString() == "var");
            var typeSymbol = ((ILocalSymbol)newItem.TypedSymbol).Type; // not sure this is valid at all
            variable.Type = new RDomReferencedType(typeSymbol.DeclaringSyntaxReferences, typeSymbol);
            // variable.Type = new RDomReferencedType(syntax.Type, typeSymbol);
            newItem.Variable = variable;
            return LoopFactoryHelper.CreateFrom<IForEachStatement>(newItem, syntax.Expression, syntax.Statement, parent, model);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IForEachStatement;

           TypeSyntax typeSyntax;
            if (itemAsT.Variable.IsImplicitlyTyped)
            { typeSyntax = SyntaxFactory.IdentifierName("var"); }
            else
            { typeSyntax = (TypeSyntax)(RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Variable.Type)); }

            var node = LoopFactoryHelper.BuildSyntax<IForEachStatement>
                (itemAsT, (c, s) => SyntaxFactory.ForEachStatement(typeSyntax, itemAsT.Name, c,s)).First() as ForEachStatementSyntax ;

            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }


    }
}
