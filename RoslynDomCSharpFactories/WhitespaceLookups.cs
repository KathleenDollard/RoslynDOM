using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class WhitespaceKindLookup
   {
      private Dictionary<SyntaxKind, LanguageElement> kindToLanguageElement = new Dictionary<SyntaxKind, LanguageElement>();
      private Dictionary<LanguageElement, SyntaxKind> languageElementToKind = new Dictionary<LanguageElement, SyntaxKind>();

      public void Add(LanguageElement languageElement, SyntaxKind syntaxKind)
      {
         kindToLanguageElement.Add(syntaxKind, languageElement);
         languageElementToKind.Add(languageElement, syntaxKind);
      }

      public void AddRange(IEnumerable<Tuple<LanguageElement, SyntaxKind>> list)
      {
         foreach (var item in list)
         {
            Add(item.Item1, item.Item2);
         }
      }

      public LanguageElement Lookup(SyntaxKind syntaxKind)
      {
         LanguageElement ret = LanguageElement.NotApplicable;
         kindToLanguageElement.TryGetValue(syntaxKind, out ret);
         return ret;
      }

      public SyntaxKind Lookup(LanguageElement languageElement)
      {
         SyntaxKind ret = SyntaxKind.None;
         languageElementToKind.TryGetValue(languageElement, out ret);
         return ret;
      }

      public static IEnumerable<Tuple<LanguageElement, SyntaxKind>> AccessModifiers = new List<Tuple<LanguageElement, SyntaxKind>>()
            {
                Tuple.Create( LanguageElement.Public ,    SyntaxKind.PublicKeyword ),
                Tuple.Create( LanguageElement.Private ,   SyntaxKind.PrivateKeyword ),
                Tuple.Create( LanguageElement.Protected , SyntaxKind.ProtectedKeyword),
                Tuple.Create( LanguageElement.Internal ,  SyntaxKind.InternalKeyword )
             };

      public static IEnumerable<Tuple<LanguageElement, SyntaxKind>> OopModifiers = new List<Tuple<LanguageElement, SyntaxKind>>()
            {
                Tuple.Create( LanguageElement.Abstract , SyntaxKind.AbstractKeyword  ),
                Tuple.Create( LanguageElement.Override , SyntaxKind.OverrideKeyword ),
                Tuple.Create( LanguageElement.Sealed ,   SyntaxKind.SealedKeyword),
                Tuple.Create( LanguageElement.Virtual ,  SyntaxKind.VirtualKeyword ),
             };

      public static IEnumerable<Tuple<LanguageElement, SyntaxKind>> StaticModifiers = new List<Tuple<LanguageElement, SyntaxKind>>()
            {
                Tuple.Create( LanguageElement.Static , SyntaxKind.StaticKeyword  )
             };

      public static IEnumerable<Tuple<LanguageElement, SyntaxKind>> Eol = new List<Tuple<LanguageElement, SyntaxKind>>()
            {
                Tuple.Create( LanguageElement.EndOfLine  , SyntaxKind.SemicolonToken    )
             };

      public static IEnumerable<Tuple<LanguageElement, SyntaxKind>> AssignmentOperators = new List<Tuple<LanguageElement, SyntaxKind>>()
            {
                Tuple.Create( LanguageElement.EqualsAssignmentOperator,      SyntaxKind.EqualsToken),
                Tuple.Create( LanguageElement.AddAssignmentOperator,         SyntaxKind.PlusEqualsToken),
                Tuple.Create( LanguageElement.SubtractAssignmentOperator,    SyntaxKind.MinusEqualsToken),
                Tuple.Create( LanguageElement.MultiplyAssignmentOperator,    SyntaxKind.AsteriskEqualsToken),
                Tuple.Create( LanguageElement.DivideAssignmentOperator,      SyntaxKind.SlashEqualsToken),
                Tuple.Create( LanguageElement.ModuloAssignmentOperator,      SyntaxKind.PercentEqualsToken),
                Tuple.Create( LanguageElement.AndAssignmentOperator,         SyntaxKind.AmpersandEqualsToken),
                Tuple.Create( LanguageElement.ExclusiveOrAssignmentOperator, SyntaxKind.CaretEqualsToken),
                Tuple.Create( LanguageElement.OrAssignmentOperator,          SyntaxKind.BarEqualsToken),
                Tuple.Create( LanguageElement.LeftShiftAssignmentOperator,   SyntaxKind.LessThanLessThanEqualsToken),
                Tuple.Create( LanguageElement.RightShiftAssignmentOperator,  SyntaxKind.GreaterThanGreaterThanEqualsToken)
             };

      public static IEnumerable<Tuple<LanguageElement, SyntaxKind>> OverloadableOperators = new List<Tuple<LanguageElement, SyntaxKind>>()
            {
               Tuple.Create( LanguageElement.PlusOperator  , SyntaxKind.PlusToken     ),
               Tuple.Create( LanguageElement.MinusOperator  , SyntaxKind.MinusToken     ),
               Tuple.Create( LanguageElement.NotOperator  , SyntaxKind.ExclamationToken  ),
               Tuple.Create( LanguageElement.ComplemententOperator  , SyntaxKind.TildeToken     ),
               Tuple.Create( LanguageElement.IncrementOperator  , SyntaxKind.PlusPlusToken     ),
               Tuple.Create( LanguageElement.DecrementOperator  , SyntaxKind.MinusMinusToken     ),
               Tuple.Create( LanguageElement.TrueOperator  , SyntaxKind.TrueKeyword      ),
               Tuple.Create( LanguageElement.FalseOperator  , SyntaxKind.FalseKeyword      ),
               Tuple.Create( LanguageElement.MultiplyOperator  , SyntaxKind.AsteriskToken   ),
               Tuple.Create( LanguageElement.DivideOperator  , SyntaxKind.SlashToken   ),
               Tuple.Create( LanguageElement.ModulusOperator  , SyntaxKind.PercentToken   ),
               Tuple.Create( LanguageElement.BitwiseAndOperator  , SyntaxKind.AmpersandToken   ),
               Tuple.Create( LanguageElement.BitwiseOrOperator  , SyntaxKind.BarToken   ),
               Tuple.Create( LanguageElement.ExclusiveOrOperator  , SyntaxKind.CaretToken   ),
               Tuple.Create( LanguageElement.LeftShiftOperator  , SyntaxKind.LessThanLessThanToken   ),
               Tuple.Create( LanguageElement.RightShiftOperator  , SyntaxKind.GreaterThanGreaterThanToken   ),
               Tuple.Create( LanguageElement.EqualOperator  , SyntaxKind.EqualsEqualsToken ),
               Tuple.Create( LanguageElement.NotEqualOperator  , SyntaxKind.ExclamationEqualsToken ),
               Tuple.Create( LanguageElement.LessThanOperator  , SyntaxKind.LessThanToken   ),
               Tuple.Create( LanguageElement.GreaterThanOperator  , SyntaxKind.GreaterThanToken   ),
               Tuple.Create( LanguageElement.LessThanOrEqualOperator  , SyntaxKind.LessThanEqualsToken ),
               Tuple.Create( LanguageElement.GreaterThanOrEqualOperator  , SyntaxKind.GreaterThanEqualsToken   ),
             };

      internal IEnumerable<LanguageElement> NotUsedInWhitespaceList(IEnumerable<Whitespace2> whitespaceList)
      {
         var ret = languageElementToKind
                     .Select(x => x.Key)
                     .ToList();
         foreach (var whitespace in whitespaceList)
         {
            var remove = ret.Where(x => x == whitespace.LanguageElement);
            if (remove.Any())
            { ret.Remove(remove.First()); }
         }
         return ret;
      }
   }
}