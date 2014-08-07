using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class TokenWhitespaceSet
    {
        private List<TokenWhitespace> _tokenTrivia = new List<TokenWhitespace>();

        public TokenWhitespaceSet(bool includeLeadingTrailing)
        { IncludeLeadingTrailing = includeLeadingTrailing;     }

      /// <summary>
        /// 
        /// </summary>
        /// <param name="oldTokenWS"></param>
        /// <remarks>
        /// This hijacks an idea from IDom functionality, but is not itself an IDom object (and should not be as it is implementation detail)
        /// </remarks>  
        private TokenWhitespaceSet(TokenWhitespaceSet oldTokenWS)
        {
            var whitespace = RoslynDomUtilities.CopyMembers(oldTokenWS._tokenTrivia);
            _tokenTrivia.AddRange(whitespace);

            IncludeLeadingTrailing = oldTokenWS.IncludeLeadingTrailing;
            LeadingWhitespace = oldTokenWS.LeadingWhitespace;
            TrailingWhitespace = oldTokenWS.TrailingWhitespace;
        }

        public TokenWhitespaceSet Copy()
        {
            return new TokenWhitespaceSet(this);
        }

  
        public string LeadingWhitespace { get; set; }
        public string TrailingWhitespace { get; set; }
        public bool IncludeLeadingTrailing { get; set; }

        public List<TokenWhitespace> TokenWhitespaceList
        { get { return _tokenTrivia; } }
    }

    public class TokenWhitespace
    {

        public TokenWhitespace(SyntaxToken token, string leadingWhitespace, string trailingWhitespace)
        {
            Token = token;
            LeadingWhitespace = leadingWhitespace == null ? "" : leadingWhitespace;
            TrailingWhitespace = trailingWhitespace == null ? "" : trailingWhitespace;
        }


        public TokenWhitespace(SyntaxToken token)
        {
            Token = token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldTokenWS"></param>
        /// <remarks>
        /// This hijacks some IDom functionality, but is not itself an IDom object (and should not be as it is implementation detail)
        /// </remarks>  
        public TokenWhitespace(TokenWhitespace oldTokenWS)
        {
            Token = oldTokenWS.Token;
            this.LeadingWhitespace = oldTokenWS.LeadingWhitespace;
            this.TrailingWhitespace = oldTokenWS.TrailingWhitespace;
        }
        public SyntaxToken Token { get; private set; }
        public string LeadingWhitespace { get; set; }
        public string TrailingWhitespace { get; set; }
    }

}
