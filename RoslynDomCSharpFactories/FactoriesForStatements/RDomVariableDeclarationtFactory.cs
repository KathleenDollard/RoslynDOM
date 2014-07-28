using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomVariableDeclarationFactory
        : RDomMiscFactory<RDomVariableDeclaration, VariableDeclaratorSyntax>
    {
        public RDomVariableDeclarationFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is VariableDeclarationSyntax;
        }

        protected  override IEnumerable<IMisc> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<IMisc>();

            var rawVariableDeclaration = syntaxNode as VariableDeclarationSyntax;
            var declarators = rawVariableDeclaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = new RDomVariableDeclaration( decl, parent, model);
                CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
                list.Add(newItem);
                InitializeNewItem(newItem, decl, model);
            }
            return list;
        }
   
        public void InitializeNewItem(RDomVariableDeclaration newItem, VariableDeclaratorSyntax syntax, SemanticModel model)
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
                newItem.Initializer = Corporation.CreateFrom<IExpression>(equalsClause.Value, newItem, model).FirstOrDefault();
            }

        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IVariableDeclaration;
            var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);
            TypeSyntax typeSyntax;
            // TODO: Type alias are not currently being used. Could be brute forced here, but I'd rather run a simplifier for real world scenarios
            if (itemAsT.IsImplicitlyTyped)
            { typeSyntax = SyntaxFactory.IdentifierName("var"); }
            else
            { typeSyntax = (TypeSyntax)(RDomCSharp.Factory.BuildSyntax(itemAsT.Type)); }
            var nodeDeclarator = SyntaxFactory.VariableDeclarator(itemAsT.Name);
            if (itemAsT.Initializer != null)
            {
                var expressionSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Initializer);
                nodeDeclarator = nodeDeclarator.WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax)expressionSyntax));
            }
            var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { (VariableDeclaratorSyntax)nodeDeclarator }));
            var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
            var node = SyntaxFactory.LocalDeclarationStatement(nodeDeclaration);

            return item.PrepareForBuildSyntaxOutput(node);
        }
    }
}
