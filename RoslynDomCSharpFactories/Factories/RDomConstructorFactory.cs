using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomConstructorTypeMemberFactory
          : RDomTypeMemberFactory<RDomConstructor, ConstructorDeclarationSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomConstructorTypeMemberFactory(RDomCorporation corporation)
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
                _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ConstructorDeclarationSyntax;
            var newItem = new RDomConstructor(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);
            CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);

            newItem.Name = newItem.TypedSymbol.Name;

            //var typeParameters = newItem.TypedSymbol.TypeParametersFrom();

            newItem.AccessModifier = RoslynUtilities.GetAccessibilityFromSymbol(newItem.Symbol);
            newItem.IsStatic = newItem.Symbol.IsStatic;
            // TODO: Assign IsNew, question on insider's list
            var parameters = ListUtilities.MakeList(syntax, x => x.ParameterList.Parameters, x => Corporation.CreateFrom<IMisc>(x, newItem, model))
                                .OfType<IParameter>();
            newItem.Parameters.AddOrMoveRange(parameters);


            if (syntax.Initializer == null)
            { newItem.ConstructorInitializerType = ConstructorInitializerType.None; }
            else
            {
                var initializerSyntax = syntax.Initializer;
                if (initializerSyntax.ThisOrBaseKeyword.ToString() == "this")
                { newItem.ConstructorInitializerType = ConstructorInitializerType.This; }
                else
                { newItem.ConstructorInitializerType = ConstructorInitializerType.Base; }
                foreach (var arg in initializerSyntax.ArgumentList.Arguments)
                {
                    var newArg = new RDomArgument(arg, newItem, model);
                    // I don't think constructor arguments can be out or ref
                    newArg.ValueExpression = Corporation.CreateFrom<IExpression>(arg.Expression, newItem, model).FirstOrDefault();
                    newItem.InitializationArguments.AddOrMove(newArg);
                }
            }

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsConstructor = item as IConstructor;
            var nameSyntax = SyntaxFactory.Identifier(itemAsConstructor.Name);

            var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsConstructor);
            var node = SyntaxFactory.ConstructorDeclaration(nameSyntax)
                            .WithModifiers(modifiers);
            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsConstructor.Whitespace2Set, WhitespaceLookup);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsConstructor.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            var parameterSyntaxList = itemAsConstructor.Parameters
                        .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .OfType<ParameterSyntax>()
                        .ToList();
            if (parameterSyntaxList.Any()) { node = node.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterSyntaxList))); }

            node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

            //node = node.WithBody(RoslynCSharpUtilities.MakeStatementBlock(itemAsConstructor.Statements));
            node = node.WithBody((BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsConstructor.Statements, itemAsConstructor, WhitespaceLookup));

            // TODO: typeParameters  and constraintClauses 

            return node.PrepareForBuildSyntaxOutput(item);
        }

    }


}
