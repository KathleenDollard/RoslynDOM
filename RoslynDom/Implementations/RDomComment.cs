using RoslynDom.Common;
namespace RoslynDom
{
   public class RDomComment : RDomDetail<IComment>, IComment
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
   }
}
