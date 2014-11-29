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
   public class RDomExpressionFactory
                : RDomBaseSyntaxNodeFactory<RDomBaseExpression, ExpressionSyntax>
   {
      public RDomExpressionFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return RDomPriority.Fallback; } }

      public override Type[] SpecialExplicitDomTypes
      { get { return new[] { typeof(IExpression) }; } }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ExpressionSyntax;

         if (syntaxNode is IdentifierNameSyntax)
         {
            // TODO: Work out how to send this via the normal extensible system
            var newItem = new RDomOtherExpression(syntaxNode, parent, model);
            newItem.InitialExpressionString = syntax.ToString();
            newItem.InitialExpressionLanguage = ExpectedLanguages.CSharp;
            newItem.ExpressionType = ExpressionType.Identifier;
            return newItem;
         }
         else
         {
            return OutputContext.Corporation.Create(syntaxNode, parent, model).FirstOrDefault();
         }
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
