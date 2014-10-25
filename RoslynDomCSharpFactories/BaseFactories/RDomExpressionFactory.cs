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
   public class RDomBaseItemFactory
                : RDomBaseSyntaxNodeFactory<RDomExpression, ExpressionSyntax>
   {
      public RDomBaseItemFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override Type[] SyntaxNodeTypes
      { get { return null; } }

      public override Type[] ExplicitNodeTypes
      { get { return new Type[] { typeof(IExpression) }; } }

      public override Func<SyntaxNode, IDom, SemanticModel, bool> CanCreateDelegate
      {
         get
         {
            return (syntax, parent, model) =>
            {
               if (syntax is ExpressionSyntax) { return true; }
               return false;
            };
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ExpressionSyntax;

         var newItem = new RDomExpression(syntaxNode, parent, model);
         newItem.Expression = syntax.ToString();
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
         var node = SyntaxFactory.ParseExpression(itemAsT.Expression);
         // TODO: return new SyntaxNode[] { node.Format() };
         return new SyntaxNode[] { node };
      }
   }
}
