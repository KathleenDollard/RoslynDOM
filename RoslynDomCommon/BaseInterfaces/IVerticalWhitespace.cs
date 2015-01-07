namespace RoslynDom.Common
{
   public interface IVerticalWhitespace : IDetail<IVerticalWhitespace>
   {
      int Count { get; set; }

      bool IsElastic { get; set; }
   }
}