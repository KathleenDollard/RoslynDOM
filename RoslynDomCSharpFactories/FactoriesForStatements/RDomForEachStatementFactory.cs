using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class RDomForEachStatementFactory
        : RDomBaseLoopStatementFactory<RDomForEachStatement, ForEachStatementSyntax>
   {
      public RDomForEachStatementFactory(RDomCorporation corporation)
         : base(corporation)
      { }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ForEachStatementSyntax;
         var newItem = base.CreateItemFrom(syntaxNode, parent, model) as RDomForEachStatement;
         newItem.TestAtEnd = false; // restating the default
         var newVariable = OutputContext.Corporation.CreateSpecial<IVariableDeclaration>(syntaxNode, newItem, model).FirstOrDefault();
         newItem.Variable = (IVariableDeclaration)newVariable;
         return newItem;
      }

      protected override ExpressionSyntax GetConditionFromSyntax(ForEachStatementSyntax syntax)
      {
         return syntax.Expression;
      }

      protected override StatementSyntax GetStatementFromSyntax(ForEachStatementSyntax syntax)
      {
         return syntax.Statement;
      }

      protected override RDomForEachStatement MakeNewItem(ForEachStatementSyntax syntax, IDom parent, SemanticModel model)
      {
         return new RDomForEachStatement(syntax, parent, model);
      }

      protected override ForEachStatementSyntax MakeSyntax(
              RDomForEachStatement itemAsT, ExpressionSyntax condition, StatementSyntax statementBlock)
      {
         var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(
                     itemAsT.Variable.IsImplicitlyTyped, itemAsT.Variable.Type);
         //var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(itemAsT.Variable);
         //typeSyntax = BuildSyntaxHelpers.AttachWhitespace(typeSyntax, itemAsT.Whitespace2Set, WhitespaceLookup);
         var id = itemAsT.Variable.Name;
         return SyntaxFactory.ForEachStatement(typeSyntax, id, condition, statementBlock);
      }
   }
}