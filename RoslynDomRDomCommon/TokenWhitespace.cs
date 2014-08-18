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

        public TokenWhitespace(SyntaxNodeOrToken nodeOrToken, string leadingWhitespace, string trailingWhitespace)
        {
            NodeOrToken = nodeOrToken;
            LeadingWhitespace = leadingWhitespace == null ? "" : leadingWhitespace;
            TrailingWhitespace = trailingWhitespace == null ? "" : trailingWhitespace;
            Marker =Guid.NewGuid();
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
            NodeOrToken = oldTokenWS.NodeOrToken;
            this.LeadingWhitespace = oldTokenWS.LeadingWhitespace;
            this.TrailingWhitespace = oldTokenWS.TrailingWhitespace;
            this.Marker = oldTokenWS.Marker;
        }
        public SyntaxNodeOrToken NodeOrToken { get; private set; }
        public string LeadingWhitespace { get; set; }
        public string TrailingWhitespace { get; set; }

        public Guid Marker { get; private set; }  

        public override string ToString()
        {
            return base.ToString() + "{" + NodeOrToken.ToString() + "}";
        }
    }

}
