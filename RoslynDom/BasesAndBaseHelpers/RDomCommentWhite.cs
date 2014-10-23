using System;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public abstract class RDomCommentWhite<T> : RDomBase<T>, ICommentWhite<T>
       where T : class, ICommentWhite<T>
   {
      internal RDomCommentWhite(StemMemberKind stemMemberKind, MemberKind memberKind)
      {
         _stemMemberKind = stemMemberKind;
         _memberKind = memberKind;
      }

      internal RDomCommentWhite(T oldRDom)
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

      public override ISymbol Symbol
      { get { return null; } }
   }
}
