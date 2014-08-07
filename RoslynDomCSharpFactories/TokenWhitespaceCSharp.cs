using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslynDom.CSharp
{
    //public class TokenWhitespaceCSharp : TokenWhitespace
    //{
    //    public TokenWhitespaceCSharp(SyntaxToken token, string leadingWhitespace, string trailingWhitespace)
    //        : base(token, leadingWhitespace, trailingWhitespace)
    //    { }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="oldTokenWS"></param>
    //    /// <remarks>
    //    /// This hijacks some IDom functionality, but is not itself an IDom object (and should not be as it is implementation detail)
    //    /// </remarks>
    //    public TokenWhitespaceCSharp(TokenWhitespaceCSharp oldTokenWS)
    //         : base(oldTokenWS)
    //    { }

    //    public static string GetWhitespace(SyntaxTriviaList triviaList, bool removeEol)
    //    {
    //        var wsTrivia = triviaList
    //                        .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia
    //                            || x.CSharpKind() == SyntaxKind.EndOfLineTrivia);
    //        if (removeEol)
    //        {
    //            // There might be multiple EOLs in the leading trivia that we need to strip here
    //            var list = new List<SyntaxTrivia>();
    //            foreach (var item in wsTrivia)
    //            {
    //                if (item.CSharpKind() == SyntaxKind.EndOfLineTrivia) // empty the list and start over
    //                { list = new List<SyntaxTrivia>(); }
    //                else { list.Add(item); }
    //            }
    //            wsTrivia = list;
    //        }
    //        return string.Join("", wsTrivia.Select(x => x.ToString()));
    //    }
    //}

}
