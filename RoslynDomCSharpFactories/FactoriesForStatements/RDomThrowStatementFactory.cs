using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class RDomThrowStatementFactory
               : RDomBaseSyntaxNodeFactory<RDomThrowStatement, ThrowStatementSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomThrowStatementFactory(RDomCorporation corporation)
         : base(corporation)
      { }

      private WhitespaceKindLookup WhitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.Throw, SyntaxKind.ThrowKeyword);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ThrowStatementSyntax;
         var newItem = new RDomThrowStatement(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

         if (syntax.Expression != null)
         {
            var expression = OutputContext.Corporation.CreateSpecial<IExpression>(syntax.Expression, newItem, model).FirstOrDefault();
            newItem.ExceptionExpression = expression;
         }

         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IThrowStatement;
         var node = SyntaxFactory.ThrowStatement();
         var exception = (ExpressionSyntax)RDom.CSharp.GetSyntaxNode(itemAsT.ExceptionExpression);
         if (exception != null) node = node.WithExpression(exception);

         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }
   }
}