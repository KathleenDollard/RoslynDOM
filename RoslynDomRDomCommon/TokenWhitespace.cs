using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace RoslynDom
{
    public abstract class TokenWhitespace
    {

        public TokenWhitespace(SyntaxToken token, string leadingWhitespace, string trailingWhitespace)
        {
            Token = token;
            LeadingWhitespace = leadingWhitespace;
            TrailingWhitespace = trailingWhitespace;
        }

        public TokenWhitespace(SyntaxToken token)
        {
            Token= token;
        }

        public SyntaxToken Token { get; private set; }
        public string LeadingWhitespace { get;  set; }
        public string TrailingWhitespace { get;  set; }
    }

    public class TokenWhitespace<TSyntaxNode> : TokenWhitespace
        where TSyntaxNode : SyntaxNode 
    {
        public TokenWhitespace(SyntaxToken token, string leadingWhitespace, string trailingWhitespace,
            Func<TSyntaxNode, SyntaxToken, TSyntaxNode> withDelegate)
            : base(token, leadingWhitespace, trailingWhitespace)
        {
            WithDelegate = withDelegate;
        }

        public Func<TSyntaxNode, SyntaxToken, TSyntaxNode> WithDelegate { get; private set; }
    }

    public class TokenWhitespaceList<TSyntaxNode> : IEnumerable<TokenWhitespace>
        where TSyntaxNode : SyntaxNode
    {
        private List<TokenWhitespace> _list = new List<TokenWhitespace>();

        public TokenWhitespace this[int rawKind]
        {
            get { return _list.Where(x => x.Token.RawKind == rawKind).FirstOrDefault(); }
        }

        public IEnumerator<TokenWhitespace> GetEnumerator()
        { return _list.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }

        public void Add(TokenWhitespace<TSyntaxNode> item)
        { _list.Add(item); }
    }
}
