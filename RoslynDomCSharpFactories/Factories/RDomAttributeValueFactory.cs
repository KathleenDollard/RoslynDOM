using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom.CSharp
{
   public class RDomAttributeValueMiscFactory
           : RDomMiscFactory<RDomAttributeValue, AttributeArgumentSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomAttributeValueMiscFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as AttributeArgumentSyntax;
         var newItem = new RDomAttributeValue(syntaxNode, parent, model);
         InitializeAttributeValue(newItem, syntax, model);
         CreateFromWorker.StandardInitialize(newItem, syntax, parent, model);
         StoreWhitespace(newItem, syntax);
         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IAttributeValue;

         var argNameSyntax = SyntaxFactory.IdentifierName(itemAsT.Name);
         argNameSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirst(argNameSyntax, item.Whitespace2Set[LanguageElement.AttributeValueName]);
         argNameSyntax = BuildSyntaxHelpers.AttachWhitespaceToLast(argNameSyntax, item.Whitespace2Set[LanguageElement.AttributeValueName]);

         //var kind = Mappings.SyntaxKindFromLiteralKind(itemAsT.ValueType, itemAsT.Value);
         ExpressionSyntax expr = BuildSyntaxHelpers.BuildArgValueExpression(
                     itemAsT.Value, itemAsT.ValueConstantIdentifier, itemAsT.ValueType);
         var node = SyntaxFactory.AttributeArgument(expr);
         if (itemAsT.Style == AttributeValueStyle.Colon)
         {
            var nameColon = SyntaxFactory.NameColon(argNameSyntax);
            nameColon = BuildSyntaxHelpers.AttachWhitespaceToLast(nameColon, item.Whitespace2Set[LanguageElement.AttributeValueEqualsOrColon]);
            node = node.WithNameColon(nameColon);
         }
         else if (itemAsT.Style == AttributeValueStyle.Equals)
         {
            var nameEquals = SyntaxFactory.NameEquals(argNameSyntax);
            nameEquals = BuildSyntaxHelpers.AttachWhitespaceToLast(nameEquals, item.Whitespace2Set[LanguageElement.AttributeValueEqualsOrColon]);
            node = node.WithNameEquals(nameEquals);
         }
         node = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(node, item.Whitespace2Set[LanguageElement.AttributeValueValue]);

         return node.PrepareForBuildSyntaxOutput(item);
      }

      private void InitializeAttributeValue(IAttributeValue newItem,
                AttributeArgumentSyntax rawItem, SemanticModel model)
      {
         var tuple = GetAttributeValueName(rawItem);
         newItem.Name = tuple.Item1;
         newItem.Style = tuple.Item2;
         var tuple2 = GetAttributeValueValue(rawItem, newItem, model);
         newItem.Value = tuple2.Item1;
         newItem.ValueConstantIdentifier = tuple2.Item2;
         newItem.ValueType = tuple2.Item3;
         newItem.Type = newItem.Value.GetType();
      }

      private void StoreWhitespace(RDomAttributeValue newItem, AttributeArgumentSyntax syntax)
      {
         // TODO: I feel like I'm working too hard here. Try creating a WhitespaceLookup and see how much of this is done in a standard StoreWhitespace

         var op = new SyntaxToken();
         if (syntax.NameColon != null)
         { StoreWhitespaceForNamed(newItem, syntax, syntax.NameColon.Name.Identifier, syntax.NameColon.ColonToken); }
         else if (syntax.NameEquals != null)
         { StoreWhitespaceForNamed(newItem, syntax, syntax.NameEquals.Name.Identifier, syntax.NameEquals.EqualsToken); }
         else
         {
            CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, syntax,
                  LanguagePart.Current, LanguageElement.AttributeValueValue);
         }
         CreateFromWorker.StoreListMemberWhitespace(syntax,
               SyntaxKind.CommaToken, LanguageElement.AttributeValueValue, newItem);
      }

      private void StoreWhitespaceForNamed(RDomAttributeValue newItem, AttributeArgumentSyntax syntax, SyntaxToken identifier, SyntaxToken op)
      {
         CreateFromWorker.StoreWhitespaceForToken(newItem, identifier,
                  LanguagePart.Current, LanguageElement.AttributeValueName);
         CreateFromWorker.StoreWhitespaceForToken(newItem, op, LanguagePart.Current, LanguageElement.AttributeValueEqualsOrColon);
         var lastToken = syntax.GetLastToken();
         CreateFromWorker.StoreWhitespaceForToken(newItem, lastToken, LanguagePart.Current, LanguageElement.AttributeValueValue);

      }


      private Tuple<string, AttributeValueStyle> GetAttributeValueName(AttributeArgumentSyntax arg)
      {
         string name = "";
         AttributeValueStyle style;
         if (arg.NameColon != null)
         {
            style = AttributeValueStyle.Colon;
            name = arg.NameColon.Name.ToString().Replace(":", "").Trim();
         }
         else if (arg.NameEquals != null)
         {
            style = AttributeValueStyle.Equals;
            name = arg.NameEquals.Name.ToString();
         }
         else
         {
            style = AttributeValueStyle.Positional;
            // TODO: Work harder at getting the real parameter name??
         }
         return Tuple.Create(name, style);
      }

      private Tuple<object, string, LiteralKind> GetAttributeValueValue(
                    SyntaxNode argNode, IDom newItem, SemanticModel model)
      {
         var arg = argNode as AttributeArgumentSyntax;
         Guardian.Assert.IsNotNull(arg, nameof(arg));

         // TODO: Manage multiple values because of AllowMultiples, param array, or missing symbol 
         var expr = arg.Expression;
         return CreateFromWorker.GetArgumentValue(newItem, model, expr);
      }



   }

}
