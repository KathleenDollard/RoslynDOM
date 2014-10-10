
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
   public class TriviaManager
   {
      internal void StoreWhitespace(IDom newItem, SyntaxNode syntaxNode, LanguagePart languagePart, WhitespaceKindLookup whitespaceLookup)
      {
         if (syntaxNode == null) return;

         // For now, all expressions are held as strings, so we just care about first/last
         var nodeAsTypeSyntax = syntaxNode as TypeSyntax;
         if (nodeAsTypeSyntax != null)
         {
            StoreWhitespaceForType(newItem, nodeAsTypeSyntax, languagePart);
            return;
         }
         else
         {
            // For now, all expressions are held as strings, so we just care about first/last
            var nodeAsExpressionSyntax = syntaxNode as ExpressionSyntax;
            if (nodeAsExpressionSyntax != null)
            {
               StoreWhitespaceForExpression(newItem, nodeAsExpressionSyntax, languagePart);
               //return;
            }
         }

         var lookForIdentifier = whitespaceLookup.Lookup(LanguageElement.Identifier) != SyntaxKind.None;
         lookForIdentifier = StoreWhitespaceForChildren(newItem, syntaxNode,
                                 languagePart, whitespaceLookup, lookForIdentifier);
         if (lookForIdentifier)
         { StoreWhitespaceForIdentifierNode(newItem, syntaxNode, languagePart); }
      }

      private void StoreWhitespaceForType(IDom newItem, TypeSyntax nodeAsTypeSyntax, LanguagePart languagePart)
      {
         StoreWhitespaceForFirstAndLastToken(newItem, nodeAsTypeSyntax, languagePart, LanguageElement.LastToken);
      }

      private void StoreWhitespaceForExpression(IDom newItem, ExpressionSyntax syntaxNode,
             LanguagePart languagePart)
      {
         StoreWhitespaceForFirstAndLastToken(newItem, syntaxNode,
                 languagePart, LanguageElement.Expression);
      }

      private void StoreWhitespaceForIdentifierNode(IDom newItem, SyntaxNode syntaxNode,
                  LanguagePart languagePart)
      {
         // assume if it was a token we already found it
         var idNode = syntaxNode.ChildNodes().OfType<NameSyntax>().FirstOrDefault();
         if (idNode != null)
         {
            StoreWhitespaceForFirstAndLastToken(newItem, idNode,
                    languagePart, LanguageElement.Identifier);
         }
      }

      private bool StoreWhitespaceForChildren(IDom newItem, SyntaxNode syntaxNode,
                  LanguagePart languagePart, WhitespaceKindLookup whitespaceLookup, bool lookForIdentifier)
      {
         foreach (var token in syntaxNode.ChildTokens())
         {
            var kind = token.CSharpKind();
            var languageElement = whitespaceLookup.Lookup(kind);
            if (languageElement == LanguageElement.Identifier)
            { lookForIdentifier = false; }
            if (languageElement != LanguageElement.NotApplicable)
            { StoreWhitespaceForToken(newItem, token, languagePart, languageElement); }
         }

         return lookForIdentifier;
      }

      internal void StoreWhitespaceForToken(IDom newItem, SyntaxToken token,
                  LanguagePart languagePart, LanguageElement languageElement)
      {
         if (languagePart == LanguagePart.None) { languagePart = LanguagePart.Current; }
         var newWS = new Whitespace2(languagePart, languageElement);
         newWS.LeadingWhitespace = GetLeadingWhitespaceForToken(token.LeadingTrivia);
         newWS.TrailingWhitespace = GetTrailingWhitespaceForToken(token.TrailingTrivia);
         // TODO: Add EOL comments here
         newItem.Whitespace2Set[languagePart, languageElement] = newWS;
      }

      internal void StoreListMemberWhitespace(SyntaxNode syntax,
                     SyntaxKind sepKind,
                     LanguageElement elementType,
                     IDom newItem)
      {
         var prevNodeOrToken = syntax.Parent
                        .ChildNodesAndTokens()
                        .PreviousSiblings(syntax)
                        .LastOrDefault();
         if (prevNodeOrToken.CSharpKind() == sepKind)
         {
            var commaToken = prevNodeOrToken.AsToken();
            var whitespace2 = newItem.Whitespace2Set[elementType];
            if (whitespace2 == null) throw new NotImplementedException();
            var newLeadingWhitespace = commaToken.TrailingTrivia.ToString(); ;
            if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace)
                || newLeadingWhitespace.EndsWith("\r\n"))
            {
               whitespace2.LeadingWhitespace = newLeadingWhitespace
                                               + whitespace2.LeadingWhitespace;
            }
            //if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace))
            //{ whitespace2.LeadingWhitespace = commaToken.TrailingTrivia.ToString(); }
         }

      }

      internal void StoreWhitespaceForComment(IDom newItem, IEnumerable<SyntaxTrivia> precedingTrivia,
       string leadingWS, string trailingWS)
      {
         var languageElement = LanguageElement.Comment;
         var newWS = new Whitespace2(LanguagePart.Current, languageElement);
         newWS.LeadingWhitespace = GetLeadingWhitespaceForToken(precedingTrivia);
         newWS.TrailingWhitespace = "\r\n";
         newItem.Whitespace2Set.Add(newWS);

         var innerWS = new Whitespace2(LanguagePart.Inner, languageElement);
         innerWS.LeadingWhitespace = leadingWS;
         innerWS.TrailingWhitespace = trailingWS;
         newItem.Whitespace2Set.Add(innerWS);
      }

      public void StoreWhitespaceForFirstAndLastToken(IDom newItem, SyntaxNode node,
              LanguagePart languagePart,
              LanguageElement languageElement)
      {
         if (languagePart == LanguagePart.None) languagePart = LanguagePart.Current;
         var newWS = new Whitespace2(languagePart, languageElement);
         var firstToken = node.GetFirstToken();
         var lastToken = node.GetLastToken();
         newWS.LeadingWhitespace = GetLeadingWhitespaceForToken(firstToken.LeadingTrivia);
         newWS.TrailingWhitespace = GetTrailingWhitespaceForToken(lastToken.TrailingTrivia);
         // TODO: Add EOL comments here
         newItem.Whitespace2Set[languagePart, languageElement] = newWS;
      }

      public void StoreStringWhitespace(IDom newItem, LanguageElement languageElement, string leadingWhitespace, string trailingWhitespace)
      {
         newItem.Whitespace2Set.Add(new Whitespace2( languageElement,  leadingWhitespace, trailingWhitespace, null));
      }

      private string GetLeadingWhitespaceForToken(IEnumerable<SyntaxTrivia> triviaList)
      {
         var list = new List<string>();
         foreach (var trivia in triviaList)
         {
            if (trivia.IsDirective
                || trivia.CSharpKind() == SyntaxKind.MultiLineCommentTrivia
                || trivia.CSharpKind() == SyntaxKind.SingleLineCommentTrivia
                || trivia.CSharpKind() == SyntaxKind.MultiLineDocumentationCommentTrivia
                || trivia.CSharpKind() == SyntaxKind.DocumentationCommentExteriorTrivia
                || trivia.CSharpKind() == SyntaxKind.SingleLineDocumentationCommentTrivia
                || trivia.CSharpKind() == SyntaxKind.EndOfLineTrivia)
            { list = new List<string>(); }
            else
            { list.Add(trivia.ToString()); }
         }
         return list.JoinString();
      }

      private string GetTrailingWhitespaceForToken(IEnumerable<SyntaxTrivia> triviaList)
      {
         return triviaList
                         .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia
                                 || x.CSharpKind() == SyntaxKind.EndOfLineTrivia)
                         .Select(x => x.ToString())
                         .JoinString();

      }

      internal T AttachWhitespace<T>(T syntaxNode, Whitespace2Collection whitespace2Set,
             WhitespaceKindLookup whitespaceLookup)
      where T : SyntaxNode
      {
         return AttachWhitespace(syntaxNode, whitespace2Set, whitespaceLookup, LanguagePart.Current);
      }

      internal T AttachWhitespace<T>(T syntaxNode, Whitespace2Collection whitespace2Set,
              WhitespaceKindLookup whitespaceLookup, LanguagePart languagePart)
       where T : SyntaxNode
      {
         var ret = syntaxNode;
         var whitespaceList = whitespace2Set.Where(x => x.LanguagePart == languagePart);
         var missingWhitespace = whitespaceLookup.NotUsedInWhitespaceList(whitespaceList);
         whitespaceList = whitespaceList
                              .Union(missingWhitespace
                                          .Select(x => new Whitespace2(languagePart, x)));
         foreach (var whitespace in whitespaceList)
         {
            ret = AttachWhitespaceItem(ret, whitespace, whitespaceLookup);
         }
         // Not sure the functional approah is best here
         ret = CheckToken<T>(ret,
                     x => x.GetLastToken(),
                     t => t.TrailingTrivia,
                     s => SyntaxFactory.ParseTrailingTrivia(s),
                     (t, trivia) => t.WithTrailingTrivia(trivia),
                     whitespace2Set.ForceTrailing);
         ret = CheckToken<T>(ret,
                     x => x.GetFirstToken(),
                     t => t.LeadingTrivia,
                     s => SyntaxFactory.ParseLeadingTrivia(s),
                     (t, trivia) => t.WithLeadingTrivia(trivia),
                     whitespace2Set.ForceLeading);
         return ret;
      }

      private T CheckToken<T>(T node, Func<SyntaxNode, SyntaxToken> getToken,
             Func<SyntaxToken, SyntaxTriviaList> getOldTrivia,
             Func<string, SyntaxTriviaList> getNewTrivia,
             Func<SyntaxToken, SyntaxTriviaList, SyntaxToken> setNewTrivia,
             string forceWhitespace)
    where T : SyntaxNode
      {
         if (string.IsNullOrEmpty(forceWhitespace)) return node;
         var lastToken = getToken(node);
         if (getOldTrivia(lastToken).ToFullString() != forceWhitespace)
         {
            var trailingTrivia = getNewTrivia(forceWhitespace);
            lastToken = lastToken.WithTrailingTrivia(trailingTrivia);
            node = node.ReplaceToken(node.GetLastToken(), lastToken);
         }
         return node;
      }


      //private T AttachWhitespaceItem<T>(T syntaxNode, Whitespace2 whitespace,
      //        WhitespaceKindLookup whitespaceLookup)
      //     where T : SyntaxNode
      //{
      //   var ret = syntaxNode;
      //   var kind = whitespaceLookup.Lookup(whitespace.LanguageElement);
      //   var tokens = syntaxNode.ChildTokens().Where(x => x.CSharpKind() == kind);
      //   if (!tokens.Any() && whitespace.LanguageElement == LanguageElement.Identifier)
      //   {
      //      var nameNode = syntaxNode.ChildNodes().OfType<NameSyntax>().FirstOrDefault();
      //      if (nameNode != null)
      //      {
      //         tokens = nameNode.DescendantTokens()
      //                 .Where(x => x.CSharpKind() == kind);
      //         tokens = tokens
      //                 .Where(x => !x.TrailingTrivia.Any(y => RealWhitespace(y)));
      //         tokens = tokens
      //                 .Where(x => !x.LeadingTrivia.Any(y => RealWhitespace(y)));
      //      }
      //   }
      //   else if (!tokens.Any() && whitespace.LanguageElement == LanguageElement.LastToken)
      //   {
      //      var typeNode = syntaxNode.ChildNodes().OfType<NameSyntax>().LastOrDefault();
      //      if (typeNode != null)
      //      {
      //         tokens = typeNode.DescendantTokens()
      //               .Where(x => x.CSharpKind() == SyntaxKind.IdentifierToken)
      //               .Where(x => !x.TrailingTrivia.Any(y => RealWhitespace(y)))
      //               .Where(x => !x.LeadingTrivia.Any(y => RealWhitespace(y)));
      //      }
      //   }           // Sometimes the token won't be there due to changes in the tree. 
      //   tokens = tokens.ToList();
      //   if (tokens.Any())
      //   {
      //      var newToken = tokens.First();
      //      var leadingTrivia = SyntaxFactory.ParseLeadingTrivia(whitespace.LeadingWhitespace)
      //                 .Concat(newToken.LeadingTrivia);
      //      var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(whitespace.TrailingWhitespace)
      //                 .Concat(newToken.TrailingTrivia);
      //      // Manage EOL comment here
      //      newToken = newToken
      //                  .WithLeadingTrivia(leadingTrivia)
      //                  .WithTrailingTrivia(trailingTrivia);
      //      ret = ret.ReplaceToken(tokens.First(), newToken);
      //   }
      //   return ret;
      //}

      private T AttachWhitespaceItem<T>(T syntaxNode, Whitespace2 whitespace,
             WhitespaceKindLookup whitespaceLookup)
          where T : SyntaxNode
      {
         var ret = syntaxNode;
         var name = ret.ToString();
         var kind = whitespaceLookup.Lookup(whitespace.LanguageElement);
         Func<SyntaxNode, IEnumerable<SyntaxToken>> makeTokens = s => s.ChildTokens().Where(x => x.CSharpKind() == kind);
         var tokens = makeTokens(syntaxNode).ToList();
         if (!tokens.Any())
         {
            if (whitespace.LanguageElement == LanguageElement.Identifier)
            {
               makeTokens = s => s.ChildNodes().OfType<NameSyntax>()
                                    .SelectMany(n => n.DescendantTokens()
                                          .Where(x => x.CSharpKind() == kind));
               tokens = makeTokens(syntaxNode).ToList();
               if (!tokens.Any() && syntaxNode.ChildTokens().Any())
               {
                  var testNode = syntaxNode.ChildTokens().First();
                  if (Mappings.IsTypeAlias(testNode.CSharpKind()))
                  { tokens.Add(testNode); }
               }
            }
            else if (whitespace.LanguageElement == LanguageElement.LastToken)
            {
               makeTokens = s => s.ChildNodes().OfType<NameSyntax>()
                                      .SelectMany(n => n.DescendantTokens()
                                          .Where(x => x.CSharpKind() == SyntaxKind.IdentifierToken));
               tokens = makeTokens(syntaxNode).ToList();
            }           // Sometimes the token won't be there due to changes in the tree. 
         }

         if (tokens.Any())
         {
            var token = tokens.First();
            var tokenString = token.ToString();
            var triviaString = token.LeadingTrivia.ToFullString();
            if (whitespace.LeadingWhitespace == null)
            {
               triviaString = triviaString.Length > 0 || token.CSharpKind() == SyntaxKind.SemicolonToken
                            ? triviaString
                            : " ";
            }
            else if (whitespace.LeadingWhitespace.Length > triviaString.Length)
            { triviaString = whitespace.LeadingWhitespace; }
            var leadingTrivia = SyntaxFactory.ParseLeadingTrivia(triviaString);
            token = token.WithLeadingTrivia(leadingTrivia);


            if (tokens.First().Parent != null &&
                  tokens.First().Parent.AncestorsAndSelf().Any(x=>x.CSharpKind() == SyntaxKind.QualifiedName ))
            {
               ret = ret.ReplaceToken(tokens.First(), token);
               tokens = makeTokens(ret).ToList();
               token = tokens.Last();
               triviaString = token.TrailingTrivia.ToFullString();
               if (whitespace.TrailingWhitespace != null && whitespace.TrailingWhitespace.Length > triviaString.Length)
               { triviaString = whitespace.TrailingWhitespace; }
               var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(triviaString);
               token = token.WithTrailingTrivia(trailingTrivia);
               ret = ret.ReplaceToken(tokens.Last(), token);
            }
            else
            {
               triviaString = token.TrailingTrivia.ToFullString();
               if (whitespace.TrailingWhitespace != null && whitespace.TrailingWhitespace.Length > triviaString.Length)
               { triviaString = whitespace.TrailingWhitespace; }
               var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(triviaString);
               token = token.WithTrailingTrivia(trailingTrivia);
               ret = ret.ReplaceToken(tokens.First(), token);
            }
            // Manage EOL comment here
         }
         return ret;
      }

      private bool RealWhitespace(SyntaxTrivia trivia)
      {
         if (trivia.CSharpKind() != SyntaxKind.WhitespaceTrivia) return false;
         return (!string.IsNullOrEmpty(trivia.ToFullString()));
      }

      internal T AttachWhitespaceToFirst<T>(T syntaxNode, Whitespace2 whitespace2)
              where T : SyntaxNode
      {
         if (whitespace2 == null) { return syntaxNode; }
         var token = syntaxNode.GetFirstToken();
         var ret = syntaxNode.ReplaceToken(token, AttachLeadingWhitespaceToToken(token, whitespace2));
         return ret;
      }

      internal T AttachWhitespaceToLast<T>(T syntaxNode, Whitespace2 whitespace2)
               where T : SyntaxNode
      {
         if (whitespace2 == null) { return syntaxNode; }
         var token = syntaxNode.GetLastToken();
         var ret = syntaxNode.ReplaceToken(token, AttachTrailingWhitespaceToToken(token, whitespace2));
         return ret;
      }

      private SyntaxToken AttachLeadingWhitespaceToToken(SyntaxToken token, Whitespace2 whitespace2)
      {
         if (token.CSharpKind() == SyntaxKind.None
                  || whitespace2 == null
                  || whitespace2.LeadingWhitespace == null)
         { return token; }
         var leadingTrivia = SyntaxFactory.ParseLeadingTrivia(whitespace2.LeadingWhitespace)
                    .Concat(token.LeadingTrivia);
         return token.WithLeadingTrivia(leadingTrivia);
      }

      private SyntaxToken AttachTrailingWhitespaceToToken(SyntaxToken token, Whitespace2 whitespace2)
      {
         if (token.CSharpKind() == SyntaxKind.None
                  || whitespace2 == null
                  || whitespace2.TrailingWhitespace == null)
         { return token; }
         var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(whitespace2.TrailingWhitespace)
                    .Concat(token.TrailingTrivia);
         // Manage EOL comment here
         return token.WithTrailingTrivia(trailingTrivia);
      }

      internal SyntaxToken AttachWhitespaceToToken(SyntaxToken token, Whitespace2 whitespace2)
      {
         token = AttachLeadingWhitespaceToToken(token, whitespace2);
         token = AttachTrailingWhitespaceToToken(token, whitespace2);
         return token;
      }

      internal T AttachWhitespaceToFirstAndLast<T>(T syntaxNode, Whitespace2 whitespace2)
       where T : SyntaxNode
      {
         if (whitespace2 == null) { return syntaxNode; }
         syntaxNode = AttachWhitespaceToFirst(syntaxNode, whitespace2);
         syntaxNode = AttachWhitespaceToLast(syntaxNode, whitespace2);
         return syntaxNode;
      }


   }
}
