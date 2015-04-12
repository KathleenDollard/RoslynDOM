using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;


namespace RoslynDom.CSharp
{
   public class RDomArgumentFactory
            : RDomBaseSyntaxNodeFactory<RDomArgument, ArgumentSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomArgumentFactory(RDomCorporation corporation)
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
               _whitespaceLookup.Add(LanguageElement.ParameterSeparator, SyntaxKind.CommaToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ArgumentSyntax;
         var newItem = new RDomArgument(syntax, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);

         newItem.ValueExpression = OutputContext.Corporation.CreateSpecial<IExpression>(syntax.Expression, newItem, model).FirstOrDefault();
         newItem.IsOut = syntax.ChildTokens().Any(x => x.Kind() == SyntaxKind.OutKeyword);
         newItem.IsRef = syntax.ChildTokens().Any(x => x.Kind() == SyntaxKind.RefKeyword);
         if (syntax.NameColon != null)
         {
            newItem.Name = syntax.NameColon.Name.ToString();
         }

         CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, syntax, LanguagePart.Current, LanguageElement.OutModifier);
         CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, syntax, LanguagePart.Current, LanguageElement.RefModifier);

         MemberWhitespace(newItem, syntax);
         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IArgument ;
         var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
         var expressionSyntax = (ExpressionSyntax)RDom.CSharp.GetSyntaxNode(itemAsT.ValueExpression);
         expressionSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(expressionSyntax, itemAsT.Whitespace2Set[LanguageElement.Expression]);
         var node = SyntaxFactory.Argument(expressionSyntax);
         if (itemAsT.IsOut)
         { node = node.WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.OutKeyword)); }
         else if (itemAsT.IsRef)
         { node = node.WithRefOrOutKeyword(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
         node = BuildSyntaxHelpers.AttachWhitespaceToFirst(node, itemAsT.Whitespace2Set[LanguageElement.OutModifier]);
         node = BuildSyntaxHelpers.AttachWhitespaceToFirst(node, itemAsT.Whitespace2Set[LanguageElement.RefModifier]);

         node = BuildSyntaxHelpers.AttachWhitespace(node, item.Whitespace2Set, WhitespaceLookup);
         node = BuildSyntaxHelpers.AttachWhitespaceToFirst(node, item.Whitespace2Set[LanguageElement.ParameterFirstToken]);
         node = BuildSyntaxHelpers.AttachWhitespaceToLast(node, item.Whitespace2Set[LanguageElement.ParameterLastToken]);
         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }

      private void MemberWhitespace(RDomArgument newItem, ArgumentSyntax  syntax)
      {
         CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetFirstToken(), LanguagePart.Current, LanguageElement.ParameterFirstToken);
         CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetLastToken(), LanguagePart.Current, LanguageElement.ParameterLastToken);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

         CreateFromWorker.StoreListMemberWhitespace(syntax,
               WhitespaceLookup.Lookup(LanguageElement.ParameterSeparator),
               LanguageElement.ParameterFirstToken, newItem);
      }
   }
}
