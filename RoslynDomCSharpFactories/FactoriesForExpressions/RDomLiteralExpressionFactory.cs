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
   public class RDomLiteralExpressionFactory
                : RDomBaseSyntaxNodeFactory<RDomLiteralExpression, LiteralExpressionSyntax>
   {
      public RDomLiteralExpressionFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return RDomPriority.Normal + 1; } }


      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as LiteralExpressionSyntax;

         var newItem = new RDomLiteralExpression(syntaxNode, parent, model);
         newItem.InitialExpressionString = syntax.ToString();
         newItem.InitialExpressionLanguage = ExpectedLanguages.CSharp;
         newItem.Literal = syntax.Token.Value;
         return newItem;

      }

      private IEnumerable<IArgument> GetArguments(ArgumentListSyntax argumentList, IDom newItem, SemanticModel model)
      {
         var ret = new List<IArgument>();
         foreach (var argSyntax in argumentList.Arguments)
         {
            // TODO: more work, align with constructor args, and probably create factory
            var newArg = new RDomArgument(argSyntax, newItem, model);
            // KAD: Explict node removal
            //newArg.ValueExpression = OutputContext.Corporation.Create<IExpression>(argSyntax.Expression, newItem, model).FirstOrDefault();
            newArg.ValueExpression = OutputContext.Corporation.CreateSpecial<IExpression>(argSyntax.Expression, newItem, model).FirstOrDefault();
            if (argSyntax.NameColon != null)
            {
               newArg.Name = argSyntax.NameColon.Name.ToString();
            }
            newArg.IsRef = argSyntax.RefOrOutKeyword.CSharpKind() == SyntaxKind.RefKeyword;
            newArg.IsOut = argSyntax.RefOrOutKeyword.CSharpKind() == SyntaxKind.OutKeyword;
            ret.Add(newArg);
         }
         return ret;
      }

      private IEnumerable<IReferencedType> GetTypeArguments(ExpressionSyntax expression, IDom newItem, SemanticModel model)
      {
         var ret = new List<IReferencedType>();
         var exp = expression as GenericNameSyntax;
         if (exp == null) return ret;
         foreach (var tArg in exp.TypeArgumentList.Arguments)
         {
            var referenceType = OutputContext.Corporation
                         .Create(tArg, newItem, model)
                         .FirstOrDefault()
                         as IReferencedType;
            ret.Add(referenceType);
         }
         return ret;
      }

      private string GetMethodName(string expression)
      {
         if (expression.Contains("<")) return expression.SubstringBefore("<");
         return expression;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as ILiteralExpression;
         if (!string.IsNullOrWhiteSpace(itemAsT.InitialExpressionLanguage)
               && itemAsT.InitialExpressionLanguage != ExpectedLanguages.CSharp)
         { throw new InvalidOperationException(); }
         // OK, it's sort of sad, but doing this explicitly is massively more complicated
         string literalString;
         if (itemAsT.Literal != null)
         {
            var literalType = itemAsT.Literal.GetType();
            if (literalType == typeof(bool))
            { literalString = itemAsT.Literal.ToString().ToLower(); }
            else if (literalType == typeof(string))
            { literalString = "\"" + itemAsT.Literal.ToString() + "\"" ; }
            else
            // TODO: Once VB factories exist, also handle date literals here
            { literalString = itemAsT.Literal.ToString(); }
         }
         else
         { literalString = itemAsT.InitialExpressionString; }
         var node = SyntaxFactory.ParseExpression(literalString);
         return new SyntaxNode[] { node };
      }
   }
}
