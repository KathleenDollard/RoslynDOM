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
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomDeclarationStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is LocalDeclarationStatementSyntax;
        }

        protected override IEnumerable<IStatementCommentWhite> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<IStatement>();
            var rawDeclaration = syntaxNode as LocalDeclarationStatementSyntax;
            var nodeWhitespaceList = new List<TokenWhitespace>(CreateFromWorker.GetWhitespaceSet(rawDeclaration, false).TokenWhitespaceList);
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
                CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

                //nodeWhitespaceList.InsertRange(1, newItem.TokenWhitespaceSet.TokenWhitespaceList);
                //nodeWhitespaceList.InsertRange(2, (itemAsT as IRoslynDom).TokenWhitespaceSet.TokenWhitespaceList);
                //newItem.TokenWhitespaceSet.TokenWhitespaceList.Clear();
                //newItem.TokenWhitespaceSet.TokenWhitespaceList.AddRange(nodeWhitespaceList);

                list.Add(newItem);
                // Don't need to copy here, because we are trashing item. 
                newItem.Name = itemAsT.Name;
                newItem.IsAliased = itemAsT.IsAliased;
                newItem.IsConst = rawDeclaration.IsConst;
                newItem.IsImplicitlyTyped = itemAsT.IsImplicitlyTyped;
                newItem.Initializer = itemAsT.Initializer;
                newItem.Type = itemAsT.Type;
                newItem.Whitespace2Set.AddRange(  itemAsT.Whitespace2Set);
            }
            list.AddRange(newItems.OfType<IStatement>());
            return list;
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
            nodeDeclarator = BuildSyntaxHelpers.AttachWhitespace(nodeDeclarator, itemAsT.Whitespace2Set, WhitespaceLookup);

            var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { (VariableDeclaratorSyntax)nodeDeclarator }));
            var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
            var node = SyntaxFactory.LocalDeclarationStatement(nodeDeclaration);

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item);
        }


    }
}
