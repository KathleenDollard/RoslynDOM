using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
 using System.ComponentModel.DataAnnotations;
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

        private StemMemberKind stemMemberKind ;
        public StemMemberKind StemMemberKind { get {return stemMemberKind; }
set {SetProperty(ref stemMemberKind, value); }}
        private MemberKind memberKind ;
        public MemberKind MemberKind { get {return memberKind; }
set {SetProperty(ref memberKind, value); }}

        public override object OriginalRawItem
        { get { return null; } }

        public override object RawItem
        { get { return null; } }

         public override object RequestValue(string propertyName)
        { return null; }
    }
}
