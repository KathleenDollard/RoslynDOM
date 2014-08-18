using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomMethodTypeMemberFactory
          : RDomTypeMemberFactory<RDomMethod, MethodDeclarationSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomMethodTypeMemberFactory(RDomCorporation corporation)
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
                _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
                _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
                _whitespaceLookup.Add(LanguageElement.ParameterStartDelimiter, SyntaxKind.OpenParenToken);
                _whitespaceLookup.Add(LanguageElement.ParameterEndDelimiter, SyntaxKind.CloseParenToken);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.OopModifiers);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.StaticModifiers);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as MethodDeclarationSyntax;
            var newItem = new RDomMethod(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, syntax.Body, LanguagePart.Block, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, syntax.ParameterList, LanguagePart.Current, WhitespaceLookup);

            newItem.Name = newItem.TypedSymbol.Name;

            var returnType = Corporation
                            .CreateFrom<IMisc>(syntax.ReturnType, newItem, model)
                            .FirstOrDefault()
                            as IReferencedType;
            newItem.ReturnType = returnType;

            // TODO: Assign IsNew, question on insider's list
            newItem.IsExtensionMethod = newItem.TypedSymbol.IsExtensionMethod;
            var parameters = ListUtilities.MakeList(syntax, x => x.ParameterList.Parameters, x => Corporation.CreateFrom<IMisc>(x, newItem, model))
                                .OfType<IParameter>();
            newItem.Parameters.AddOrMoveRange(parameters);


            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsMethod = item as IMethod;
            var nameSyntax = SyntaxFactory.Identifier(itemAsMethod.Name);

            var returnTypeSyntax = (TypeSyntax)RDomCSharp.Factory.BuildSyntaxGroup(itemAsMethod.ReturnType).First();
            var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsMethod);
            var node = SyntaxFactory.MethodDeclaration(returnTypeSyntax, nameSyntax)
                            .WithModifiers(modifiers);
            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsMethod.Whitespace2Set, WhitespaceLookup);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsMethod.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            var parameterSyntaxList = itemAsMethod.Parameters
                        .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .OfType<ParameterSyntax>()
                        .ToList();
            //if (parameterSyntaxList.Any())
            //{
            var parameterListSyntax = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterSyntaxList));
            parameterListSyntax = BuildSyntaxHelpers.AttachWhitespace(parameterListSyntax, itemAsMethod.Whitespace2Set, WhitespaceLookup);
            node = node.WithParameterList(parameterListSyntax);
            //}

            //node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

            //node = node.WithBody(RoslynCSharpUtilities.MakeStatementBlock(itemAsMethod.Statements));
            node = node.WithBody((BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsMethod.Statements, itemAsMethod, WhitespaceLookup));

            // TODO: typeParameters  and constraintClauses 

            return node.PrepareForBuildSyntaxOutput(item);
        }

    }


}
