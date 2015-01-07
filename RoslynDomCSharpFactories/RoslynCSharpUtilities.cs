using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   public static class RoslynCSharpUtilities
   {
      public static string NameFrom(this SyntaxNode node)
      {
         var qualifiedNameNode = node.ChildNodes()
                                   .OfType<QualifiedNameSyntax>()
                                   .SingleOrDefault();
         var identifierNameNodes = node.ChildNodes()
                            .OfType<IdentifierNameSyntax>()
                            .ToList();
         var name = "";
         if (qualifiedNameNode != null)
         {
            name = name + qualifiedNameNode.ToString();
         }
         foreach (var identifierNameNode in identifierNameNodes)
         {
            var identifierName = identifierNameNode.ToString();
            if (!(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(identifierName)))
            { name += "."; }
            name = name += identifierName;
         }
         if (!string.IsNullOrWhiteSpace(name)) return name;
         var nameToken = node.ChildTokens()
                                   .Where(x => x.CSharpKind() == SyntaxKind.IdentifierToken)
                                   .SingleOrDefault();
         return nameToken.ValueText;
      }

      public static StatementSyntax BuildStatement(IEnumerable<IStatement> statements,
           IStatementBlock parent, WhitespaceKindLookup whitespaceLookup)
      {
         StatementSyntax statementBlock;
         var statementSyntaxList = statements
                      .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                      .ToList();
         var hasBlock = parent.HasBlock;
         if (hasBlock || statements.Count() > 1)
         {
            statementBlock = SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList));
            statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, parent.Whitespace2Set, whitespaceLookup);
            // Block tokens are held in parent
         }
         else if (statements.Count() == 1)
         {
            statementBlock = (StatementSyntax)statementSyntaxList.First();
            //statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, parent.Whitespace2Set, whitespaceLookup);
         }
         else
         {
            statementBlock = SyntaxFactory.EmptyStatement();
            statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, parent.Whitespace2Set, whitespaceLookup);
         }
         return statementBlock;
      }
   }
}