using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;

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
    }
}
