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
   public class RDomLambdaExpressionFactory
                : RDomBaseSyntaxNodeFactory<ILambdaExpression, SimpleLambdaExpressionSyntax>
   {
      public RDomLambdaExpressionFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return RDomPriority.Normal + 1; } }

      public override Type[] SupportedSyntaxNodeTypes
      { get { return new Type[] { typeof(ParenthesizedLambdaExpressionSyntax), typeof(SimpleLambdaExpressionSyntax) }; } }

 
      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var parenthesizedSyntax = syntaxNode as ParenthesizedLambdaExpressionSyntax;
         ILambdaExpression newItem = null ;
         if (parenthesizedSyntax != null)
         {
            newItem = CreateItemFromInternal(parenthesizedSyntax.Body, syntaxNode,parent, model);
            newItem.Parameters.CreateAndAdd(parenthesizedSyntax, x => x.ParameterList.Parameters, x => OutputContext.Corporation.Create(x, newItem, model).Cast<IParameter>());
         }
         var syntax = syntaxNode as SimpleLambdaExpressionSyntax;
         if (syntax != null)
         {
            newItem = CreateItemFromInternal(syntax.Body, syntaxNode,parent, model);
            newItem.Parameters.CreateAndAdd(syntax, x => new[] { x.Parameter }, x => OutputContext.Corporation.Create(x, newItem, model).Cast<IParameter>());
         }
         newItem.InitialExpressionString = syntax.ToString();
         newItem.InitialExpressionLanguage = ExpectedLanguages.CSharp;

         return newItem;

      }

      private ILambdaExpression CreateItemFromInternal(CSharpSyntaxNode body , SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var block = body as BlockSyntax;
         if (block != null )
         {
            var newItem = new RDomLambdaMultiLineExpression(syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, block, newItem, model);
            return newItem;
         }
         else
         {
            var exp = body as IExpression;
            var newItem = new RDomLambdaSingleExpression(syntaxNode, parent, model);
            newItem.Expression = exp;
            return newItem;
         }
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IExpression;
         if (itemAsT.InitialExpressionLanguage  != ExpectedLanguages.CSharp) { throw new InvalidOperationException(); }
         var node = SyntaxFactory.ParseExpression(itemAsT.InitialExpressionString);
         // TODO: return new SyntaxNode[] { node.Format() };
         return new SyntaxNode[] { node };
      }
   }
}
