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
            { return CreateFromCatchDeclaration(rawCatchDeclaration, parent, model); }

            var rawForEachSyntax = syntaxNode as ForEachStatementSyntax;
            if (rawForEachSyntax != null)
            { return CreateFromForEach(rawForEachSyntax, parent, model); }


            throw new InvalidOperationException();
        }

        private IEnumerable<IMisc> CreateFromCatchDeclaration(CatchDeclarationSyntax syntax, IDom parent, SemanticModel model)
        {
            var newItems = GetWithIdAndType(syntax.Identifier, syntax.Type, syntax, parent, model);
            return newItems;
        }

        private IEnumerable<IMisc> CreateFromForEach(ForEachStatementSyntax syntax, IDom parent, SemanticModel model)
        {
            var newItems = GetWithIdAndType(syntax.Identifier, syntax.Type, syntax, parent, model);
            return newItems;
        }

        private IEnumerable<IMisc> GetWithIdAndType(SyntaxToken identifier, TypeSyntax type, SyntaxNode node, IDom parent, SemanticModel model)
        {
            var list = new List<IMisc>();
            var newItem = new RDomVariableDeclaration(node, parent, model);
            CreateFromWorker.StandardInitialize(newItem, node, parent, model);
            list.Add(newItem);
            newItem.Name = identifier.ToString();
            ISymbol typeSymbol = null;
            if (newItem.TypedSymbol != null)
            { typeSymbol = ((ILocalSymbol)newItem.TypedSymbol).Type; }
            if (typeSymbol == null)
            { typeSymbol = model.GetDeclaredSymbol(node); }
            if (typeSymbol == null)
            { typeSymbol = model.GetTypeInfo(type).Type; }
            newItem.Type = new RDomReferencedType(typeSymbol.DeclaringSyntaxReferences, typeSymbol);
            InitializeVariable(newItem, type.ToString(),typeSymbol, node, model);
            return list;
        }

        private IEnumerable<IMisc> CreateFromVariableDeclaration(VariableDeclarationSyntax syntax, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<IMisc>();
            var declarators = syntax.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = new RDomVariableDeclaration(decl, parent, model);
                CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
                list.Add(newItem);
                newItem.Name = newItem.TypedSymbol.Name;
                var typeSymbol = ((ILocalSymbol)newItem.TypedSymbol).Type;
                InitializeVariable(newItem, syntax.Type.ToString(), typeSymbol, decl, model);
                //newItem.IsImplicitlyTyped = (syntax.Type.ToString() == "var");
                //var typeSymbol = ((ILocalSymbol)newItem.TypedSymbol).Type;
                //newItem.Type = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, typeSymbol);
                if (decl.Initializer != null)
                {
                    var equalsClause = decl.Initializer;
                    newItem.Initializer = Corporation.CreateFrom<IExpression>(equalsClause.Value, newItem, model).FirstOrDefault();
                }
            }
            return list;
        }

        public void InitializeVariable(IVariable newItem, string declaredType, ISymbol typeSymbol, SyntaxNode node, SemanticModel model)
        {

            //ISymbol symbol = null;
            //ISymbol typeSymbol = null;
            //var itemAsIDom = newItem as IRoslynHasSymbol;
            //if (itemAsIDom != null)
            //{ symbol = itemAsIDom.Symbol; }
            //if (symbol != null)
            //{ typeSymbol = ((ILocalSymbol)symbol).Type; }
            //if (typeSymbol == null)
            //{ typeSymbol = model.GetDeclaredSymbol(node); }
            //if (typeSymbol == null)
            //{
            //    var syntax = node as VariableDeclarationSyntax;
            //    if (syntax == null) syntax = node.Parent as VariableDeclarationSyntax;
            //    if (syntax != null)
            //    { typeSymbol = model.GetTypeInfo(syntax.Type).Type; }
            //}

            newItem.Type = new RDomReferencedType(typeSymbol.DeclaringSyntaxReferences, typeSymbol);
            newItem.IsImplicitlyTyped = (declaredType == "var");
            if (!newItem.IsImplicitlyTyped && declaredType != newItem.Type.Name)
            {
                var test = Mappings.AliasFromSystemType(newItem.Type.Name);
                if (declaredType == test) newItem.IsAliased = true;
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
