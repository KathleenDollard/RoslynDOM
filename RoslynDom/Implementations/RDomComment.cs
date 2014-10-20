using RoslynDom.Common;
namespace RoslynDom
{
   public class RDomComment : RDomCommentWhite, IComment
   {
      public RDomComment(string text, bool isMultiline = false)
          : base(StemMemberKind.Comment, MemberKind.Comment)
      {
         _text = text;
         _isMultiline = isMultiline;
      }
      internal RDomComment(RDomComment oldRDom)
          : base(oldRDom)
      {
         _text = oldRDom.Text;
         _isMultiline = oldRDom.IsMultiline;
      }

      private string _text;
      public string Text
      {
         get { return _text; }
         set { SetProperty(ref _text, value); }
      }

      private bool _isMultiline;
      public bool IsMultiline
      {
         get { return _isMultiline; }
         set { SetProperty(ref _isMultiline, value); }
      }

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
