using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomComment : RDomBase, IComment
    {
        public string Text { get; set; }

        public bool IsMultiline { get; set; }

        public MemberKind MemberKind
        { get { return MemberKind.Whitespace; } }

        public override object OriginalRawItem
        { get { return null; } }

        public override string OuterName
        { get { return null; } }

        public override object RawItem
        { get { return null; } }

        public StemMemberKind StemMemberKind
        { get { return StemMemberKind.Whitespace; } }

        public override ISymbol Symbol
        { get { return null; } }

        public override object RequestValue(string propertyName)
        {  return null; } 


        protected override bool SameIntentInternal<TLocal>(TLocal other, bool includePublicAnnotations)
        {
            var otherAsT = other as IComment;
            if (otherAsT == null) return false;
            if (Text == otherAsT.Text) return false;
            // Don't test multi-line, if that's the only difference, it's the same intent
            return true;
        }
    }
}
