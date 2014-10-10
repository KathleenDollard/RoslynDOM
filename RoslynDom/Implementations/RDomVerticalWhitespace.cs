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

      // TODO: This is not going to be updated by the generator, consider how this affects the RoslynDom
      private int _count ;
      public int Count { get {return _count; }
set {SetProperty(ref _count, value); }}
      private bool _isElastic ;
      public bool IsElastic { get {return _isElastic; }
set {SetProperty(ref _isElastic, value); }}

      protected override bool SameIntentInternal<TLocal>(TLocal other, bool skipPublicAnnotations)
      {
         var otherAsT = other as IVerticalWhitespace;
         if (otherAsT == null) return false;
         return (Count == otherAsT.Count);
      }
   }
}
