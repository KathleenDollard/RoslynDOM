using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomVerticalWhitespace : RDomCommentWhite, IVerticalWhitespace
    {
        public RDomVerticalWhitespace(int count, bool isElastic)
            : base(StemMemberKind.Whitespace, MemberKind.Whitespace)
        {
            Count = count;
            IsElastic = isElastic;
        }

        internal RDomVerticalWhitespace(RDomVerticalWhitespace oldRDom)
            : base(oldRDom)
        {
            Count = oldRDom.Count;
            IsElastic = oldRDom.IsElastic;
        }

        public int Count { get; set; }
        public bool IsElastic { get; set; }

        protected override bool SameIntentInternal<TLocal>(TLocal other, bool skipPublicAnnotations)
        {
            var otherAsT = other as IVerticalWhitespace;
            if (otherAsT == null) return false;
            return (Count == otherAsT.Count);
        }
    }
}
