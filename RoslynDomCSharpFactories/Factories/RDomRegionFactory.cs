using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace RoslynDom.CSharp
{
   public class RDomRegionMiscFactory : RDomMiscFactory<IRegion, RegionDirectiveTriviaSyntax>
   {
      [ExcludeFromCodeCoverage]
      private static string nameof<T>(T value) { return ""; }

      public RDomRegionMiscFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return 0; } }

      public override bool CanCreateFrom(SyntaxNode syntaxNode)
      { return true; }

      protected override IEnumerable<IMisc> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         return InternalCreateFrom(syntaxNode, parent, model);
      }

      private override IEnumerable<IMisc> InternalCreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var ret = new List<IRegion>();
         if (!syntaxNode.HasLeadingTrivia) return ret;
         var triviaList = syntaxNode.GetLeadingTrivia();
         var precedingTrivia = new List<SyntaxTrivia>();
         foreach (var trivia in triviaList)
         {

            if (trivia.CSharpKind() == SyntaxKind.RegionDirectiveTrivia)
            {
               ret.Add(MakeRegion(syntaxNode, precedingTrivia, trivia));
            }
            precedingTrivia.Add(trivia);
         }
         return ret;
      }

      private IRegion MakeRegion(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia, SyntaxTrivia trivia)
      {
         if (!trivia.HasStructure()) return null;
         var regionSyntax = trivia as RegionDirectiveTriviaSyntax;
         var regionText = regionSyntax.
      }

      private RDomComment MakeComment(SyntaxNode syntaxNode, List<SyntaxTrivia> precedingTrivia,
              SyntaxTrivia trivia, bool isMultiline)
      {
         var commentText = trivia.ToString();
         var tuple = ExtractComment(trivia.ToString());
         var newComment = new RDomComment(tuple.Item2, isMultiline);
         triviaManager.StoreWhitespaceForComment(newComment, precedingTrivia, tuple.Item1, tuple.Item3);
         return newComment;
      }


      private Tuple<string, string, string> ExtractComment(string text)
      {

         if (text.StartsWith("//")) { text = text.Substring(2); }
         if (text.StartsWith("/*"))
         {
            text = text.Substring(2);
            if (text.EndsWith("*/"))
            { text = text.Substring(0, text.Length - 2); }
         }
         // TODO: Ensure you test with whitespace only comment of both types
         var trailing = text.SubstringAfter(text.TrimEnd());
         var leading = text.SubstringBefore(text.TrimStart());
         return Tuple.Create(leading, text.Trim(), trailing);
      }
      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         return null;
      }

      private static WhitespaceKindLookup _whitespaceLookup;

      private static WhitespaceKindLookup whitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.InterfaceKeyword, SyntaxKind.InterfaceKeyword);
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.Add(LanguageElement.InterfaceStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.InterfaceEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterStartDelimiter, SyntaxKind.LessThanToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterEndDelimiter, SyntaxKind.GreaterThanToken);
               _whitespaceLookup.Add(LanguageElement.BaseListPrefix, SyntaxKind.ColonToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }
   }

}
