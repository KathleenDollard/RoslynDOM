using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomDeclarationStatementFactory
        : RDomStatementFactory<RDomDeclarationStatement, VariableDeclaratorSyntax>
    {
        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is LocalDeclarationStatementSyntax;
        }

        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<IStatement>();

            var rawDeclaration = syntaxNode as LocalDeclarationStatementSyntax;
            var rawVariableDeclaration = rawDeclaration.Declaration;
            var declarators = rawDeclaration.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = new RDomDeclarationStatement( decl, parent, model);
                list.Add(newItem);
                InitializeNewItem(newItem, decl, model);
            }
            return list;
        }
   
        public void InitializeNewItem(RDomDeclarationStatement newItem, VariableDeclaratorSyntax syntax, SemanticModel model)
        {
            newItem.Name = newItem.TypedSymbol.Name;
            var declaration = syntax.Parent as VariableDeclarationSyntax;
            if (declaration == null) throw new InvalidOperationException();
            newItem.IsImplicitlyTyped = (declaration.Type.ToString() == "var");
            var typeSymbol = ((ILocalSymbol)newItem.TypedSymbol).Type;
            newItem.Type = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, typeSymbol);
            if (syntax.Initializer != null)
            {
                var equalsClause = syntax.Initializer;
                newItem.Initializer = RDomFactoryHelper.GetHelper<IExpression>()
                                .MakeItem(equalsClause.Value, newItem, model).FirstOrDefault();
            }

        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IDeclarationStatement;
            var nameSyntax = SyntaxFactory.Identifier(item.Name);
            TypeSyntax typeSyntax;
            // TODO: Type alias are not currently being used. Could be brute forced here, but I'd rather run a simplifier for real world scenarios
            if (itemAsT.IsImplicitlyTyped)
            { typeSyntax = SyntaxFactory.IdentifierName("var"); }
            else
            { typeSyntax = (TypeSyntax)(RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Type)); }
            var nodeDeclarator = SyntaxFactory.VariableDeclarator(item.Name);
            if (itemAsT.Initializer != null)
            {
                var expressionSyntax = RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Initializer);
                nodeDeclarator = nodeDeclarator.WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax)expressionSyntax));
            }
            var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { (VariableDeclaratorSyntax)nodeDeclarator }));
            var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
            var node = SyntaxFactory.LocalDeclarationStatement(nodeDeclaration);
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }
}
