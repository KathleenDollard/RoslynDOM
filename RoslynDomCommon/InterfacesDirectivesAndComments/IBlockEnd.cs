namespace RoslynDom.Common
{
   /// <summary>
   ///
   /// </summary>
   public interface IDetailBlockEnd : IDetail<IDetailBlockEnd>, IHasGroup
   {
      IDetailBlockStart BlockStart { get; }

      string BlockStyleName { get; }
   }
}