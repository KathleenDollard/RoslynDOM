using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharpFactories
{
    public class RDomDeclarationStatementFactory
        : RDomStatementFactory<IDeclarationStatement, VariableDeclaratorSyntax>
    {
        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is LocalDeclarationStatementSyntax;
        }

        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode)
        {
            var list = new List<IStatement>();
            // We can't do this in the constructor, because many may be created and we want to flatten
            var rawDeclaration = syntaxNode as LocalDeclarationStatementSyntax;
            var rawVariableDeclaration = rawDeclaration.Declaration;
            var declarators = rawDeclaration.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = new RDomDeclarationStatement( decl);
                list.Add(newItem);
                InitializeItem(newItem, decl);
            }
            return list;
        }

        public override void InitializeItem(IDeclarationStatement newItem, VariableDeclaratorSyntax syntax)
        {
            newItem.Name = newItem.Symbol.Name;
            var declaration = syntax.Parent as VariableDeclarationSyntax;
            if (declaration == null) throw new InvalidOperationException();
            newItem.IsImplicitlyTyped = (declaration.Type.ToString() == "var");
            var typeSymbol = ((ILocalSymbol)newItem.TypedSymbol).Type;
            newItem.Type = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, typeSymbol);
            if (syntax.Initializer != null)
            {
                var equalsClause = syntax.Initializer;
                newItem.Initializer = RDomFactoryHelper.ExpressionFactoryHelper.MakeItem(equalsClause.Value).FirstOrDefault();
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
            { typeSyntax = (TypeSyntax)(RDomFactory.BuildSyntax(itemAsT.Type)); }
            var nodeDeclarator = SyntaxFactory.VariableDeclarator(item.Name);
            if (itemAsT.Initializer != null)
            {
                var expressionSyntax = RDomFactory.BuildSyntax(itemAsT.Initializer);
                nodeDeclarator = nodeDeclarator.WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax)expressionSyntax));
            }
            var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { (VariableDeclaratorSyntax)nodeDeclarator }));
            var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
            var node = SyntaxFactory.LocalDeclarationStatement(nodeDeclaration);
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }


}
