using RoslynDom.Common;

namespace RoslynDom
{
   public abstract class RDomCommentWhite : RDomBase, ICommentWhite
   {
      internal RDomCommentWhite(StemMemberKind stemMemberKind, MemberKind memberKind)
      {
         _stemMemberKind = stemMemberKind;
         _memberKind = memberKind;
      }

      internal RDomCommentWhite(RDomCommentWhite oldRDom)
          : base(oldRDom)
      {
         _stemMemberKind = oldRDom.StemMemberKind;
         _memberKind = oldRDom.MemberKind;
      }

      private StemMemberKind _stemMemberKind;
      public StemMemberKind StemMemberKind
      {
         get { return _stemMemberKind; }
      }

      private MemberKind _memberKind;
      public MemberKind MemberKind
      {
         get { return _memberKind; }
      }

      public override object OriginalRawItem
      { get { return null; } }

      public override object RawItem
      { get { return null; } }

      public override object RequestValue(string propertyName)
      { return null; }
   }
}
