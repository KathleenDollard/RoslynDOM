using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomPropertyAccessorMiscFactory
          : RDomMiscFactory<RDomPropertyAccessor, AccessorDeclarationSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomPropertyAccessorMiscFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
            if (_whitespaceLookup == null)
            {
                _whitespaceLookup = new WhitespaceKindLookup();
                _whitespaceLookup.Add(LanguageElement.AccessorStartDelimiter, SyntaxKind.OpenBraceToken);
                _whitespaceLookup.Add(LanguageElement.AccessorGetKeyword, SyntaxKind.GetKeyword);
                _whitespaceLookup.Add(LanguageElement.AccessorSetKeyword, SyntaxKind.SetKeyword);
                _whitespaceLookup.Add(LanguageElement.AccessorShortFormIndicator, SyntaxKind.SemicolonToken);
                _whitespaceLookup.Add(LanguageElement.AccessorEndDelimiter, SyntaxKind.CloseBraceToken);
                //Tuple.Create( LanguageElement.AccessorAddEventKeyword, SyntaxKind.??),
                // TODO: Find Add/Remove keywords
                //Tuple.Create( LanguageElement.AccessorRemoveEventKeyword, SyntaxKind.??),
                _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                }
                return _whitespaceLookup;
            }
        }

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as AccessorDeclarationSyntax;
            var parentProperty = parent as IProperty;
            var accessorType = (syntaxNode.CSharpKind() == SyntaxKind.GetAccessorDeclaration)
                                ? AccessorType.Get : AccessorType.Set;
            var newItem = new RDomPropertyAccessor(syntaxNode, accessorType, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);

            CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, syntax.Body, LanguagePart.Block, WhitespaceLookup);

            var newItemName = accessorType.ToString().ToLower() + "_" + parentProperty.Name;
            newItem.Name = newItemName;

            return newItem;

        }

          public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsAccessor = item as IAccessor;
            var parentProperty = item.Parent as IProperty;
            if (itemAsAccessor == null || parentProperty == null) { throw new InvalidOperationException(); }
            var kind = (itemAsAccessor.AccessorType == AccessorType.Get)
                        ? SyntaxKind.GetAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;
            AccessorDeclarationSyntax node;
            if (itemAsAccessor.Statements.Any())
            {
                //var statementBlock = BuildSyntaxWorker.GetStatementBlock(itemAsAccessor.Statements);
                var statementBlock = (BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsAccessor.Statements, itemAsAccessor, WhitespaceLookup);
                //statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, itemAsAccessor.Whitespace2Set, WhitespaceLookup);
                node = SyntaxFactory.AccessorDeclaration(kind, statementBlock);
            }
            else
            {
                node = SyntaxFactory.AccessorDeclaration(kind).WithSemicolonToken(
                            SyntaxFactory.Token(
                                SyntaxKind.SemicolonToken));
            }


            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsAccessor.Whitespace2Set, WhitespaceLookup);

            var attributeList = BuildSyntaxWorker.BuildAttributeSyntax(itemAsAccessor.Attributes);
            if (attributeList.Any()) { node = node.WithAttributeLists(attributeList); }

            return node.PrepareForBuildSyntaxOutput(item);
        }


    }

}
