using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class RDomInvocationStatementFactory
        : RDomBaseSyntaxNodeFactory<RDomInvocationStatement, ExpressionStatementSyntax>
   {
      public RDomInvocationStatementFactory(RDomCorporation corporation)
         : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return RDomPriority.Normal - 1; } }

      public override bool CanCreate(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var statement = syntaxNode as ExpressionStatementSyntax;
         if (statement == null) { return false; }
         return (statement.Expression is InvocationExpressionSyntax);
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ExpressionStatementSyntax;
         var newItem = new RDomInvocationStatement(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
         CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, syntax, LanguagePart.Current,
                                 LanguageElement.Expression);

         var expression = syntax.Expression;
         //newItem.Invocation = (IInvocationExpression)OutputContext.Corporation.Create<IExpression>(expression, newItem, model).FirstOrDefault();
         newItem.Invocation = OutputContext.Corporation.Create(expression, newItem, model).FirstOrDefault() as IInvocationExpression;
         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IInvocationStatement;
         var expressionSyntax = RDom.CSharp.GetSyntaxNode(itemAsT.Invocation);
         var node = SyntaxFactory.ExpressionStatement((ExpressionSyntax)expressionSyntax);

         node = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(node,
                     itemAsT.Whitespace2Set[LanguageElement.Expression]);
         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }
   }
}