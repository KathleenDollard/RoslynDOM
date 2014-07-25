using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomComment : RDomCommentWhite, IComment
    {
        public RDomComment (string text, bool isMultiline)
            : base(StemMemberKind.Comment, MemberKind.Comment )
        {
            Text = text;
            IsMultiline = isMultiline;
        }
        internal RDomComment(  RDomComment oldRDom)
            : base(oldRDom)
        {
            Text = oldRDom.Text;
            IsMultiline = oldRDom.IsMultiline;
        }

        public string Text { get; set; }

        public bool IsMultiline { get; set; }

        protected override bool SameIntentInternal<TLocal>(TLocal other, bool skipPublicAnnotations)
        {
            var otherAsT = other as IComment;
            if (otherAsT == null) return false;
            if (Text == otherAsT.Text) return false;
            // Don't test multi-line, if that's the only difference, it's the same intent
            return true;
        }
    }
}
