using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom.CSharp
{
   public class RDomParameterMiscFactory
           : RDomBaseItemFactory<RDomParameter, ParameterSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomParameterMiscFactory(RDomCorporation corporation)
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
               _whitespaceLookup.Add(LanguageElement.OutParameter, SyntaxKind.OutKeyword);
               _whitespaceLookup.Add(LanguageElement.RefParameter, SyntaxKind.RefKeyword);
               _whitespaceLookup.Add(LanguageElement.ParamsParameter, SyntaxKind.ParamsKeyword);
               _whitespaceLookup.Add(LanguageElement.ParameterDefaultAssignOperator, SyntaxKind.EqualsToken);
               _whitespaceLookup.Add(LanguageElement.ParameterSeparator, SyntaxKind.CommaToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ParameterSyntax;
         var newItem = new RDomParameter(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

         newItem.Name = newItem.TypedSymbol.Name;

         var type = OutputContext.Corporation
                          .Create(syntax.Type, newItem, model)
                          .FirstOrDefault()
                          as IReferencedType;
         newItem.Type = type;

         newItem.IsOut = newItem.TypedSymbol.RefKind == RefKind.Out;
         newItem.IsRef = newItem.TypedSymbol.RefKind == RefKind.Ref;
         newItem.IsParamArray = newItem.TypedSymbol.IsParams;
         newItem.IsOptional = newItem.TypedSymbol.IsOptional;
         if (syntax.Default != null)
         {
            var tuple = CreateFromWorker.GetArgumentValue(newItem, model, syntax.Default.Value);
            newItem.DefaultValue = tuple.Item1;
            newItem.DefaultConstantIdentifier = tuple.Item2;
            newItem.DefaultValueType = tuple.Item3;
         }
         newItem.Ordinal = newItem.TypedSymbol.Ordinal;
         MemberWhitespace(newItem, syntax);

         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IParameter;
         Guardian.Assert.IsNotNull(itemAsT, nameof(itemAsT));

         var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);
         var syntaxType = (TypeSyntax)(RDom.CSharp.GetSyntaxNode(itemAsT.Type));
         syntaxType = BuildSyntaxHelpers.RemoveLeadingSpaces(syntaxType);

         var node = SyntaxFactory.Parameter(nameSyntax)
                     .WithType(syntaxType);

         if (itemAsT.DefaultValueType != LiteralKind.Unknown)
         {
            var defaultValueExpression = BuildSyntaxHelpers.BuildArgValueExpression(
                        itemAsT.DefaultValue, itemAsT.DefaultConstantIdentifier, itemAsT.DefaultValueType);
            var defaultClause = SyntaxFactory.EqualsValueClause(defaultValueExpression);
            defaultClause = BuildSyntaxHelpers.AttachWhitespace(defaultClause, item.Whitespace2Set, WhitespaceLookup);
            node = node.WithDefault(defaultClause);
         }

         var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         var modifiers = SyntaxFactory.TokenList();
         if (itemAsT.IsOut) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.OutKeyword)); }
         if (itemAsT.IsRef) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
         if (itemAsT.IsParamArray) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ParamsKeyword)); }
         if (modifiers.Any()) { node = node.WithModifiers(modifiers); }

         node = BuildSyntaxHelpers.AttachWhitespace(node, item.Whitespace2Set, WhitespaceLookup);
         node = BuildSyntaxHelpers.AttachWhitespaceToFirst(node, item.Whitespace2Set[LanguageElement.ParameterFirstToken]);
         node = BuildSyntaxHelpers.AttachWhitespaceToLast(node, item.Whitespace2Set[LanguageElement.ParameterLastToken]);
         return node.PrepareForBuildSyntaxOutput(item, OutputContext);

      }
      private void MemberWhitespace(RDomParameter newItem, ParameterSyntax syntax)
      {
         CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetFirstToken(), LanguagePart.Current, LanguageElement.ParameterFirstToken);
         CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetLastToken(), LanguagePart.Current, LanguageElement.ParameterLastToken);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
         if (syntax.Default != null)
         {
            CreateFromWorker.StoreWhitespace(newItem, syntax.Default, LanguagePart.Current, WhitespaceLookup);
            //CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.Default.Value.GetLastToken(), LanguagePart.Current, LanguageElement.Identifier);
            //CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.Default.EqualsToken, LanguagePart.Current, LanguageElement.ParameterDefaultAssignOperator);
         }

         CreateFromWorker.StoreListMemberWhitespace(syntax,
               WhitespaceLookup.Lookup(LanguageElement.ParameterSeparator),
               LanguageElement.ParameterFirstToken, newItem);

         //var prevNodeOrToken = syntax.Parent
         //                        .ChildNodesAndTokens()
         //                        .PreviousSiblings(syntax)
         //                        .LastOrDefault();
         //var sepKind = WhitespaceLookup.Lookup(LanguageElement.ParameterSeparator);
         //if (prevNodeOrToken.CSharpKind() == sepKind)
         //{
         //   var commaToken = prevNodeOrToken.AsToken();
         //   var whitespace2 = newItem.Whitespace2Set[LanguageElement.ParameterFirstToken];
         //   if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace))
         //   { whitespace2.LeadingWhitespace = commaToken.TrailingTrivia.ToString(); }
         //}
      }
   }

}
