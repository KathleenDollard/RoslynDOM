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
            return syntaxNode is VariableDeclarationSyntax
                || syntaxNode is CatchDeclarationSyntax
                || syntaxNode is ForEachStatementSyntax;
        }

        protected override IEnumerable<IMisc> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var rawVariableDeclaration = syntaxNode as VariableDeclarationSyntax;
            if (rawVariableDeclaration != null)
            { return CreateFromVariableDeclaration(rawVariableDeclaration, syntaxNode, parent, model); }

            var rawCatchDeclaration = syntaxNode as CatchDeclarationSyntax;
            if (rawCatchDeclaration != null)
            {
                return new IMisc[] { GetNewVariable(
                  rawCatchDeclaration.Type, rawCatchDeclaration, parent, model) };
            }

            var rawForEachSyntax = syntaxNode as ForEachStatementSyntax;
            if (rawForEachSyntax != null)
            {
                return new IMisc[] { GetNewVariable(
                  rawForEachSyntax.Type, rawForEachSyntax, parent, model) };
            }

            throw new InvalidOperationException();
        }

        private IEnumerable<IMisc> CreateFromVariableDeclaration(VariableDeclarationSyntax syntax, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<IMisc>();
            var declarators = syntax.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = GetNewVariable(syntax.Type, decl, parent, model);
                list.Add(newItem);
                if (decl.Initializer != null)
                {
                    var equalsClause = decl.Initializer;
                    newItem.Initializer = Corporation.CreateFrom<IExpression>(equalsClause.Value, newItem, model).FirstOrDefault();
                }
            }
            return list;
        }

        public IVariable GetNewVariable(TypeSyntax typeSyntax,
            SyntaxNode node, IDom parent, SemanticModel model)
        {
            var newItem = new RDomVariableDeclaration(node, parent, model);
            CreateFromWorker.StandardInitialize(newItem, node, parent, model);
            newItem.Name = newItem.TypedSymbol.Name;
            var declaredType = typeSyntax.ToString();
            var returnType = Corporation
                            .CreateFrom<IMisc>(typeSyntax, newItem, model)
                            .FirstOrDefault()
                            as IReferencedType;
            newItem.Type = returnType;

            newItem.IsImplicitlyTyped = (declaredType == "var");
            if (!newItem.IsImplicitlyTyped && declaredType != newItem.Type.Name)
            {
                var test = Mappings.AliasFromSystemType(newItem.Type.Name);
                if (declaredType == test) newItem.IsAliased = true;
            }
            return newItem;
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

            return node.PrepareForBuildSyntaxOutput(item);
        }
    }
}
