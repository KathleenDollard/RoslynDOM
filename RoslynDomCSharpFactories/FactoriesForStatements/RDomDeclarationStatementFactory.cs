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
        public RDomDeclarationStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is LocalDeclarationStatementSyntax;
        }

        protected override IEnumerable<IStatementCommentWhite> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<IStatement>();
            var rawDeclaration = syntaxNode as LocalDeclarationStatementSyntax;
            // This returns VariableDeclarations that are then converted into Declaration statements. 
            // That is ugly, but I felt it more important to reuse the variable extraction code. 
            var newItems = Corporation.CreateFrom<IMisc>(rawDeclaration.Declaration, parent, model);
            foreach (var item in newItems)
            {
                if (item is ICommentWhite) { continue; }
                var syntax = (SyntaxNode)item.RawItem;
                var itemAsT = item as IVariableDeclaration;
                var newItem = new RDomDeclarationStatement(syntax, parent, model);
                CreateFromWorker.StandardInitialize(newItem, syntax, parent, model);
                list.Add(newItem);
                // Don't need to copy here, because we are trashing item. 
                newItem.Name = itemAsT.Name;
                newItem.IsAliased = itemAsT.IsAliased;
                newItem.IsConst = rawDeclaration.IsConst;
                newItem.IsImplicitlyTyped = itemAsT.IsImplicitlyTyped;
                newItem.Initializer = itemAsT.Initializer;
                newItem.Type = itemAsT.Type;
            }
            list.AddRange(newItems.OfType<IStatement>());
            //var rawVariableDeclaration = rawDeclaration.Declaration;
            //var declarators = rawDeclaration.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            //foreach (var decl in declarators)
            //{
            //    var newItem = new RDomDeclarationStatement(decl, parent, model);
            //    list.Add(newItem);
            //    CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            //    InitializeNewItem(newItem, decl, model);
            //}
            return list;
        }

        public void InitializeNewItem(RDomDeclarationStatement newItem, VariableDeclaratorSyntax syntax, SemanticModel model)
        {
            newItem.Name = newItem.TypedSymbol.Name;

            //var declaration = syntax.Parent as VariableDeclarationSyntax;
            //if (declaration == null) throw new InvalidOperationException();

            //CreateFromWorker.InitializeVariable(newItem, declaration.Type.ToString(), syntax, model);

            //if (syntax.Initializer != null)
            //{
            //    var equalsClause = syntax.Initializer;
            //    newItem.Initializer = Corporation.CreateFrom<IExpression>(equalsClause.Value, newItem, model).FirstOrDefault();
            //}

        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IDeclarationStatement;
            var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);
            TypeSyntax typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(itemAsT);

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
