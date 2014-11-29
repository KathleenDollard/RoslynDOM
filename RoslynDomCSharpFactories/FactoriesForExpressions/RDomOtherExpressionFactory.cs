using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   // Sample of extending factory
   public class RDomOtherExpressionFactory
                : RDomBaseSyntaxNodeFactory<RDomOtherExpression, ExpressionSyntax>
   {
      public RDomOtherExpressionFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return RDomPriority.Top; } }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ExpressionSyntax;

         var newItem = new RDomOtherExpression(syntaxNode, parent, model);
         newItem.InitialExpressionString = syntax.ToString();
         newItem.InitialExpressionLanguage = ExpectedLanguages.CSharp;
         newItem.ExpressionType = ExpressionTypeFromSyntax(syntaxNode);

         return newItem;

      }

      private ExpressionType ExpressionTypeFromSyntax(SyntaxNode syntaxNode)
      {
         if (syntaxNode is LiteralExpressionSyntax) { return ExpressionType.Literal; }
         if (syntaxNode is ObjectCreationExpressionSyntax) { return ExpressionType.ObjectCreation; }
         if (syntaxNode is InvocationExpressionSyntax) { return ExpressionType.Invocation; }
         if (syntaxNode is IdentifierNameSyntax) { return ExpressionType.Identifier; }
         if (syntaxNode is BinaryExpressionSyntax) { return ExpressionType.Complex; }
         return ExpressionType.Unknown;
      }


      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IExpression;
         if (itemAsT.InitialExpressionLanguage != ExpectedLanguages.CSharp) { throw new InvalidOperationException(); }
         var node = SyntaxFactory.ParseExpression(itemAsT.InitialExpressionString);
         // TODO: return new SyntaxNode[] { node.Format() };
         return new SyntaxNode[] { node };
      }
   }
}
