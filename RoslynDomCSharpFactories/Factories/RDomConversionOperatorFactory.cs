using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomConversionOperatorTypeMemberFactory
          : RDomTypeMemberFactory<RDomConversionOperator, ConversionOperatorDeclarationSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomConversionOperatorTypeMemberFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
            if (_whitespaceLookup == null)
            {
                _whitespaceLookup = new WhitespaceKindLookup();
                _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ConversionOperatorDeclarationSyntax;
            var newItem = new RDomConversionOperator(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);
            CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);

            newItem.Name = newItem.TypedSymbol.Name;

            //var typeParameters = newItem.TypedSymbol.TypeParametersFrom();

            newItem.AccessModifier = RoslynUtilities.GetAccessibilityFromSymbol(newItem.Symbol);

            //newItem.Type = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.ReturnType);
            var type = Corporation
                            .CreateFrom<IMisc>(syntax.Type, newItem, model)
                            .FirstOrDefault()
                            as IReferencedType;
            newItem.Type = type;

            newItem.IsStatic = newItem.Symbol.IsStatic;
            // TODO: Assign IsNew, question on insider's list
            var parameters = ListUtilities.MakeList(syntax, x => x.ParameterList.Parameters, x => Corporation.CreateFrom<IMisc>(x, newItem, model))
                                .OfType<IParameter>();

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IConversionOperator;

            var typeSyntax = (TypeSyntax)RDomCSharp.Factory.BuildSyntaxGroup(itemAsT.Type).First();
            var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
            var x = SyntaxFactory.Token(SyntaxKind.ImplicitKeyword);
            var kind = itemAsT.IsImplicit ? SyntaxKind.ImplicitKeyword : SyntaxKind.ExplicitKeyword;
            var node = SyntaxFactory.ConversionOperatorDeclaration(SyntaxFactory.Token(kind), typeSyntax)
                            .WithModifiers(modifiers);
            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

           // node = node.WithBody(RoslynCSharpUtilities.MakeStatementBlock(itemAsT.Statements));
            node = node.WithBody((BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup));

            // TODO: typeParameters  and constraintClauses 

            return node.PrepareForBuildSyntaxOutput(item);
        }

    }


}
