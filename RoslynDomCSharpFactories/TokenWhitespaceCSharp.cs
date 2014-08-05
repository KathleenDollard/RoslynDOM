using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslynDom.CSharp
{
    public class TokenWhitespaceCSharp : TokenWhitespace
    {
        public TokenWhitespaceCSharp(SyntaxToken token, string leadingWhitespace, string trailingWhitespace)
            : base(token)
        {
            this.LeadingWhitespace = leadingWhitespace;
            this.TrailingWhitespace = trailingWhitespace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldTokenWS"></param>
        /// <remarks>
        /// This hijacks some IDom functionality, but is not itself an IDom object (and should not be as it is implementation detail)
        /// </remarks>
        public TokenWhitespaceCSharp(TokenWhitespaceCSharp oldTokenWS)
             : base(oldTokenWS.Token)
        {
            this.LeadingWhitespace =  oldTokenWS.LeadingWhitespace;
            this.TrailingWhitespace = oldTokenWS.TrailingWhitespace;
        }

        public static string GetWhitespace(SyntaxTriviaList triviaList)
        {
            var wsTrivia = triviaList
                            .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia
                                || x.CSharpKind() == SyntaxKind.EndOfLineTrivia);
            return  string.Join("", wsTrivia.Select(x => x.ToString()));
        }
    }

}
