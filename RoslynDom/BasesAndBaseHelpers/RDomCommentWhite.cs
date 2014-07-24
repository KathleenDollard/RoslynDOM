using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomCommentWhite : RDomBase, ICommentWhite 
    {
        internal RDomCommentWhite(StemMemberKind stemMemberKind, MemberKind memberKind)
        {
            StemMemberKind = stemMemberKind;
            MemberKind = memberKind;
        }

        internal RDomCommentWhite(RDomCommentWhite oldRDom)
            : base(oldRDom)
        {
            StemMemberKind = oldRDom.StemMemberKind;
            MemberKind = oldRDom.MemberKind;
        }

        public StemMemberKind StemMemberKind { get; set; }
        public MemberKind MemberKind { get; set; }

        public override object OriginalRawItem
        { get { return null; } }

        //public override string OuterName
        //{ get { return null; } }

        public override object RawItem
        { get { return null; } }

        public override ISymbol Symbol
        { get { return null; } }

        public override object RequestValue(string propertyName)
        { return null; }
      
    }

}
