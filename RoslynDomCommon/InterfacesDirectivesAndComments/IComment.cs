namespace RoslynDom.Common
{
   /// <summary>
   ///
   /// </summary>
   public interface IComment : IDetail<IComment>
   {
      string Text { get; set; }

      bool IsMultiline { get; set; }
   }
}