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
   public class RDomInvocationExpressionFactory
                : RDomBaseSyntaxNodeFactory<RDomInvocationExpression, InvocationExpressionSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomInvocationExpressionFactory(RDomCorporation corporation)
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
               _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.Add(LanguageElement.ParameterStartDelimiter, SyntaxKind.OpenParenToken);
               _whitespaceLookup.Add(LanguageElement.ParameterEndDelimiter, SyntaxKind.CloseParenToken);
               _whitespaceLookup.Add(LanguageElement.ConstructorInitializerPrefix, SyntaxKind.ColonToken);
               _whitespaceLookup.Add(LanguageElement.ConstructorBaseInitializer, SyntaxKind.BaseKeyword);
               _whitespaceLookup.Add(LanguageElement.ConstructorThisInitializer, SyntaxKind.ThisKeyword);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.StaticModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      public override RDomPriority Priority
      { get { return RDomPriority.Normal + 1; } }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as InvocationExpressionSyntax;

         var newItem = new RDomInvocationExpression(syntaxNode, parent, model);
         newItem.InitialExpressionString = syntax.ToString();
         newItem.InitialExpressionLanguage = ExpectedLanguages.CSharp;
         newItem.MethodName = GetMethodName(syntax.Expression.ToString());
         newItem.TypeArguments.AddOrMoveRange(GetTypeArguments(syntax.Expression, newItem, model));
         newItem.Arguments.CreateAndAdd(syntax, x => x.ArgumentList.Arguments, x => OutputContext.Corporation.Create(x, newItem, model).Cast<IArgument>());
         CreateFromWorker.StoreWhitespace(newItem, syntax.ArgumentList, LanguagePart.Initializer, WhitespaceLookup);

         return newItem;

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
         var itemAsT = item as IInvocationExpression;
         if (!string.IsNullOrWhiteSpace(itemAsT.InitialExpressionLanguage)
               && itemAsT.InitialExpressionLanguage != ExpectedLanguages.CSharp)
         { throw new InvalidOperationException(); }
         SyntaxNode node;
         if (itemAsT.MethodName != null)
         {
            var methodName = SyntaxFactory.ParseExpression(GetMethodName(itemAsT));
            node = SyntaxFactory.ParseExpression(methodName + "()");
            var argList = itemAsT.Arguments
                   .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                   .OfType<ArgumentSyntax>()
                   .ToList();
            var argListSyntax = SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList(argList));
            argListSyntax = BuildSyntaxHelpers.AttachWhitespace(argListSyntax, itemAsT.Whitespace2Set, WhitespaceLookup, LanguagePart.Initializer);
            node = (node as InvocationExpressionSyntax).WithArgumentList(argListSyntax);
         }
         else
         { node = SyntaxFactory.ParseExpression(itemAsT.InitialExpressionString); }
         return new SyntaxNode[] { node };
      }

      private string GetMethodName(IInvocationExpression itemAsT)
      {
         var methodName =  itemAsT.MethodName;
         if (itemAsT.TypeArguments.Any())
         {
            var typeArgs = itemAsT.TypeArguments
                  .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                  .Select(x => x.ToFullString())
                  .ToList();
            var typeArgString = string.Join(", ", typeArgs);
            methodName +="<" + typeArgString + ">";
         }
         return methodName;
      }
   }
}
